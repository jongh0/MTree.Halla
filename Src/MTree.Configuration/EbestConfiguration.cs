using Newtonsoft.Json;

namespace MTree.Configuration
{
    public class EbestConfiguration
    {
        public string UserId { get; set; }

        public string UserPw { get; set; }

        public string CertPw { get; set; }

        public string AccountPw { get; set; }

        [JsonIgnore]
        public string ServerAddress { get; set; } = "Hts.etrade.co.kr";

        [JsonIgnore]
        public string DemoServerAddress { get; set; } = "demo.etrade.co.kr";

        [JsonIgnore]
        public int ServerPort { get; set; } = 20001;
    }
}
