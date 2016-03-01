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
    public class Config
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private static object lockObject = new object();

        #region Default Configuration
        private static readonly string defaultConfigFileName = "Config.Default.json";
        private static DefaultConfiguration _default;
        public static DefaultConfiguration Default
        {
            get
            {
                if (_default == null)
                {
                    lock (lockObject)
                    {
                        if (_default == null)
                        {
                            LoadConfiguration(ref _default, defaultConfigFileName);
                            SaveConfiguration(_default, defaultConfigFileName);
                        }
                    }
                }

                return _default;
            }
        }
        #endregion

        #region Ebest Configuration
        private static readonly string ebestConfigFileName = "Config.Ebest.json";
        private static EbestConfiguration _ebest;
        public static EbestConfiguration Ebest
        {
            get
            {
                if (_ebest == null)
                {
                    lock (lockObject)
                    {
                        if (_ebest == null)
                        {
                            LoadConfiguration(ref _ebest, ebestConfigFileName);
                            SaveConfiguration(_ebest, ebestConfigFileName);
                        }
                    }
                }

                return _ebest;
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
                    logger.Info("Configuration loaded");
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
                    logger.Info("Configuration saved");
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