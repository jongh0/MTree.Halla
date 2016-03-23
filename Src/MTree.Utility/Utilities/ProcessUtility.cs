using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.Utility
{
    public enum ProcessTypes
    {
        None,
        Daishin,
        DaishinMaster,
        DaishinPopupStopper,
        DaishinSessionManager,
        Kiwoom,
        Ebest,
        Krx,
        Naver,
        HistorySaver,
    }

    public class ProcessUtility
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static string CybosStarterName = "CpStart";

        public static Process Start(ProcessTypes type)
        {
            var windowStyle = ProcessWindowStyle.Minimized;

            switch (type)
            {
                case ProcessTypes.Daishin:
                case ProcessTypes.DaishinMaster:
                    return Start("MTree.DaishinPublisher.exe", type.ToString(), windowStyle: windowStyle);

                case ProcessTypes.Ebest:
                    return Start("MTree.EbestPublisher.exe", type.ToString(), windowStyle: windowStyle);

                case ProcessTypes.Kiwoom:
                    return Start("MTree.KiwoomPublisher.exe", type.ToString(), windowStyle: windowStyle);

                case ProcessTypes.Krx:
                    return Start("MTree.KrxPublisher.exe", type.ToString(), windowStyle: windowStyle);

                case ProcessTypes.Naver:
                    return Start("MTree.NaverPublisher.exe", type.ToString(), windowStyle: windowStyle);

                case ProcessTypes.DaishinPopupStopper:
                    return Start("MTree.DaishinPopupStopper.exe", windowStyle: windowStyle);

                case ProcessTypes.HistorySaver:
                    return Start("MTree.HistorySaver.exe", windowStyle: windowStyle);

                case ProcessTypes.DaishinSessionManager:
                    return Start("MTree.DaishinSessionManager.exe", windowStyle: windowStyle);

                default:
                    return null;
            }
        }

        public static Process Start(string filePath, string arguments = "", ProcessWindowStyle windowStyle = ProcessWindowStyle.Normal)
        {
            try
            {
                logger.Info($"{Path.GetFileName(filePath)} {arguments} process start");

                var process = new Process();
                process.StartInfo.FileName = filePath;
                process.StartInfo.Arguments = arguments;
                process.StartInfo.WindowStyle = windowStyle;
                process.Start();

                return process;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return null;
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

        public static bool Exists(string processName)
        {
            var processList = Process.GetProcessesByName(processName);
            return (processList != null && processList.Length > 0);
        }
    }
}
