using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTree.Configuration;
using System.Threading;
using MTree.DbProvider;
using MTree.DataStructure;
using MTree.Utility;

namespace TestConsole
{
    class Program
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            Config.Initialize();

            //TestPushService();
            //TestEmail();

            Console.ReadLine();
        }

        private static void TestEmail()
        {
            //EmailUtility.SendEmail("vvvv", "vvvv", @"D:\Documents\GitHub\MTree.Halla\Src\bin\Debug\Logs\MTree.All.2016-03-23.zip");
            EmailUtility.SendEmail("vvv", "vvvvv");
        }

        private static void TestPushService()
        {
            PushUtility.NotifyMessage("Hello MTree");
        }
    }
}
