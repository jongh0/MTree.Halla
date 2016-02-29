using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Configuration;
using System.Threading;

namespace TestConsole
{
    class Program
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            while (true)
            {
                Thread.Sleep(200);
                Console.WriteLine(Config.Default.DateTimeFormat);
                logger.Info(Config.Default.DateTimeFormat);
            }
        }
    }
}
