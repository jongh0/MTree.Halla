using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration
{
    public class KiwoomConfiguration
    {
        public string UserId { get; set; } = string.Empty;

        public string UserPw { get; set; } = string.Empty;

        public string CertPw { get; set; } = string.Empty;

        public string AccountPw { get; set; } = string.Empty;

        public bool UseSessionManager { get; set; } = false;
    }
}
