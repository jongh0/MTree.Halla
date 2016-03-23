using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MTree.Configuration
{
    public enum ServerTypes
    {
        Real,
        Simul,
    }

    public class EbestConfiguration
    {
        public string UserId { get; set; } = string.Empty;

        public string UserPw { get; set; } = string.Empty;

        public string CertPw { get; set; } = string.Empty;

        public string AccountPw { get; set; } = string.Empty;

        [JsonConverter(typeof(StringEnumConverter))]
        public ServerTypes ServerType { get; set; } = ServerTypes.Real;

        [JsonIgnore]
        public string ServerAddress
        {
            get
            {
                if (ServerType == ServerTypes.Real)
                    return "Hts.etrade.co.kr";
                else
                    return "demo.etrade.co.kr";
            }
        }

        [JsonIgnore]
        public int ServerPort { get; } = 20001;
    }
}
