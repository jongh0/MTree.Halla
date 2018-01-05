using Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib
{
    public class LoginInfo : INotifyPropertyChanged
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        private LoginStates _State = LoginStates.Disconnect;
        public LoginStates State
        {
            get { return _State; }
            set
            {
                _State = value;
                NotifyPropertyChanged(nameof(State));
            }
        }

        public FirmTypes FirmType { get; set; } = FirmTypes.Ebest;

        private ServerTypes _ServerType = ServerTypes.Real;
        public ServerTypes ServerType
        {
            get { return _ServerType; }
            set
            {
                _ServerType = value;
                NotifyPropertyChanged(nameof(ServerType));
            }
        }

        private string _UserId;
        public string UserId
        {
            get { return _UserId; }
            set
            {
                _UserId = value;
                NotifyPropertyChanged(nameof(UserId));
            }
        }

        public string UserPw { get; set; }
        public string CertPw { get; set; }
        public string AccountPw { get; set; }

        public string ServerAddress { get; set; }
        public int ServerPort { get; set; }

        public override string ToString()
        {
            return $"{State}/{FirmType}/{ServerAddress}/{ServerPort}/{UserId}/{Id}";
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
