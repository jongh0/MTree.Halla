using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTree.Configuration;
using System.Threading;

namespace TestConsole
{
    class Program
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            Console.WriteLine(Config.Ebest.UserPw);

            //while (true)
            //{
            //    logger.Info("hhhhhhh");
            //    Thread.Sleep(500);
            //}
        }
    }
}
