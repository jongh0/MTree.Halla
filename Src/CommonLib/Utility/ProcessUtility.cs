﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CommonLib.Utility
{
    public class ProcessUtility
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private static Dictionary<ProcessTypes, string> ProcessList { get; set; }

        static ProcessUtility()
        {
            ProcessList = new Dictionary<ProcessTypes, string>();
            ProcessList.Add(ProcessTypes.CybosStarter, "CpStart");
            ProcessList.Add(ProcessTypes.DibServer, "DibServer");
            ProcessList.Add(ProcessTypes.KiwoomStarter, "khministarter");
            ProcessList.Add(ProcessTypes.Dashboard, "Dashboard");
            ProcessList.Add(ProcessTypes.HistorySaver, "HistorySaver");
            ProcessList.Add(ProcessTypes.StrategyManager, "StrategyManager");
            ProcessList.Add(ProcessTypes.TestConsumer, "TestConsumer");
            ProcessList.Add(ProcessTypes.RealTimeProvider, "RealTimeProvider");
            ProcessList.Add(ProcessTypes.DaishinPublisher, "DaishinPublisher");
            ProcessList.Add(ProcessTypes.DaishinPublisherMaster, "DaishinPublisher");
            ProcessList.Add(ProcessTypes.EbestPublisher, "EbestPublisher");
            ProcessList.Add(ProcessTypes.KiwoomPublisher, "KiwoomPublisher");
            ProcessList.Add(ProcessTypes.KiwoomSessionManager, "KiwoomSessionManager");
            ProcessList.Add(ProcessTypes.TestPublisher, "TestPublisher");
            ProcessList.Add(ProcessTypes.EbestTrader, "EbestTrader");
            ProcessList.Add(ProcessTypes.KiwoomTrader, "KiwoomTrader");
            ProcessList.Add(ProcessTypes.VirtualTrader, "VirtualTrader");
            ProcessList.Add(ProcessTypes.KillAll, "KillAll");
            ProcessList.Add(ProcessTypes.AutoLauncher, "AutoLauncher");
            ProcessList.Add(ProcessTypes.DaishinSessionManager, "DaishinSessionManager");
            ProcessList.Add(ProcessTypes.DataValidator, "DataValidator");
            ProcessList.Add(ProcessTypes.DataValidatorRegularCheck, "DataValidator");
            ProcessList.Add(ProcessTypes.PopupStopper, "PopupStopper");
            ProcessList.Add(ProcessTypes.ResourceMonitor, "ResourceMonitor");
            ProcessList.Add(ProcessTypes.SendLog, "SendLog");
            ProcessList.Add(ProcessTypes.TestConsole, "TestConsole");
        }

        public static Process Start(ProcessTypes type, ProcessWindowStyle windowStyle = ProcessWindowStyle.Normal)
        {
            if (ProcessList.ContainsKey(type) == true)
                return Start(ProcessList[type] + ".exe", type.ToString(), windowStyle: windowStyle);

            return null;
        }

        public static Process Start(ProcessTypes type, string arguments, ProcessWindowStyle windowStyle = ProcessWindowStyle.Normal)
        {
            if (ProcessList.ContainsKey(type) == true)
                return Start(ProcessList[type] + ".exe", arguments, windowStyle: windowStyle);

            return null;
        }

        public static Process Start(string filePath, string arguments = "", ProcessWindowStyle windowStyle = ProcessWindowStyle.Normal)
        {
            try
            {
                var process = new Process();
                process.StartInfo.Verb = "runas";
                process.StartInfo.FileName = filePath;
                process.StartInfo.Arguments = arguments;
                process.StartInfo.WindowStyle = windowStyle;
                process.Start();

                _logger.Info($"{Path.GetFileName(filePath)} {arguments} process started");
                return process;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return null;
        }

        public static void Kill(ProcessTypes type, int excludeId = -1)
        {
            if (ProcessList.ContainsKey(type) == true)
                Kill(ProcessList[type], excludeId);
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
                        _logger.Info($"{p.ProcessName}, {p.Id} process killed");
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                }
            }
        }
        public static void Kill(int excludeId)
        {
            var process = Process.GetProcessById(excludeId);
            if (process == null)
            {
                _logger.Info($"Process({process.Id}) is not found");
                return;
            }

            process.Kill();
            _logger.Info($"{process.ProcessName}, {process.Id} process killed");
        }

        public static void Close(int excludeId)
        {
            var process = Process.GetProcessById(excludeId);
            if (process == null)
            {
                _logger.Info($"Process({process.Id}) is not found");
                return;
            }

            process.Close();
            _logger.Info($"{process.ProcessName}, {process.Id} process closed");
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

        public static bool WaitIfNotExists(ProcessTypes type, int timeout = 1000 * 2)
        {
            var retry = timeout / 100;

            while (retry-- > 0)
            {
                if (Exists(type) == true)
                    return true;

                Thread.Sleep(100);
            }

            return false;
        }

        public static bool WaitIfNotExists(string processName, int timeout = 1000 * 2)
        {
            var retry = timeout / 100;

            while (retry-- > 0)
            {
                if (Exists(processName) == true)
                    return true;

                Thread.Sleep(100);
            }

            return false;
        }
    }
}
