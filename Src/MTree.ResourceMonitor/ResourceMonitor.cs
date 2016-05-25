using MTree.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.ResourceMonitor
{
    public class ResourceMonitor
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private const string logBasePath = "Logs";
        private string counterListFile = "CounterList.txt";
        private string counterName = "MtreeMon";

        public int QueryMonitor()
        {
            var process = new Process();
            logger.Info("Start Logman Query.");
            process.StartInfo = new ProcessStartInfo("logman", $"query {counterName}")
            {
                UseShellExecute = false,
                Verb = "runas"
            };
            process.Start();
            process.WaitForExit();
            logger.Info($"Start Logman Query Done. Result:{process.ExitCode}");
            return process.ExitCode;
        }

        public int CreateMonitor()
        {
            var process = new Process();
            logger.Info($"Start Logman Create.");
            string outputFile = Path.Combine(logBasePath, Config.General.DateNow, DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".blg");
            process.StartInfo = new ProcessStartInfo("logman", $"create counter {counterName} -o {outputFile} -cf {counterListFile} -si {Config.ResourceMonitor.SamplingFrequence}")
            {
                UseShellExecute = false,
                Verb = "runas"
            };
            process.Start();
            process.WaitForExit();
            logger.Info($"Start Logman Create Done. Result:{process.ExitCode}");
            return process.ExitCode;
        }

        public int StartMonitor()
        {
            var process = new Process();
            logger.Info($"Start Logman Start.");
            process.StartInfo = new ProcessStartInfo("logman", $"start {counterName}")
            {
                UseShellExecute = false,
                Verb = "runas"
            };
            process.Start();
            process.WaitForExit();
            logger.Info($"Start Logman Start Done. Result:{process.ExitCode}");
            return process.ExitCode;
        }

        public int StopMonitor()
        {
            var process = new Process();
            logger.Info($"Start Logman Stop Done.");
            process.StartInfo = new ProcessStartInfo("logman", $"stop {counterName}")
            {
                UseShellExecute = false,
                Verb = "runas"
            };
            process.Start();
            process.WaitForExit();
            logger.Info($"Start Logman Stop Done. Result:{process.ExitCode}");
            return process.ExitCode;
        }

        public int DeleteMonitor()
        {
            var process = new Process();
            logger.Info($"Start Logman Delete Done.");
            process.StartInfo = new ProcessStartInfo("logman", $"delete {counterName}")
            {
                UseShellExecute = false,
                Verb = "runas"
            };
            process.Start();
            process.WaitForExit();

            logger.Info($"Start Logman Delete Done. Result:{process.ExitCode}");
            return process.ExitCode;
        }
    }
}
