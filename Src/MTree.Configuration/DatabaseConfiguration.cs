using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
