using Newtonsoft.Json;

namespace MTree.Configuration
{
    public class DaishinConfiguration
    {
        public static readonly string FileName = "Config.Daishin.json";

        public string UserId { get; set; }

        public string UserPw { get; set; }

        public string CertPw { get; set; }

        public string AccountPw { get; set; }
    }
}
