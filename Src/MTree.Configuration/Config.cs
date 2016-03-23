using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;

namespace MTree.Configuration
{
    public class Config
    {
        public GeneralConfiguration General { get; set; } = new GeneralConfiguration();
        public PushConfiguration Push { get; set; } = new PushConfiguration();
        public DatabaseConfiguration Database { get; set; } = new DatabaseConfiguration();
        public DaishinConfiguration Daishin { get; set; } = new DaishinConfiguration();
        public EbestConfiguration Ebest { get; set; } = new EbestConfiguration();
        public EmailConfiguration Email { get; set; } = new EmailConfiguration();


        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static readonly string FileName = "Config.MTree.json";

        private static object lockObject = new object();

        #region Instance
        private static Config _instance;
        public static Config Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockObject)
                    {
                        if (_instance == null)
                            LoadConfiguration(ref _instance, FileName);
                    }
                }

                return _instance;
            }
        } 
        #endregion

        public static void Initialize()
        {
            SaveConfiguration(Instance, FileName);
        }

        #region Load / Save
        private static void LoadConfiguration<T>(ref T config, string filePath)
        {
            bool fileExist = File.Exists(filePath);

            try
            {
                if (fileExist == true)
                {
                    config = JsonConvert.DeserializeObject<T>(File.ReadAllText(filePath), new JsonSerializerSettings
                    {
                        Error = (sender, args) =>
                        {
                            logger.Error($"Configuration deserialize error, {args.ErrorContext.Error.Message}");
                            args.ErrorContext.Handled = true;
                        }
                    });

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
                logger.Error(ex);
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
                logger.Error(ex);
            }
        }
        #endregion
    }
}