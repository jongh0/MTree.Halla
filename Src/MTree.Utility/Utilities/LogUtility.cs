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
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public static void SendLog()
        {
            try
            {
                _logger.Info("Send log");

                var date = Config.General.DateNow;
                var logFolder = Path.Combine(Environment.CurrentDirectory, "Logs", date);
                var targetFileName = $"MTree.Log.{date}.zip";
                var targetFilePath = Path.Combine(Environment.CurrentDirectory, "Logs", targetFileName);

                int retry = 3;
                while (retry-- > 0)
                {
                    try
                    {
                        if (File.Exists(targetFilePath) == true)
                            File.Delete(targetFilePath);

                        using (var stream = File.Create(targetFilePath))
                        using (var zip = new Ionic.Zip.ZipFile())
                        {
                            zip.AddDirectory(logFolder);
                            zip.Save(stream);
                            stream.Flush(true);
                        }

                        break; // 여기까지 못 오고 Exception 발생하면 1초 간격으로 3회 재시도
                    }
                    catch (Exception)
                    {
                        Thread.Sleep(1000);
                    }
                }

                _logger.Info($"{targetFileName} file created");

                // 압축한 로그 파일을 Email로 전송
                EmailUtility.SendEmail($"[{Environment.MachineName}] MTree log {date}", "Log attached", targetFilePath);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        public static void DeleteOldLog()
        {
            try
            {
                var logDir = Path.Combine(Environment.CurrentDirectory, "Logs");

                if (Config.General.LogFolderKeepDays > 0)
                {
                    var dirs = Directory.GetDirectories(logDir, "*", SearchOption.TopDirectoryOnly);
                    foreach (var dir in dirs)
                    {
                        var strs = Path.GetFileName(dir).Split('-');
                        if (strs.Length == 3)
                        {
                            var logDate = new DateTime(int.Parse(strs[0]), int.Parse(strs[1]), int.Parse(strs[2]));
                            if ((DateTime.Now - logDate).TotalDays > Config.General.LogFolderKeepDays)
                                Directory.Delete(dir, true);
                        }
                    }
                }

                if (Config.General.LogZipFileKeepDays > 0)
                {
                    var files = Directory.GetFiles(logDir, "*.zip", SearchOption.TopDirectoryOnly);
                    foreach (var file in files)
                    {
                        var date = Path.GetFileNameWithoutExtension(file).Replace("MTree.Log.", "");
                        var strs = date.Split('-');
                        if (strs.Length == 3)
                        {
                            var logDate = new DateTime(int.Parse(strs[0]), int.Parse(strs[1]), int.Parse(strs[2]));
                            if ((DateTime.Now - logDate).TotalDays > Config.General.LogZipFileKeepDays)
                                File.Delete(file);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }
    }
}
