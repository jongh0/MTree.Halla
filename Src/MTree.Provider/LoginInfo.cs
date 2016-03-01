using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.Provider
{
    public enum LoginState
    {
        LoggedIn,
        LoggedOut,
        LoggingIn,
        LoggingOut,
        Disconnected,
    }

    public enum BrokerageFirm
    {
        Krx,
        Daishin,
        Etrade,
    }

    public enum BrokerageServerType
    {
        Real,
        Sham,
    }

    public class LoginInfo
    {
        public Guid GUID { get; set; }

        public LoginState LoginState { get; set; }

        public BrokerageFirm BrokerageFirm { get; set; }

        public BrokerageServerType ProviderType { get; set; }

        public string UserId { get; set; }

        public string UserPw { get; set; }

        public string CertPw { get; set; }

        public string AccountPw { get; set; }
    }
}
