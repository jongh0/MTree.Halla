using MTree.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.Publisher
{
    public enum LoginStates
    {
        Disconnect,
        Login,
        Logout,
    }

    public enum FirmTypes
    {
        Daishin,
        Ebest,
        Kiwoom,
    }

    public class LoginInfo
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public LoginStates State { get; set; } = LoginStates.Disconnect;
        public FirmTypes FirmType { get; set; } = FirmTypes.Ebest;
        public ServerTypes ServerType { get; set; } = ServerTypes.Real;

        public string UserId { get; set; }
        public string UserPw { get; set; }
        public string CertPw { get; set; }
        public string AccountPw { get; set; }

        public string ServerAddress { get; set; }
        public int ServerPort { get; set; }

        public override string ToString()
        {
            return $"{State}/{FirmType}/{ServerAddress}/{ServerPort}/{UserId}/{Id}";
        }
    }
}
