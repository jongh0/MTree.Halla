using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.Configuration
{
    public class DefaultConfiguration
    {
        public string DateFormat { get; set; } = "yyyy-MM-dd";

        public string TimeFormat { get; set; } = "HH:mm:ss.fff";

        public string DateTimeFormat { get; set; } = "yyyy-MM-dd HH:mm:ss.fff";

        public string MongoDbConnectionString { get; set; } = "mongodb://localhost";
    }
}
