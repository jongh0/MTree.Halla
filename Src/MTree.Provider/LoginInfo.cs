using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.Provider
{
    public enum LoginStateType
    {
        Disconnected,
        LoggedIn,
        LoggedOut,
    }

    public enum BrokerageFirmType
    {
        Krx,
        Daishin,
        Etrade,
    }

    public enum BrokerageServerType
    {
        Real,
        Simul,
    }

    public class LoginInfo
    {
        public Guid GUID { get; set; }

        public LoginStateType LoginState { get; set; }

        public BrokerageFirmType BrokerageFirm { get; set; }

        public BrokerageServerType ProviderType { get; set; }

        public string UserId { get; set; }

        public string UserPw { get; set; }

        public string CertPw { get; set; }

        public string AccountPw { get; set; }
    }
}
