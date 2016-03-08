using Newtonsoft.Json;
using System;

namespace MTree.Configuration
{
    public class DatabaseConfiguration
    {
        public static readonly string FileName = "Config.Database.json";

        [JsonIgnore]
        public string TodayCollectionName { get { return DateTime.Now.ToString("yyyyMMdd"); } }

        public string ConnectionString { get; set; } = "mongodb://localhost";
    }
}
