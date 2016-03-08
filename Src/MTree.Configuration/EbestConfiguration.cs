using Newtonsoft.Json;

namespace MTree.Configuration
{
    public class EbestConfiguration
    {
        public static readonly string FileName = "Config.Ebest.json";

        public string UserId { get; set; }

        public string UserPw { get; set; }

        public string CertPw { get; set; }

        public string AccountPw { get; set; }

        [JsonIgnore]
        public string RealServerAddress { get; } = "Hts.etrade.co.kr";

        [JsonIgnore]
        public string DemoServerAddress { get; } = "demo.etrade.co.kr";

        [JsonIgnore]
        public int ServerPort { get; } = 20001;
    }
}
