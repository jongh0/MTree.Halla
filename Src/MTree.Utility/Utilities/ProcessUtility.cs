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
        Unknown,
        CybosStarter,
        Dashboard,
        HistorySaver,
        StrategyManager,
        TestConsumer,
        RealTimeProvider,
        DaishinPublisher,
        DaishinPublisherMaster,
        EbestPublisher,
        KiwoomPublisher,
        TestPublisher,
        EbestTrader,
        KiwoomTrader,
        VirtualTrader,
        KillAll,
        AutoLauncher,
        DaishinSessionManager,
        DataValidator,
        PopupStopper,
        SendLog,
        TestConsole,
    }

    public class ProcessUtility
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private static Dictionary<ProcessTypes, string> ProcessList { get; set; }

        static ProcessUtility()
        {
            ProcessList = new Dictionary<ProcessTypes, string>();
            ProcessList.Add(ProcessTypes.CybosStarter, "CpStart");
            ProcessList.Add(ProcessTypes.Dashboard, "MTree.Dashboard");
            ProcessList.Add(ProcessTypes.HistorySaver, "MTree.HistorySaver");
            ProcessList.Add(ProcessTypes.StrategyManager, "MTree.StrategyManager");
            ProcessList.Add(ProcessTypes.TestConsumer, "TestConsumer");
            ProcessList.Add(ProcessTypes.RealTimeProvider, "MTree.RealTimeProvider");
            ProcessList.Add(ProcessTypes.DaishinPublisher, "MTree.DaishinPublisher");
            ProcessList.Add(ProcessTypes.DaishinPublisherMaster, "MTree.DaishinPublisher");
            ProcessList.Add(ProcessTypes.EbestPublisher, "MTree.EbestPublisher");
            ProcessList.Add(ProcessTypes.KiwoomPublisher, "MTree.KiwoomPublisher");
            ProcessList.Add(ProcessTypes.TestPublisher, "TestPublisher");
            ProcessList.Add(ProcessTypes.EbestTrader, "MTree.EbestTrader");
            ProcessList.Add(ProcessTypes.KiwoomTrader, "MTree.KiwoomTrader");
            ProcessList.Add(ProcessTypes.VirtualTrader, "MTree.VirtualTrader");
            ProcessList.Add(ProcessTypes.KillAll, "KillAll");
            ProcessList.Add(ProcessTypes.AutoLauncher, "MTree.AutoLauncher");
            ProcessList.Add(ProcessTypes.DaishinSessionManager, "MTree.DaishinSessionManager");
            ProcessList.Add(ProcessTypes.DataValidator, "MTree.DataValidator");
            ProcessList.Add(ProcessTypes.PopupStopper, "MTree.PopupStopper");
            ProcessList.Add(ProcessTypes.SendLog, "MTree.SendLog");
            ProcessList.Add(ProcessTypes.TestConsole, "TestConsole");
        }

        public static Process Start(ProcessTypes type, ProcessWindowStyle windowStyle = ProcessWindowStyle.Normal)
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
