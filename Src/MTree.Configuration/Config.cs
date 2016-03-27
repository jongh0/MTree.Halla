using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;

namespace MTree.Configuration
{
    public class Config
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public GeneralConfiguration general { get; set; } = new GeneralConfiguration();
        public PushConfiguration push { get; set; } = new PushConfiguration();
        public DatabaseConfiguration database { get; set; } = new DatabaseConfiguration();
        public DaishinConfiguration daishin { get; set; } = new DaishinConfiguration();
        public EbestConfiguration ebest { get; set; } = new EbestConfiguration();
        public EmailConfiguration email { get; set; } = new EmailConfiguration();

        public static GeneralConfiguration General { get { return Instance.general; } }
        public static PushConfiguration Push { get { return Instance.push; } }
        public static DatabaseConfiguration Database { get { return Instance.database; } }
        public static DaishinConfiguration Daishin { get { return Instance.daishin; } }
        public static EbestConfiguration Ebest { get { return Instance.ebest; } }
        public static EmailConfiguration Email { get { return Instance.email; } }

        #region Instance
        private static object lockObject = new object();

        private static Config _instance;
        private static Config Instance
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
        private static readonly string FileName = "Config.MTree.json";

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