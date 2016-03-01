using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.Configuration
{
    public class EbestConfiguration
    {
        public string UserId { get; set; }

        public string UserPw { get; set; }

        public string CertPw { get; set; }

        public string AccountPw { get; set; }

        public string ServerAddress { get; set; } = "Hts.etrade.co.kr";

        public string DemoServerAddress { get; set; } = "demo.etrade.co.kr";

        public int ServerPort { get; set; } = 20001;
    }
}
