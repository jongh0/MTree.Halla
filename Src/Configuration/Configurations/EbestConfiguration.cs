using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Configuration
{
    public class EbestConfiguration
    {
        public string UserId { get; set; } = string.Empty;

        public string UserPw { get; set; } = string.Empty;

        public string CertPw { get; set; } = string.Empty;

        public string AccountPw { get; set; } = string.Empty;

        [JsonIgnore]
        public string RealServerAddress => "Hts.etrade.co.kr";

        [JsonIgnore]
        public string SimulServerAddress => "demo.etrade.co.kr";

        [JsonIgnore]
        public int ServerPort => 20001;
    }
}
