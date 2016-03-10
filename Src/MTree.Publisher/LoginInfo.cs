using MTree.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.Publisher
{
    public enum StateType
    {
        Disconnect,
        Login,
        Logout,
    }

    public enum FirmType
    {
        Daishin,
        Ebest,
        Kiwoom,
    }

    public class LoginInfo
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public StateType State { get; set; } = StateType.Disconnect;
        public FirmType Firm { get; set; } = FirmType.Ebest;
        public ServerType Server { get; set; } = ServerType.Real;

        public string UserId { get; set; }
        public string UserPw { get; set; }
        public string CertPw { get; set; }
        public string AccountPw { get; set; }

        public string ServerAddress { get; set; }
        public int ServerPort { get; set; }

        public override string ToString()
        {
            return $"{State}, {Firm}, {Server}, {Id}";
        }
    }
}
