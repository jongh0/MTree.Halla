using GalaSoft.MvvmLight.Command;
using Configuration;
using DbProvider;
using CommonLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;

namespace DataValidator
{
    public partial class MainViewModel : INotifyPropertyChanged
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private DataValidator_ _validator = new DataValidator_();
        private DataRecoverer _recoverer = new DataRecoverer();
        
        #region Server Config

        private string _sourceAddress;
        public string SourceAddress
        {
            get => _sourceAddress;
            set
            {
                _sourceAddress = value;
                NotifyPropertyChanged(nameof(SourceAddress));
            }
        }

        private string _destinationAddress;
        public string DestinationAddress
        {
            get => _destinationAddress;
            set
            {
                _destinationAddress = value;
                NotifyPropertyChanged(nameof(_destinationAddress));
            }
        }

        private RelayCommand _checkServerCommand;
        public ICommand CheckServerCommand => _checkServerCommand ?? (_checkServerCommand = new RelayCommand(() => Task.Run(() => CheckServer())));


        private void CheckServer()
        {
            if (DbAgent.Instance.ConnectionTest())
                _logger.Info($"Connection to {DbAgent.Instance.ConnectionString} success");
            else
                _logger.Warn($"Connection to {DbAgent.Instance.ConnectionString} fail");

            if (DbAgent.RemoteInstance.ConnectionTest())
                _logger.Info($"Connection to {DbAgent.RemoteInstance.ConnectionString} success");
            else
                _logger.Warn($"Connection to {DbAgent.RemoteInstance.ConnectionString} fail");
        }

        private RelayCommand _updateServerCommand;
        public ICommand UpdateServerCommand
        {
            get
            {
                if (_updateServerCommand == null)
                {
                    _updateServerCommand = new RelayCommand((Action)(() => Task.Run((Action)(() =>
                    {
                        DbAgent.Instance.ChangeServer(SourceAddress);
                        DbAgent.RemoteInstance.ChangeServer(DestinationAddress);

                        _validator = new DataValidator_(DbAgent.Instance, DbAgent.RemoteInstance);
                        _recoverer = new DataRecoverer(DbAgent.Instance, DbAgent.RemoteInstance);

                        CheckServer();
                    }))));
                }

                return _updateServerCommand;
            }
        }
        #endregion

        #region Validate with Daishin
        private string _codeForDaishinValidate;
        public string CodeForDaishinValidate
        {
            get => _codeForDaishinValidate;
            set
            {
                _codeForDaishinValidate = value;
                NotifyPropertyChanged(nameof(CodeForDaishinValidate));
            }
        }

        private RelayCommand _validateSourceConclusionWithDaishinCommand;
        public ICommand ValidateSourceConclusionWithDaishinCommand
        {
            get
            {
                if (_validateSourceConclusionWithDaishinCommand == null)
                {
                    _validateSourceConclusionWithDaishinCommand = new RelayCommand(() => Task.Run(() =>
                    {
                        _validator.ValidateSourceConclusionWithDaishin(CodeForDaishinValidate);
                    }));
                }

                return _validateSourceConclusionWithDaishinCommand;
            }
        }

        private RelayCommand _validateDestinationConclusionWithDaishinCommand;
        public ICommand ValidateDestinationConclusionWithDaishinCommand
        {
            get
            {
                if (_validateDestinationConclusionWithDaishinCommand == null)
                {
                    _validateDestinationConclusionWithDaishinCommand = new RelayCommand(() => Task.Run(() =>
                    {
                        _validator.ValidateDestinationConclusionWithDaishin(CodeForDaishinValidate);
                    }));
                }

                return _validateDestinationConclusionWithDaishinCommand;
            }
        } 
        #endregion
        
        public MainViewModel()
        {
            RecoveryPopupFactory = new RecoveryPopupWindowFactory(this);

            SourceAddress = Config.Database.ConnectionString;
            DestinationAddress = Config.Database.RemoteConnectionString;
            
            Task.Run(() =>
            {
                Thread.Sleep(1000);
                _logger.Info("Data Validator Started");

                if (Environment.GetCommandLineArgs().Count() > 1)
                {
                    if (Environment.GetCommandLineArgs()[1] == ProcessTypes.DataValidatorRegularCheck.ToString())
                    {
                        DoRecoverAll();
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
