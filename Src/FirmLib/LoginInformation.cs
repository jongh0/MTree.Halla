using CommonLib;
using CommonLib.Utility;
using Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirmLib
{
    public class LoginInformation : INotifyPropertyChanged
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        private LoginStatus _state = LoginStatus.Disconnect;
        public LoginStatus Status
        {
            get { return _state; }
            set
            {
                _state = value;
                NotifyPropertyChanged(nameof(Status));
            }
        }

        public FirmTypes FirmType { get; set; } = FirmTypes.Ebest;

        private ServerTypes _serverType = ServerTypes.Real;
        public ServerTypes ServerType
        {
            get { return _serverType; }
            set
            {
                _serverType = value;
                NotifyPropertyChanged(nameof(ServerType));
            }
        }

        private string _userId;
        public string UserId
        {
            get { return _userId; }
            set
            {
                _userId = value;
                NotifyPropertyChanged(nameof(UserId));
            }
        }

        public string UserPassword { get; set; }
        public string CertPassword { get; set; }
        public string AccountPassword { get; set; }

        public string ServerAddress { get; set; }
        public int ServerPort { get; set; }

        public bool IsValid => string.IsNullOrEmpty(UserId) == false &&
                               string.IsNullOrEmpty(UserPassword) == false &&
                               string.IsNullOrEmpty(CertPassword) == false;

        public override string ToString()
        {
            return PropertyUtility.PrintNameValues(this);
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
