using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MTree.Configuration
{
    public enum ServerType
    {
        Real,
        Simul,
    }

    public class EbestConfiguration
    {
        public static readonly string FileName = "Config.Ebest.json";

        public string UserId { get; set; } = string.Empty;

        public string UserPw { get; set; } = string.Empty;

        public string CertPw { get; set; } = string.Empty;

        public string AccountPw { get; set; } = string.Empty;

        [JsonConverter(typeof(StringEnumConverter))]
        public ServerType Server { get; set; } = ServerType.Real;

        [JsonIgnore]
        public string ServerAddress
        {
            get
            {
                if (Server == ServerType.Real)
                    return "Hts.etrade.co.kr";
                else
                    return "demo.etrade.co.kr";
            }
        }

        [JsonIgnore]
        public int ServerPort { get; } = 20001;
    }
}
