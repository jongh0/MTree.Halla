using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.Utility
{
    public enum ProcessType
    {
        None,
        Daishin,
        DaishinMaster,
        DaishinPopupStopper,
        Kiwoon,
        Ebest,
        Krx,
        Naver,
        HistorySaver,
    }

    public class ProcessUtility
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static void Start(ProcessType type, bool waitIdle = false)
        {
            try
            {
                var windowStyle = ProcessWindowStyle.Minimized;

                switch (type)
                {
                    case ProcessType.Daishin:
                    case ProcessType.DaishinMaster:
                        Start("MTree.DaishinPublisher.exe", type.ToString(), windowStyle: windowStyle, waitIdle: waitIdle);
                        break;

                    case ProcessType.Ebest:
                        Start("MTree.EbestPublisher.exe", type.ToString(), windowStyle: windowStyle, waitIdle: waitIdle);
                        break;
                    case ProcessType.Kiwoon:
                        Start("MTree.KiwoomPublisher.exe", type.ToString(), windowStyle: windowStyle, waitIdle: waitIdle);
                        break;
                    case ProcessType.Krx:
                        Start("MTree.KrxPublisher.exe", type.ToString(), windowStyle: windowStyle, waitIdle: waitIdle);
                        break;

                    case ProcessType.Naver:
                        Start("MTree.NaverPublisher.exe", type.ToString(), windowStyle: windowStyle, waitIdle: waitIdle);
                        break;

                    case ProcessType.DaishinPopupStopper:
                        Start("MTree.DaishinPopupStopper.exe", windowStyle: windowStyle, waitIdle: waitIdle);
                        break;

                    case ProcessType.HistorySaver:
                        Start("MTree.HistorySaver.exe", windowStyle: windowStyle, waitIdle: waitIdle);
                        break;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public static void Start(string filePath, string arguments = "", ProcessWindowStyle windowStyle = ProcessWindowStyle.Normal, bool waitIdle = false)
        {
            try
            {
                var process = new Process();
                process.StartInfo.FileName = filePath;
                process.StartInfo.Arguments = arguments;
                process.StartInfo.WindowStyle = windowStyle;
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
