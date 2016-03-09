using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.Utility
{
    public class ProcessUtility
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static void Start(string filePath, string arguments = "", bool waitIdle = false)
        {
            try
            {
                var process = new Process();
                process.StartInfo.FileName = filePath;
                process.StartInfo.Arguments = arguments;
                process.Start();

                if (waitIdle == true)
                    process.WaitForInputIdle();

                logger.Info($"{Path.GetFileName(filePath)} {arguments} process started");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public static void Kill(string processName, int excludeId = -1)
        {
            var processList = Process.GetProcessesByName(processName);
            if (processList == null)
                return;

            foreach (var p in processList)
            {
                try
                {
                    if (p.Id != excludeId)
                    {
                        p.Kill();
                        logger.Info($"{p.ProcessName}, {p.Id} process killed");
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }
            }
        }
    }
}
