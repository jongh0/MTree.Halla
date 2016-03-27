using MTree.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.Utility
{
    public class LogUtility
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static void SendLogToEmail()
        {
            try
            {
                logger.Info("Send log to email");

                var date = DateTime.Now.ToString(Config.General.DateFormat);
                var logFolder = Path.Combine(Environment.CurrentDirectory, "Logs");
                var targetFile = $"MTree.Log.{date}.zip";
                var targetPath = Path.Combine(logFolder, targetFile);

                if (File.Exists(targetPath) == true)
                    File.Delete(targetPath);

                using (var zip = ZipFile.Open(targetPath, ZipArchiveMode.Create))
                {
                    foreach (var file in Directory.GetFiles(logFolder, "*.log", SearchOption.TopDirectoryOnly))
                    {
                        if (file.IndexOf(date) != -1)
                            zip.CreateEntryFromFile(file, Path.GetFileName(file));
                    }
                }

                logger.Info($"{targetFile} file created");

                EmailUtility.SendEmail($"[{Environment.MachineName}] MTree log {date}", "Log attached", targetPath);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
    }
}
