using Newtonsoft.Json;

namespace MTree.Configuration
{
    public class DefaultConfiguration
    {
        [JsonIgnore]
        public string DateFormat { get; set; } = "yyyy-MM-dd";

        [JsonIgnore]
        public string TimeFormat { get; set; } = "HH:mm:ss.fff";

        [JsonIgnore]
        public string DateTimeFormat { get; set; } = "yyyy-MM-dd HH:mm:ss.fff";

        public string MongoDbConnectionString { get; set; } = "mongodb://localhost";
    }
}
