﻿using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;

namespace MTree.Configuration
{
    public class Config
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private static object lockObject = new object();

        #region General
        private static GeneralConfiguration _general;
        public static GeneralConfiguration General
        {
            get
            {
                if (_general == null)
                {
                    lock (lockObject)
                    {
                        if (_general == null)
                            LoadConfiguration(ref _general, GeneralConfiguration.FileName);
                    }
                }

                return _general;
            }
        }
        #endregion

        #region Ebest
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
                            LoadConfiguration(ref _ebest, EbestConfiguration.FileName);
                    }
                }

                return _ebest;
            }
        }
        #endregion

        #region Daishin
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
                            LoadConfiguration(ref _daishin, DaishinConfiguration.FileName);
                    }
                }

                return _daishin;
            }
        }
        #endregion

        #region Database
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
                            LoadConfiguration(ref _database, DatabaseConfiguration.FileName);
                    }
                }

                return _database;
            }
        }
        #endregion

        #region Push
        private static PushConfiguration _push;
        public static PushConfiguration Push
        {
            get
            {
                if (_push == null)
                {
                    lock (lockObject)
                    {
                        if (_push == null)
                            LoadConfiguration(ref _push, PushConfiguration.FileName);
                    }
                }

                return _push;
            }
        }
        #endregion

        #region Load / Save
        private static void LoadConfiguration<T>(ref T config, string filePath)
        {
            bool fileExist = File.Exists(filePath);

            try
            {
                if (fileExist == true)
                {
                    config = JsonConvert.DeserializeObject<T>(File.ReadAllText(filePath));
                    logger.Info($"{Path.GetFileName(filePath)} loaded");
                }
                else
                {
                    if (config == null)
                        config = Activator.CreateInstance<T>();

                    SaveConfiguration(config, filePath);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
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
                    logger.Info($"{Path.GetFileName(filePath)} saved");
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