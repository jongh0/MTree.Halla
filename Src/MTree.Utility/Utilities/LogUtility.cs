using MTree.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTree.Utility
{
    public class LogUtility
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static void SendLog()
        {
            try
            {
                logger.Info("Send log");

                var date = DateTime.Now.ToString(Config.General.DateFormat);
                var logFolder = Path.Combine(Environment.CurrentDirectory, "Logs");
                var targetFile = $"MTree.Log.{date}.zip";
                var targetPath = Path.Combine(logFolder, targetFile);

                int retry = 3;
                while (retry-- > 0)
                {
                    try
                    {
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

                        break; // 여기까지 못 오고 Exception 발생하면 1초 간격으로 3회 재시도
                    }
                    catch (Exception)
                    {
                        Thread.Sleep(1000);
                    }
                }

                logger.Info($"{targetFile} file created");

                // 압축한 로그 파일을 Email로 전송
                EmailUtility.SendEmail($"[{Environment.MachineName}] MTree log {date}", "Log attached", targetPath);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
    }
}
