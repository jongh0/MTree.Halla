using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Configuration;
using System.Threading;
using DataStructure;

namespace TestConsole
{
    class PropertyTest
    {
        public string StringProperty
        {
            get { return "string..."; }
        }
    }

    class Program
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            //while (true)
            //{
            //    Thread.Sleep(200);
            //    Console.WriteLine(Config.Default.DateTimeFormat);
            //    logger.Info(Config.Default.DateTimeFormat);
            //}

            //var pt = new PropertyTest();
            //Console.WriteLine(nameof(pt.StringProperty));

            var sm = new StockMaster();
            Console.WriteLine(sm.ToString());

            var sc = new StockConclusion();
            Console.WriteLine(sc.ToString());
        }
    }
}
