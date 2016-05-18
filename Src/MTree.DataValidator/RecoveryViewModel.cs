using GalaSoft.MvvmLight.Command;
using MTree.Configuration;
using MTree.DbProvider;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MTree.DataValidator
{
    public class RecoveryViewModel: INotifyPropertyChanged
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

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
                    if (Validator.ValidateStockConclusion(targetDate, code, false) == false)
                    {
                        Recoverer.RecoverStockConclusion(targetDate, code, AskBeforeRecovery);
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
                            if (ValidateBeforeRecovery == true)
                            {
                                if (Validator.ValidateStockConclusion(targetDate, Code, false) == true)
                                {
                                    logger.Info($"Master of {Code} is same. Do nothing");
                                    return;
                                }
                            }

                            if (Recoverer != null)
                            {
                                Recoverer.RecoverStockConclusion(targetDate, Code, AskBeforeRecovery);
                            }
                            else
                            {
                                logger.Error("Validator is not assigned");
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
