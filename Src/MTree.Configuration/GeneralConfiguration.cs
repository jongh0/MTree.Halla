using Newtonsoft.Json;

namespace MTree.Configuration
{
    public class GeneralConfiguration
    {
        public static readonly string FileName = "Config.General.json";

        [JsonIgnore]
        public string DateFormat { get; } = "yyyy-MM-dd";

        [JsonIgnore]
        public string TimeFormat { get; } = "HH:mm:ss.fff";

        [JsonIgnore]
        public string DateTimeFormat { get; } = "yyyy-MM-dd HH:mm:ss.fff";

        public bool OfflineMode { get; } = false;

        public bool TestMode { get; } = false;
    }
}
