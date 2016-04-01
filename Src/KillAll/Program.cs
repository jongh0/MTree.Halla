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
            "MTree.HistorySaver",
            "MTree.DashBoard",
            "MTree.DaishinPublisher",
            "MTree.DaishinPublisherTests",
            "MTree.KrxPublisher",
            "MTree.EbestPublisher",
            "MTree.KiwoomPublisher",
            "MTree.RealTimeProvider",
            "MTree.RealTimeProviderTests",
            "MTree.DaishinSessionManager",
            "MTree.PopupStopper",
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
