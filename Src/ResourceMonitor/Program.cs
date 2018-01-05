using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResourceMonitor
{
    class Program
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        static int Main(string[] args)
        {
            int exitCode = 0;

            ResourceMonitor monitor = new ResourceMonitor();

            if (args.Count() > 0)
            {
                if (args[0] == "Start")
                {
                    if (monitor.QueryMonitor() != 0)
                        monitor.CreateMonitor();
                    monitor.StartMonitor();
                }
                else if (args[0] == "Stop")
                {
                    monitor.StopMonitor();
                    monitor.DeleteMonitor();
                }
                else
                {
                    _logger.Error("Invalid Arguments");
                }
            }
            else
            {
                if (monitor.QueryMonitor() != 0)
                    monitor.CreateMonitor();
                monitor.StartMonitor();
                Console.ReadLine();
                monitor.StopMonitor();
                monitor.DeleteMonitor();
            }

            return exitCode;
        }
    }
}
