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
        private NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private const string LogBasePath = "Logs";
        private const string CounterListFile = "CounterList.txt";
        private const string CounterName = "MtreeMon";

        public int QueryMonitor()
        {
            var process = new Process();
            _logger.Info("Start Logman Query.");
            process.StartInfo = new ProcessStartInfo("logman", $"query {CounterName}")
            {
                UseShellExecute = false,
                Verb = "runas"
            };
            process.Start();
            process.WaitForExit();
            _logger.Info($"Start Logman Query Done. Result:{process.ExitCode}");
            return process.ExitCode;
        }

        public int CreateMonitor()
        {
            var process = new Process();
            _logger.Info($"Start Logman Create.");
            string outputFile = Path.Combine(LogBasePath, Config.General.DateNow, DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".blg");
            process.StartInfo = new ProcessStartInfo("logman", $"create counter {CounterName} -o {outputFile} -cf {CounterListFile} -si {Config.ResourceMonitor.SamplingFrequence}")
            {
                UseShellExecute = false,
                Verb = "runas"
            };
            process.Start();
            process.WaitForExit();
            _logger.Info($"Start Logman Create Done. Result:{process.ExitCode}");
            return process.ExitCode;
        }

        public int StartMonitor()
        {
            var process = new Process();
            _logger.Info($"Start Logman Start.");
            process.StartInfo = new ProcessStartInfo("logman", $"start {CounterName}")
            {
                UseShellExecute = false,
                Verb = "runas"
            };
            process.Start();
            process.WaitForExit();
            _logger.Info($"Start Logman Start Done. Result:{process.ExitCode}");
            return process.ExitCode;
        }

        public int StopMonitor()
        {
            var process = new Process();
            _logger.Info($"Start Logman Stop Done.");
            process.StartInfo = new ProcessStartInfo("logman", $"stop {CounterName}")
            {
                UseShellExecute = false,
                Verb = "runas"
            };
            process.Start();
            process.WaitForExit();
            _logger.Info($"Start Logman Stop Done. Result:{process.ExitCode}");
            return process.ExitCode;
        }

        public int DeleteMonitor()
        {
            var process = new Process();
            _logger.Info($"Start Logman Delete Done.");
            process.StartInfo = new ProcessStartInfo("logman", $"delete {CounterName}")
            {
                UseShellExecute = false,
                Verb = "runas"
            };
            process.Start();
            process.WaitForExit();

            _logger.Info($"Start Logman Delete Done. Result:{process.ExitCode}");
            return process.ExitCode;
        }
    }
}
