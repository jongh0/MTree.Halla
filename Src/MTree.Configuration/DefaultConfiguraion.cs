using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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

        public string DbSource { get; set; } = "localhost";



        #region Configuration Instance
        private static object lockObject = new object();
        private static readonly string configFileName = "MTree.Configuration.json";

        private static DefaultConfiguration _instance;
        public static DefaultConfiguration Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockObject)
                    {
                        if (_instance == null)
                        {
                            LoadConfiguration(ref _instance, configFileName);
                            SaveConfiguration(_instance, configFileName);
                        }
                    }
                }

                return _instance;
            }
        } 
        #endregion

        #region Load / Save Configuration
        private static void LoadConfiguration<T>(ref T config, string filePath)
        {
            try
            {
                if (File.Exists(filePath) == true)
                {
                    config = JsonConvert.DeserializeObject<T>(File.ReadAllText(filePath));
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }

            if (config == null)
                config = Activator.CreateInstance<T>();
        }

        private static void SaveConfiguration<T>(T config, string filePath)
        {
            try
            {
                using (StreamWriter stream = File.CreateText(filePath))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.NullValueHandling = NullValueHandling.Ignore;
                    serializer.Formatting = Formatting.Indented;
                    serializer.Serialize(stream, config);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }
        #endregion
    }
}
