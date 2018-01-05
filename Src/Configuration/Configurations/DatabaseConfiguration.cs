using Newtonsoft.Json;
using System;

namespace Configuration
{
    public class DatabaseConfiguration
    {
        [JsonIgnore]
        public string TodayCollectionName { get { return DateTime.Now.ToString("yyyyMMdd"); } }

        public string ConnectionString { get; set; } = "mongodb://localhost";

        public string RemoteConnectionString { get; set; } = "mongodb://localhost";
    }
}
