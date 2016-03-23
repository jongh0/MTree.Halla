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
        }

        private static void TestPushService()
        {
            PushUtility.NotifyMessage("Hello MTree");
        }
    }
}
