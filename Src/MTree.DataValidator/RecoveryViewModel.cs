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
        private object _DataContext;
        private RecoveryPopup PopupWindow = new RecoveryPopup();

        public RecoveryPopupWindowFactory(object dataContext)
        {
            _DataContext = dataContext;
        }

        public DialogResult CreateNewWindow(string uri)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                PopupWindow = new RecoveryPopup
                {
                    DataContext = _DataContext
                };

                PopupWindow.SetUri(uri);
                PopupWindow.ShowDialog();
            }));

            return PopupWindow.Result;
        }
    }

    public partial class MainViewModel
    {        
        private const string logBasePath = "Logs";
        private const string compareResultPath = "CompareResult";
        private const string codeCompareResultFile = "CodeCompare.html";
        private const string masterCompareResultPath = "Master";
        private const string stockConclusionCompareResultPath = "StockConclusion";
        private const string indexConclusionCompareResultPath = "IndexConclusion";
        private const string circuitbreakCompareResultPath = "CircuitBreak";

        private object popupWindowLockObject = new object();

        private IWindowFactory RecoveryPopupFactory { get; }

        private DateTime _StartingDate = DateTime.Now;
        public DateTime StartingDate
        {
            get { return _StartingDate; }
            set
            {
                _StartingDate = value;

                if (EndingDate < _StartingDate)
                    EndingDate = _StartingDate;

                NotifyPropertyChanged(nameof(StartingDate));
            }
        }

        private DateTime _EndingDate = DateTime.Now;
        public DateTime EndingDate
        {
            get { return _EndingDate; }
            set
            {
                _EndingDate = value;

                if (_EndingDate < StartingDate)
                    StartingDate = _EndingDate;

                NotifyPropertyChanged(nameof(EndingDate));
            }
        }

        private string _Code;
        public string Code
        {
            get { return _Code; }
            set
            {
                _Code = value.Trim();
                NotifyPropertyChanged(nameof(Code));
            }
        }

        private bool _ValidateOnly = true;
        public bool ValidateOnly
        {
            get { return _ValidateOnly; }
            set
            {
                _ValidateOnly = value;
                NotifyPropertyChanged(nameof(ValidateOnly));
            }
        }

        private bool _ApplyForAll = false;
        public bool ApplyForAll
        {
            get { return _ApplyForAll; }
            set
            {
                _ApplyForAll = value;
                NotifyPropertyChanged(nameof(ApplyForAll));
            }
        }

        private bool _FromSourceToDestination = true;
        public bool FromSourceToDestination
        {
            get { return _FromSourceToDestination; }
            set
            {
                _FromSourceToDestination = value;
                NotifyPropertyChanged(nameof(FromSourceToDestination));

                if (_FromSourceToDestination == true)
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
                        DoRecoverAll();
                    }));

                return _RecoverAllCommand;
            }
        }

        private void DoRecoverAll()
        {
            DoRecoverMasterAll();
            DoRecoverStockConclusionAll();
            DoRecoverIndexConclusionAll();
            DoRecoverCircuitBreakAll();
        }

        private RelayCommand _RecoverMasterAllCommand;
        public ICommand RecoverMasterAllCommand
        {
            get
            {
                if (_RecoverMasterAllCommand == null)
                    _RecoverMasterAllCommand = new RelayCommand(() => Task.Run(() => DoRecoverMasterAll()));

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
                int cnt = 0;
                Parallel.ForEach(codeList, new ParallelOptions { MaxDegreeOfParallelism = Config.Validator.ThreadLimit }, code =>
                {
                    if (Validator.ValidateMaster(targetDate, code, true) == false)
                    {
                        logger.Error($"Master Validation for {code} of {targetDate.ToString("yyyy-MM-dd")} Fail. {Interlocked.Increment(ref cnt)}/{codeList.Count}");
                        if (ValidateOnly == false)
                        {
                            DialogResult needRecovery = DialogResult.None;

                            lock (popupWindowLockObject)
                            {
                                if (ApplyForAll == false)
                                    needRecovery = RecoveryPopupFactory.CreateNewWindow(Path.Combine(logBasePath, Config.General.DateNow, compareResultPath, masterCompareResultPath, code + ".html"));
                                else
                                    needRecovery = DialogResult.OK;
                            }

                            if (needRecovery == DialogResult.OK)
                                Recoverer.RecoverMaster(targetDate, code);
                        }
                    }
                    else
                    {
                        logger.Info($"Master Validation for {code} of {targetDate.ToString("yyyy-MM-dd")} success. {Interlocked.Increment(ref cnt)}/{codeList.Count}");
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
                            if (Validator.ValidateMaster(targetDate, Code, true) == false)
                            {
                                logger.Error($"Master Validation for {Code} of {targetDate.ToString("yyyy-MM-dd")} Fail.");
                                if (ValidateOnly == false)
                                {
                                    DialogResult needRecovery = DialogResult.None;
                                    lock (popupWindowLockObject)
                                    {
                                        if (ApplyForAll == false)
                                            needRecovery = RecoveryPopupFactory.CreateNewWindow(Path.Combine(logBasePath, Config.General.DateNow, compareResultPath, masterCompareResultPath, Code + ".html"));
                                        else
                                            needRecovery = DialogResult.OK;
                                    }
                                    if (needRecovery == DialogResult.OK)
                                    {
                                        Recoverer.RecoverMaster(targetDate, Code);
                                    }
                                }
                            }
                            else
                            {
                                logger.Error($"Master Validation for {Code} of {targetDate.ToString("yyyy-MM-dd")} success.");
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
                    _RecoverStockConclusionAllCommand = new RelayCommand(() => Task.Run(() => DoRecoverStockConclusionAll()));

                return _RecoverStockConclusionAllCommand;
            }
        }

        private void DoRecoverStockConclusionAll()
        {
            List<string> codeList = new List<string>();
            codeList.AddRange(DbAgent.Instance.GetCollectionList(DbTypes.StockMaster).OrderBy(s => s));

            for (DateTime targetDate = StartingDate; targetDate <= EndingDate; targetDate = targetDate.AddDays(1))
            {
                int cnt = 0;
                Parallel.ForEach(codeList, new ParallelOptions { MaxDegreeOfParallelism = Config.Validator.ThreadLimit }, code =>
                {
                    if (Validator.ValidateStockConclusion(targetDate, code, true) == false)
                    {
                        logger.Error($"Stock Conclusion Validation for {code} of {targetDate.ToString("yyyy-MM-dd")} Fail. {Interlocked.Increment(ref cnt)}/{codeList.Count}");

                        if (ValidateOnly == false)
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
                                Recoverer.RecoverStockConclusion(targetDate, code);
                        }
                    }
                    else
                    {
                        logger.Info($"Stock Conclusion Validation for {code} of {targetDate.ToString("yyyy-MM-dd")} success. {Interlocked.Increment(ref cnt)}/{codeList.Count}");
                    }
                });

                ApplyForAll = false;
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
                                if (ValidateOnly == false)
                                {
                                    DialogResult needRecovery = DialogResult.None;

                                    lock (popupWindowLockObject)
                                    {
                                        needRecovery = RecoveryPopupFactory.CreateNewWindow(Path.Combine(logBasePath, Config.General.DateNow, compareResultPath, stockConclusionCompareResultPath, Code + ".html"));
                                    }

                                    if (needRecovery == DialogResult.OK)
                                        Recoverer.RecoverStockConclusion(targetDate, Code);
                                }
                            }
                        }

                        ApplyForAll = false;
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
                    _RecoverIndexConclusionAllCommand = new RelayCommand(() => Task.Run(() => DoRecoverIndexConclusionAll()));

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
                    if (Validator.ValidateIndexConclusion(targetDate, code, true) == false)
                    {
                        if (ValidateOnly == false)
                        {
                            DialogResult needRecovery = DialogResult.None;

                            lock (popupWindowLockObject)
                            {
                                if (File.Exists(Path.Combine(logBasePath, Config.General.DateNow, compareResultPath, indexConclusionCompareResultPath, code + ".html")) == false)
                                    logger.Error($"{Path.Combine(logBasePath, Config.General.DateNow, compareResultPath, indexConclusionCompareResultPath, code + ".html")} is not exist.");

                                if (ApplyForAll == false)
                                    needRecovery = RecoveryPopupFactory.CreateNewWindow(Path.Combine(logBasePath, Config.General.DateNow, compareResultPath, indexConclusionCompareResultPath, code + ".html"));
                                else
                                    needRecovery = DialogResult.OK;
                            }

                            if (needRecovery == DialogResult.OK)
                                Recoverer.RecoverIndexConclusion(targetDate, code);
                        }
                    }
                });

                ApplyForAll = false;
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
                            if (Validator.ValidateIndexConclusion(targetDate, Code, true) == false)
                            {
                                if (ValidateOnly == false)
                                {
                                    DialogResult needRecovery = DialogResult.None;

                                    lock (popupWindowLockObject)
                                    {
                                        needRecovery = RecoveryPopupFactory.CreateNewWindow(Path.Combine(logBasePath, Config.General.DateNow, compareResultPath, indexConclusionCompareResultPath, Code + ".html"));
                                    }

                                    if (needRecovery == DialogResult.OK)
                                        Recoverer.RecoverIndexConclusion(targetDate, Code);
                                }
                            }
                        }
                    }));

                return _RecoverIndexConclusionCommand;
            }
        }
        
        private RelayCommand _RecoverCircuitBreakAllCommand;
        public ICommand RecoverCircuitBreakAllCommand
        {
            get
            {
                if (_RecoverCircuitBreakAllCommand == null)
                    _RecoverCircuitBreakAllCommand = new RelayCommand(() => Task.Run(() => DoRecoverCircuitBreakAll()));

                return _RecoverCircuitBreakAllCommand;
            }
        }
        private void DoRecoverCircuitBreakAll()
        {
            List<string> codeList = new List<string>();
            codeList.AddRange(DbAgent.Instance.GetCollectionList(DbTypes.StockMaster).OrderBy(s => s));

            for (DateTime targetDate = StartingDate; targetDate <= EndingDate; targetDate = targetDate.AddDays(1))
            {
                Parallel.ForEach(codeList, new ParallelOptions { MaxDegreeOfParallelism = Config.Validator.ThreadLimit }, code =>
                {
                    if (Validator.ValidateCircuitBreak(targetDate, code, true) == false)
                    {
                        if (ValidateOnly == false)
                        {
                            DialogResult needRecovery = DialogResult.None;

                            lock (popupWindowLockObject)
                            {
                                if (ApplyForAll == false)
                                    needRecovery = RecoveryPopupFactory.CreateNewWindow(Path.Combine(logBasePath, Config.General.DateNow, compareResultPath, circuitbreakCompareResultPath, code + ".html"));
                                else
                                    needRecovery = DialogResult.OK;
                            }

                            if (needRecovery == DialogResult.OK)
                                Recoverer.RecoverCircuitBreak(targetDate, code);
                        }
                    }
                });
            }

            logger.Info("Circuit Break Recovery Done.");
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
                        for (DateTime targetDate = StartingDate; targetDate <= EndingDate; targetDate = targetDate.AddDays(1))
                        {
                            if (Validator.ValidateCircuitBreak(targetDate, Code, true) == false)
                            {
                                if (ValidateOnly == false)
                                {
                                    DialogResult needRecovery = DialogResult.None;

                                    lock (popupWindowLockObject)
                                    {
                                        needRecovery = RecoveryPopupFactory.CreateNewWindow(Path.Combine(logBasePath, Config.General.DateNow, compareResultPath, circuitbreakCompareResultPath, Code + ".html"));
                                    }

                                    if (needRecovery == DialogResult.OK)
                                        Recoverer.RecoverCircuitBreak(targetDate, Code);
                                }
                            }
                        }
                    }));

                return _RecoverCircuitBreakCommand;
            }
        }
    }
}
