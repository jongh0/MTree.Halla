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

        #region General Configuration
        private static GeneralConfiguration _general;
        public static GeneralConfiguration General
        {
            get
            {
                if (_general == null)
                {
                    lock (lockObject)
                    {
                        _general = new GeneralConfiguration();
                    }
                }

                return _general;
            }
        }
        #endregion

        #region Ebest Configuration
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
                            LoadConfiguration(ref _ebest, EbestConfiguration.FileName);
                            SaveConfiguration(_ebest, EbestConfiguration.FileName);
                        }
                    }
                }

                return _ebest;
            }
        }
        #endregion

        #region Daishin Configuration
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
                            LoadConfiguration(ref _daishin, DaishinConfiguration.FileName);
                            SaveConfiguration(_daishin, DaishinConfiguration.FileName);
                        }
                    }
                }

                return _daishin;
            }
        }
        #endregion

        #region Database Configuration
        private static DatabaseConfiguration _database;
        public static DatabaseConfiguration Database
        {
            get
            {
                if (_database == null)
                {
                    lock (lockObject)
                    {
                        if (_database == null)
                        {
                            LoadConfiguration(ref _database, DatabaseConfiguration.FileName);
                            SaveConfiguration(_database, DatabaseConfiguration.FileName);
                        }
                    }
                }

                return _database;
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