using GalaSoft.MvvmLight.Command;
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

        private DateTime _TargetDate = DateTime.Now;
        public DateTime TargetDate
        {
            get
            {
                return _TargetDate;
            }
            set
            {
                _TargetDate = value;
                NotifyPropertyChanged(nameof(TargetDate));
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

        private RelayCommand _RecoverMasterAllCommand;
        public ICommand RecoverMasterAllCommand
        {
            get
            {
                if (_RecoverMasterAllCommand == null)
                    _RecoverMasterAllCommand = new RelayCommand(() =>
                    Task.Run(() =>
                    {
                        if (ValidateBeforeRecovery == true)
                        {
                            if (Validator.ValidateMasters(TargetDate, false) == true)
                            {
                                logger.Info($"Master of {Code} is same. Do nothing");
                                return;
                            }
                        }
                        if (Recoverer != null)
                        {
                            Recoverer.RecoverMasters(TargetDate, AskBeforeRecovery);
                        }
                        else
                        {
                            logger.Error("Validator is not assigned");
                        }
                    }));

                return _RecoverMasterAllCommand;
            }
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
                        if (ValidateBeforeRecovery == true)
                        {
                            if (Validator.ValidateMaster(TargetDate, Code, false) == true)
                            {
                                logger.Info($"Master of {Code} is same. Do nothing");
                                return;
                            }
                        }

                        if (Recoverer != null)
                        {
                            Recoverer.RecoverMaster(TargetDate, Code, AskBeforeRecovery);
                        }
                        else
                        {
                            logger.Error("Validator is not assigned");
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
                        if (ValidateBeforeRecovery == true)
                        {
                            if (Validator.ValidateStockConclusions(TargetDate, false) == true)
                            {
                                logger.Info($"Master of {Code} is same. Do nothing");
                                return;
                            }
                        }
                        if (Recoverer != null)
                        {
                            Recoverer.RecoverStockConclusions(TargetDate, AskBeforeRecovery);
                        }
                        else
                        {
                            logger.Error("Validator is not assigned");
                        }
                    }));

                return _RecoverStockConclusionAllCommand;
            }
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
                        if (ValidateBeforeRecovery == true)
                        {
                            if (Validator.ValidateStockConclusion(TargetDate, Code, false) == true)
                            {
                                logger.Info($"Master of {Code} is same. Do nothing");
                                return;
                            }
                        }

                        if (Recoverer != null)
                        {
                            Recoverer.RecoverStockConclusion(TargetDate, Code, AskBeforeRecovery);
                        }
                        else
                        {
                            logger.Error("Validator is not assigned");
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
                        if (ValidateBeforeRecovery == true)
                        {
                            if (Validator.ValidateIndexConclusion(TargetDate, false) == true)
                            {
                                logger.Info($"Index Conclusion of {Code} is same. Do nothing");
                                return;
                            }
                        }
                        if (Recoverer != null)
                        {
                            Recoverer.RecoverIndexConclusions(TargetDate, AskBeforeRecovery);
                        }
                        else
                        {
                            logger.Error("Validator is not assigned");
                        }
                    }));

                return _RecoverIndexConclusionAllCommand;
            }
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
                        if (ValidateBeforeRecovery == true)
                        {
                            if (Validator.ValidateIndexConclusion(TargetDate, Code, false) == true)
                            {
                                logger.Info($"Index Conclusion of {Code} is same. Do nothing");
                                return;
                            }
                        }

                        if (Recoverer != null)
                        {
                            Recoverer.RecoverIndexConclusion(TargetDate, Code, AskBeforeRecovery);
                        }
                        else
                        {
                            logger.Error("Validator is not assigned");
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
                        if (ValidateBeforeRecovery == true)
                        {
                            if (Validator.ValidateCircuitBreak(TargetDate, false) == true)
                            {
                                logger.Info($"CircuitBreak is same. Do nothing");
                                return;
                            }
                        }
                        if (Recoverer != null)
                        {
                            Recoverer.RecoverCircuitBreaks(TargetDate, AskBeforeRecovery);
                        }
                        else
                        {
                            logger.Error("Validator is not assigned");
                        }
                    }));

                return _RecoverCircuitBreakCommand;
            }
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
