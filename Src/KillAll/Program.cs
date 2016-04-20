using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KillAll
{
    class Program
    {
        static string[] proccessNameList = {
            "MTree.DashBoard",
            "MTree.HistorySaver",
            "MTree.StrategyManager",
            "MTree.RealTimeProvider",
            "MTree.RealTimeProviderTests",
            "MTree.DaishinPublisher",
            "MTree.DaishinPublisherTests",
            "MTree.EbestPublisher",
            "MTree.KiwoomPublisher",
            "MTree.EbestTrader",
            "MTree.KiwoomTrader",
            "MTree.VirtualTrader",
            "MTree.DaishinSessionManager",
            "MTree.DataValidator",
            "MTree.PopupStopper",
            "MTree.SendLog",
            "TestConsole",
            "TestConsumer",
            "TestPublisher"
        };

        static void Main(string[] args)
        {
            try
            {
                foreach (var processName in proccessNameList)
                {
                    KillProcess(processName);
                }

                Thread.Sleep(200);

                foreach (var processName in proccessNameList)
                {
                    KillProcess(processName);
                }

                Thread.Sleep(500);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        static void KillProcess(string processName)
        {
            var processList = Process.GetProcessesByName(processName);
            if (processList == null)
                return;

            foreach (var p in processList)
            {
                try
                {
                    p.Kill();
                    Console.WriteLine($"{p.ProcessName}, {p.Id} process killed");
                }
                catch (Exception)
                {
                    Console.WriteLine($"{p.ProcessName}, {p.Id} has a problem");
                }
            }
        }
    }
}
