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

        private bool _ShowDifferent = true;
        public bool ShowDifferent
        {
            get
            {
                return _ShowDifferent;
            }
            set
            {
                _ShowDifferent = value;
                NotifyPropertyChanged(nameof(ShowDifferent));
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

        private RelayCommand _RecoverMasterCommand;
        public ICommand RecoverMasterCommand
        {
            get
            {
                if (_RecoverMasterCommand == null)
                    _RecoverMasterCommand = new RelayCommand(() =>
                    Task.Run(() =>
                    {
                        if (Validator.ValidateMaster(TargetDate, Code, false) == true)
                        {
                            logger.Info($"Master of {Code} is same");
                            return;
                        }

                        if (Recoverer != null)
                        {
                            Recoverer.RecoverMaster(TargetDate, Code);
                        }
                        else
                        {
                            logger.Error("Validator is not assigned");
                        }
                    }));

                return _RecoverMasterCommand;
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
