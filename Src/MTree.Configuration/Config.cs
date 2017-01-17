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
        public DatabaseConfiguration database { get; set; } = new DatabaseConfiguration();
        public DaishinConfiguration daishin { get; set; } = new DaishinConfiguration();
        public EbestConfiguration ebest { get; set; } = new EbestConfiguration();
        public KiwoomConfiguration kiwoom { get; set; } = new KiwoomConfiguration();
        public EmailConfiguration email { get; set; } = new EmailConfiguration();
        public ValidatorConfiguration validator { get; set; } = new ValidatorConfiguration();
        public ResourceMonitorConfiguration resourceMonitor { get; set; } = new ResourceMonitorConfiguration();

        public static GeneralConfiguration General { get { return Instance.general; } }
        public static DatabaseConfiguration Database { get { return Instance.database; } }
        public static DaishinConfiguration Daishin { get { return Instance.daishin; } }
        public static EbestConfiguration Ebest { get { return Instance.ebest; } }
        public static KiwoomConfiguration Kiwoom { get { return Instance.kiwoom; } }
        public static EmailConfiguration Email { get { return Instance.email; } }
        public static ValidatorConfiguration Validator { get { return Instance.validator; } }
        public static ResourceMonitorConfiguration ResourceMonitor { get { return Instance.resourceMonitor; } }

        #region Instance
        private static object lockObject = new object();

        private static Config _Instance;
        private static Config Instance
        {
            get
            {
                if (_Instance == null)
                {
                    lock (lockObject)
                    {
                        if (_Instance == null)
                            LoadConfiguration(ref _Instance, FileName);
                    }
                }

                return _Instance;
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
                using (var stream = File.Open(filePath, FileMode.Create, FileAccess.Write))
                using (var writer = new StreamWriter(stream))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.NullValueHandling = NullValueHandling.Ignore;
                    serializer.Formatting = Formatting.Indented;
                    serializer.Serialize(writer, config);
                    writer.Flush();
                    stream.Flush(true);

                    logger.Info($"{Path.GetFileName(filePath)} saved");
                }

                //using (StreamWriter stream = File.CreateText(filePath))
                //{
                //    JsonSerializer serializer = new JsonSerializer();
                //    serializer.NullValueHandling = NullValueHandling.Ignore;
                //    serializer.Formatting = Formatting.Indented;
                //    serializer.Serialize(stream, config);
                //    logger.Info($"{Path.GetFileName(filePath)} saved");
                //}
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
        #endregion
    }
}