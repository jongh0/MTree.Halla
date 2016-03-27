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
        CybosStarter,
        TestConsole,
        TestConsumer,
        TestPublisher,
        Daishin,
        DaishinMaster,
        DaishinPopupStopper,
        DaishinSessionManager,
        Kiwoom,
        Ebest,
        Krx,
        HistorySaver,
        RealTimeProvider,
        DashBoard,
    }

    public class ProcessUtility
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private static Dictionary<ProcessTypes, string> ProcessList { get; set; }

        static ProcessUtility()
        {
            ProcessList = new Dictionary<ProcessTypes, string>();
            ProcessList.Add(ProcessTypes.CybosStarter, "CpStart");
            ProcessList.Add(ProcessTypes.TestConsole, "TestConsole");
            ProcessList.Add(ProcessTypes.TestConsumer, "TestConsumer");
            ProcessList.Add(ProcessTypes.TestPublisher, "TestPublisher");
            ProcessList.Add(ProcessTypes.Daishin, "MTree.DaishinPublisher");
            ProcessList.Add(ProcessTypes.DaishinMaster, "MTree.DaishinPublisher");
            ProcessList.Add(ProcessTypes.DaishinPopupStopper, "MTree.DaishinPopupStopper");
            ProcessList.Add(ProcessTypes.DaishinSessionManager, "MTree.DaishinSessionManager");
            ProcessList.Add(ProcessTypes.Kiwoom, "MTree.KiwoomPublisher");
            ProcessList.Add(ProcessTypes.Ebest, "MTree.EbestPublisher");
            ProcessList.Add(ProcessTypes.Krx, "MTree.KrxPublisher");
            ProcessList.Add(ProcessTypes.HistorySaver, "MTree.HistorySaver");
            ProcessList.Add(ProcessTypes.RealTimeProvider, "MTree.RealTimeProvider");
            ProcessList.Add(ProcessTypes.DashBoard, "MTree.DashBoard");
        }

        public static Process Start(ProcessTypes type, ProcessWindowStyle windowStyle = ProcessWindowStyle.Minimized)
        {
            if (ProcessList.ContainsKey(type) == true)
                return Start(ProcessList[type] + ".exe", type.ToString(), windowStyle: windowStyle);

            return null;
        }

        public static Process Start(string filePath, string arguments = "", ProcessWindowStyle windowStyle = ProcessWindowStyle.Normal)
        {
            try
            {
                var process = new Process();
                process.StartInfo.FileName = filePath;
                process.StartInfo.Arguments = arguments;
                process.StartInfo.WindowStyle = windowStyle;
                process.Start();

                logger.Info($"{Path.GetFileName(filePath)} {arguments} process started");
                return process;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return null;
        }

        public static void Kill(ProcessTypes type)
        {
            if (ProcessList.ContainsKey(type) == true)
                Kill(ProcessList[type]);
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

        public static bool Exists(ProcessTypes type)
        {
            if (ProcessList.ContainsKey(type) == true)
                return Exists(ProcessList[type]);

            return false;
        }

        public static bool Exists(string processName)
        {
            var processList = Process.GetProcessesByName(processName);
            return (processList != null && processList.Length > 0);
        }
    }
}
