using GalaSoft.MvvmLight.Command;
using MTree.Configuration;
using MTree.DbProvider;
using MTree.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MTree.DataValidator
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private DataValidator validator = new DataValidator();
        
        private ValidatorViewModel _ValidatorViewModel = new ValidatorViewModel();
        public ValidatorViewModel ValidatorViewModel
        {
            get
            {
                return _ValidatorViewModel;
            }
            set
            {
                _ValidatorViewModel = value;
                NotifyPropertyChanged(nameof(ValidatorViewModel));
            }
        }

        private RecoveryViewModel _RecoveryViewModel = new RecoveryViewModel();
        public RecoveryViewModel RecoveryViewModel
        {
            get
            {
                return _RecoveryViewModel;
            }
            set
            {
                _RecoveryViewModel = value;
                NotifyPropertyChanged(nameof(RecoveryViewModel));
            }
        }

        private string _SourceAddress;
        public string SourceAddress
        {
            get
            {
                return _SourceAddress;
            }
            set
            {
                _SourceAddress = value;
                NotifyPropertyChanged(nameof(SourceAddress));
            }
        }

        private string _DestinationAddress;
        public string DestinationAddress
        {
            get
            {
                return _DestinationAddress;
            }
            set
            {
                _DestinationAddress = value;
                NotifyPropertyChanged(nameof(_DestinationAddress));
            }
        }

        private RelayCommand _UpdateServerCommand;
        public ICommand UpdateServerCommand
        {
            get
            {
                if (_UpdateServerCommand == null)
                    _UpdateServerCommand = new RelayCommand(() => Task.Run(() =>
                    {
                        DbAgent.Instance.ChangeServer(SourceAddress);
                        DbAgent.RemoteInstance.ChangeServer(DestinationAddress);
                        validator = new DataValidator(new DbCollector(DbAgent.Instance), new DbCollector(DbAgent.RemoteInstance));
                    }));

                return _UpdateServerCommand;
            }
        }

        public MainViewModel()
        {
            SourceAddress = Config.Database.ConnectionString;
            DestinationAddress = Config.Database.RemoteConnectionString;
            
            ValidatorViewModel.Validator = validator;
            RecoveryViewModel.Validator = validator;
            
            Task.Run(() =>
            {
                Thread.Sleep(1000);
                logger.Info("Data Validator Started");
                
                if (Environment.GetCommandLineArgs().Count() > 1)
                {
                    if (Environment.GetCommandLineArgs()[1] == ProcessTypes.DataValidatorRegularCheck.ToString())
                    {
                        ValidatorViewModel.ValidateAllExecute();
                        Environment.Exit(0);
                    }
                }
            });
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
