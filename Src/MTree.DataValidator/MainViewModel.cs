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
using System.Windows.Forms;
using System.Windows.Input;

namespace MTree.DataValidator
{
    public partial class MainViewModel : INotifyPropertyChanged
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private DataValidator _validator = new DataValidator();
        private DataRecoverer _recoverer = new DataRecoverer();
        
        #region Server Config

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

        private RelayCommand _CheckServerCommand;
        public ICommand CheckServerCommand
        {
            get
            {
                if (_CheckServerCommand == null)
                    _CheckServerCommand = new RelayCommand(() => Task.Run(() => CheckServer()));

                return _CheckServerCommand;
            }
        }
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

        private RelayCommand _UpdateServerCommand;
        public ICommand UpdateServerCommand
        {
            get
            {
                if (_UpdateServerCommand == null)
                    _UpdateServerCommand = new RelayCommand((Action)(() => Task.Run((Action)(() =>
                    {
                        DbAgent.Instance.ChangeServer(SourceAddress);
                        DbAgent.RemoteInstance.ChangeServer(DestinationAddress);

                        _validator = new DataValidator(DbAgent.Instance, DbAgent.RemoteInstance);
                        _recoverer = new DataRecoverer(DbAgent.Instance, DbAgent.RemoteInstance);

                        CheckServer();
                    }))));

                return _UpdateServerCommand;
            }
        }
        #endregion

        #region Validate with Daishin
        private string _CodeForDaishinValidate;
        public string CodeForDaishinValidate
        {
            get
            {
                return _CodeForDaishinValidate;
            }
            set
            {
                _CodeForDaishinValidate = value;
                NotifyPropertyChanged(nameof(CodeForDaishinValidate));
            }
        }

        private RelayCommand _ValidateSourceConclusionWithDaishinCommand;
        public ICommand ValidateSourceConclusionWithDaishinCommand
        {
            get
            {
                if (_ValidateSourceConclusionWithDaishinCommand == null)
                    _ValidateSourceConclusionWithDaishinCommand = new RelayCommand(() =>
                    Task.Run(() =>
                    {
                        _validator.ValidateSourceConclusionWithDaishin(CodeForDaishinValidate);
                    }));

                return _ValidateSourceConclusionWithDaishinCommand;
            }
        }

        private RelayCommand _ValidateDestinationConclusionWithDaishinCommand;
        public ICommand ValidateDestinationConclusionWithDaishinCommand
        {
            get
            {
                if (_ValidateDestinationConclusionWithDaishinCommand == null)
                    _ValidateDestinationConclusionWithDaishinCommand = new RelayCommand(() =>
                    Task.Run(() =>
                    {
                        _validator.ValidateDestinationConclusionWithDaishin(CodeForDaishinValidate);
                    }));

                return _ValidateDestinationConclusionWithDaishinCommand;
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
