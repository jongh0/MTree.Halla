using GalaSoft.MvvmLight.Command;
using MTree.Configuration;
using MTree.DbProvider;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace MTree.DataValidator
{

    public interface IWindowFactory
    {
        DialogResult CreateNewWindow(string uri);
    }

    public class RecoveryPopupWindowFactory : IWindowFactory
    {
        private RecoveryPopup PopupWindow = new RecoveryPopup();
        public RecoveryPopupWindowFactory(object dataContext)
        {
            PopupWindow = new RecoveryPopup
            {
                DataContext = dataContext
            };
        }

        public DialogResult CreateNewWindow(string uri)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                PopupWindow.SetUri(uri);
                PopupWindow.ShowDialog();
            }));
            return PopupWindow.Result;
        }
    }
    public class RecoveryViewModel: INotifyPropertyChanged
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        
        private const string logBasePath = "Logs";
        private const string compareResultPath = "CompareResult";
        private const string codeCompareResultFile = "CodeCompare.html";
        private const string masterCompareResultFile = "MasterCompare.html";
        private const string stockConclusionCompareResultPath = "StockConclusion";
        private const string indexConclusionCompareResultPath = "IndexConclusion";
        private const string circuitbreakCompareResultFile = "CircuitBreak.html";

        private object popupWindowLockObject = new object();

        private IWindowFactory RecoveryPopupFactory { get; }

        public RecoveryViewModel()
        {
            RecoveryPopupFactory = new RecoveryPopupWindowFactory(this);
        }

        public DataValidator Validator { get; set; } = new DataValidator();

        public DataRecoverer Recoverer { get; set; } = new DataRecoverer();

        private DateTime _StartingDate = DateTime.Now;
        public DateTime StartingDate
        {
            get
            {
                return _StartingDate;
            }
            set
            {
                _StartingDate = value;
                NotifyPropertyChanged(nameof(StartingDate));
            }
        }

        private DateTime _EndingDate = DateTime.Now;
        public DateTime EndingDate
        {
            get
            {
                return _EndingDate;
            }
            set
            {
                _EndingDate = value;
                NotifyPropertyChanged(nameof(EndingDate));
            }
        }

        private string _Code;
        public string Code
        {
            get
            {
                return _Code;
            }
            set
            {
                _Code = value;
                NotifyPropertyChanged(nameof(Code));
            }
        }

        private bool _ValidateBeforeRecovery = true;
        public bool ValidateBeforeRecovery
        {
            get
            {
                return _ValidateBeforeRecovery;
            }
            set
            {
                _ValidateBeforeRecovery = value;
                NotifyPropertyChanged(nameof(ValidateBeforeRecovery));
            }
        }

        private bool _AskBeforeRecovery = true;
        public bool AskBeforeRecovery
        {
            get
            {
                return _AskBeforeRecovery;
            }
            set
            {
                _AskBeforeRecovery = value;
                NotifyPropertyChanged(nameof(AskBeforeRecovery));
            }
        }
        
        private bool _ApplyForAll = false;
        public bool ApplyForAll
        {
            get
            {
                return _ApplyForAll;
            }
            set
            {
                _ApplyForAll = value;
                NotifyPropertyChanged(nameof(ApplyForAll));
            }
        }

        private bool _FromSourceToDestination = true;
        public bool FromSourceToDestination
        {
            get
            {
                return _FromSourceToDestination;
            }
            set
            {
                _FromSourceToDestination = value;
                NotifyPropertyChanged(nameof(FromSourceToDestination));

                if (FromSourceToDestination == true)
                {
                    Recoverer.From = DbAgent.Instance;
                    Recoverer.To = DbAgent.RemoteInstance;
                }
                else
                {
                    Recoverer.From = DbAgent.RemoteInstance;
                    Recoverer.To = DbAgent.Instance;
                }
            }
        }

        private RelayCommand _RecoverAllCommand;
        public ICommand RecoverAllCommand
        {
            get
            {
                if (_RecoverAllCommand == null)
                    _RecoverAllCommand = new RelayCommand(() =>
                    Task.Run(() =>
                    {
                        DoRecoverMasterAll();
                        DoRecoverStockConclusionAll();
                        DoRecoverIndexConclusionAll();
                        DoRecoverCircuitBreak();
                    }));

                return _RecoverAllCommand;
            }
        }

        private RelayCommand _RecoverMasterAllCommand;
        public ICommand RecoverMasterAllCommand
        {
            get
            {
                if (_RecoverMasterAllCommand == null)
                    _RecoverMasterAllCommand = new RelayCommand(() =>
                    Task.Run(() =>
                    {
                        DoRecoverMasterAll();
                    }));

                return _RecoverMasterAllCommand;
            }
        }
        private void DoRecoverMasterAll()
        {
            List<string> codeList = new List<string>();
            codeList.AddRange(DbAgent.Instance.GetCollectionList(DbTypes.IndexMaster).OrderBy(s => s));
            codeList.AddRange(DbAgent.Instance.GetCollectionList(DbTypes.StockMaster).OrderBy(s => s));

            for (DateTime targetDate = StartingDate; targetDate <= EndingDate; targetDate = targetDate.AddDays(1))
            {                
                Parallel.ForEach(codeList, new ParallelOptions { MaxDegreeOfParallelism = Config.Validator.ThreadLimit }, code =>
                {
                    if (Validator.ValidateMaster(targetDate, code, false) == false)
                    {
                        Recoverer.RecoverMaster(targetDate, code, AskBeforeRecovery);
                    }
                });   
            }
            logger.Info("Master Recovery Done.");
        }

        private RelayCommand _RecoverMasterCommand;
        public ICommand RecoverMasterCommand
        {
            get
            {
                if (_RecoverMasterCommand == null)
                    _RecoverMasterCommand = new RelayCommand(() =>
                    Task.Run(() =>
                    {
                        for (DateTime targetDate = StartingDate; targetDate <= EndingDate; targetDate = targetDate.AddDays(1))
                        {
                            if (ValidateBeforeRecovery == true)
                            {
                                if (Validator.ValidateMaster(targetDate, Code, false) == true)
                                {
                                    logger.Info($"Master of {Code} is same. Do nothing");
                                    return;
                                }
                            }

                            if (Recoverer != null)
                            {
                                Recoverer.RecoverMaster(targetDate, Code, AskBeforeRecovery);
                            }
                            else
                            {
                                logger.Error("Validator is not assigned");
                            }
                        }
                    }));

                return _RecoverMasterCommand;
            }
        }

        private RelayCommand _RecoverStockConclusionAllCommand;
        public ICommand RecoverStockConclusionAllCommand
        {
            get
            {
                if (_RecoverStockConclusionAllCommand == null)
                    _RecoverStockConclusionAllCommand = new RelayCommand(() =>
                    Task.Run(() =>
                    {
                        DoRecoverStockConclusionAll();
                    }));

                return _RecoverStockConclusionAllCommand;
            }
        }

        private void DoRecoverStockConclusionAll()
        {
            List<string> codeList = new List<string>();
            codeList.AddRange(DbAgent.Instance.GetCollectionList(DbTypes.StockMaster).OrderBy(s => s));

            for (DateTime targetDate = StartingDate; targetDate <= EndingDate; targetDate = targetDate.AddDays(1))
            {
                Parallel.ForEach(codeList, new ParallelOptions { MaxDegreeOfParallelism = Config.Validator.ThreadLimit }, code =>
                {
                    if (Validator.ValidateStockConclusion(targetDate, code, true) == false)
                    {
                        DialogResult needRecovery = DialogResult.None;
                        lock (popupWindowLockObject)
                        {
                            if (ApplyForAll == false)
                                needRecovery = RecoveryPopupFactory.CreateNewWindow(Path.Combine(logBasePath, Config.General.DateNow, compareResultPath, stockConclusionCompareResultPath, code + ".html"));
                            else
                                needRecovery = DialogResult.OK;
                        }
                        if (needRecovery == DialogResult.OK)
                        {
                            Recoverer.RecoverStockConclusion(targetDate, code);
                        }
                    }
                });
            }
            logger.Info("Stock Conclusion Recovery Done.");
        }

        private RelayCommand _RecoverStockConclusionCommand;
        public ICommand RecoverStockConclusionCommand
        {
            get
            {
                if (_RecoverStockConclusionCommand == null)
                    _RecoverStockConclusionCommand = new RelayCommand(() =>
                    Task.Run(() =>
                    {
                        for (DateTime targetDate = StartingDate; targetDate <= EndingDate; targetDate = targetDate.AddDays(1))
                        {
                            if (Validator.ValidateStockConclusion(targetDate, Code, true) == false)
                            {
                                DialogResult needRecovery = DialogResult.None;
                                lock (popupWindowLockObject)
                                {
                                    needRecovery = RecoveryPopupFactory.CreateNewWindow(Path.Combine(logBasePath, Config.General.DateNow, compareResultPath, stockConclusionCompareResultPath, Code + ".html"));
                                }
                                if (needRecovery == DialogResult.OK)
                                {
                                    Recoverer.RecoverStockConclusion(targetDate, Code);
                                }
                                
                            }
                        }
                    }));

                return _RecoverStockConclusionCommand;
            }
        }

        private RelayCommand _RecoverIndexConclusionAllCommand;
        public ICommand RecoverIndexConclusionAllCommand
        {
            get
            {
                if (_RecoverIndexConclusionAllCommand == null)
                    _RecoverIndexConclusionAllCommand = new RelayCommand(() =>
                    Task.Run(() =>
                    {
                        DoRecoverIndexConclusionAll();
                    }));

                return _RecoverIndexConclusionAllCommand;
            }
        }
        private void DoRecoverIndexConclusionAll()
        {
            List<string> codeList = new List<string>();
            codeList.AddRange(DbAgent.Instance.GetCollectionList(DbTypes.IndexMaster).OrderBy(s => s));

            for (DateTime targetDate = StartingDate; targetDate <= EndingDate; targetDate = targetDate.AddDays(1))
            {
                Parallel.ForEach(codeList, new ParallelOptions { MaxDegreeOfParallelism = Config.Validator.ThreadLimit }, code =>
                {
                    if (Validator.ValidateIndexConclusion(targetDate, code, false) == false)
                    {
                        Recoverer.RecoverIndexConclusion(targetDate, code, AskBeforeRecovery);
                    }
                });
            }
            logger.Info("Index Conclusion Recovery Done.");
        }

        private RelayCommand _RecoverIndexConclusionCommand;
        public ICommand RecoverIndexConclusionCommand
        {
            get
            {
                if (_RecoverIndexConclusionCommand == null)
                    _RecoverIndexConclusionCommand = new RelayCommand(() =>
                    Task.Run(() =>
                    {
                        for (DateTime targetDate = StartingDate; targetDate <= EndingDate; targetDate = targetDate.AddDays(1))
                        {
                            if (ValidateBeforeRecovery == true)
                            {
                                if (Validator.ValidateIndexConclusion(targetDate, Code, false) == true)
                                {
                                    logger.Info($"Index Conclusion of {Code} is same. Do nothing");
                                    return;
                                }
                            }

                            if (Recoverer != null)
                            {
                                Recoverer.RecoverIndexConclusion(targetDate, Code, AskBeforeRecovery);
                            }
                            else
                            {
                                logger.Error("Validator is not assigned");
                            }
                        }
                    }));

                return _RecoverIndexConclusionCommand;
            }
        }
        
        private RelayCommand _RecoverCircuitBreakCommand;
        public ICommand RecoverCircuitBreakCommand
        {
            get
            {
                if (_RecoverCircuitBreakCommand == null)
                    _RecoverCircuitBreakCommand = new RelayCommand(() =>
                    Task.Run(() =>
                    {
                        DoRecoverCircuitBreak();
                    }));

                return _RecoverCircuitBreakCommand;
            }
        }
        private void DoRecoverCircuitBreak()
        {
            List<string> codeList = new List<string>();
            codeList.AddRange(DbAgent.Instance.GetCollectionList(DbTypes.StockMaster).OrderBy(s => s));

            for (DateTime targetDate = StartingDate; targetDate <= EndingDate; targetDate = targetDate.AddDays(1))
            {
                Parallel.ForEach(codeList, new ParallelOptions { MaxDegreeOfParallelism = Config.Validator.ThreadLimit }, code =>
                {
                    if (Validator.ValidateCircuitBreak(targetDate, code, false) == false)
                    {
                        Recoverer.RecoverCircuitBreak(targetDate, code, AskBeforeRecovery);
                    }
                });
            }
            logger.Info("Circuit Break Recovery Done.");
        }
        
        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}
