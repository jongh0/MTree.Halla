using Newtonsoft.Json;

namespace MTree.Configuration
{
    public class DaishinConfiguration
    {
        public static readonly string FileName = "Config.Daishin.json";

        public string UserId { get; set; } = string.Empty;

        public string UserPw { get; set; } = string.Empty;

        public string CertPw { get; set; } = string.Empty;

        public string AccountPw { get; set; } = string.Empty;
    }
}
