using Newtonsoft.Json;
using System;

namespace MTree.Configuration
{
    public class DatabaseConfiguration
    {
        [JsonIgnore]
        public string TodayCollectionName { get { return DateTime.Now.ToString("yyyyMMdd"); } }

        public string ConnectionString { get; set; } = "mongodb://localhost";
    }
}
