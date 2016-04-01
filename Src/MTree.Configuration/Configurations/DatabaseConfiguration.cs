using Newtonsoft.Json;
using System;

namespace MTree.Configuration
{
    public class DatabaseConfiguration
    {
        public string ConnectionString { get; set; } = "mongodb://localhost";

        public string RemoteConnectionString { get; set; } = "mongodb://localhost";
    }
}
