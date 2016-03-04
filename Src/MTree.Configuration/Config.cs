using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;

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

        #region Daishin Configuration
        private static readonly string daishinConfigFileName = "Config.Daishin.json";
        private static DaishinConfiguration _daishin;
        public static DaishinConfiguration Daishin
        {
            get
            {
                if (_daishin == null)
                {
                    lock (lockObject)
                    {
                        if (_daishin == null)
                        {
                            LoadConfiguration(ref _daishin, daishinConfigFileName);
                            SaveConfiguration(_daishin, daishinConfigFileName);
                        }
                    }
                }

                return _daishin;
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
                    logger.Info($"{Path.GetFileName(filePath)} configuration loaded");
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
                    logger.Info($"{Path.GetFileName(filePath)} configuration saved");
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