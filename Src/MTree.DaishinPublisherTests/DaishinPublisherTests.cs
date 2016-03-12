using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTree.DaishinPublisher;
using MTree.DataStructure;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.DaishinPublisher.Tests
{
    [TestClass()]
    public class DaishinPublisherTests
    {
        [TestMethod()]
        public void GetQuoteTest()
        {
            string code = "000020";
            StockMaster master = new StockMaster();
            DaishinPublisher publisher = new DaishinPublisher();
            int startTick = Environment.TickCount;
            bool result = publisher.GetQuote(code, ref master);
            Trace.WriteLine(">>>>>>>>>>>>>>> " + (Environment.TickCount - startTick));
            Debugger.Break();
        }

        [TestMethod()]
        public void SubscribeStockTest()
        {
            string code = "000020";
            DaishinPublisher publisher = new DaishinPublisher();
            bool result = publisher.SubscribeStock(code);
            Assert.IsTrue(result);
            while (true) ;
        }

        [TestMethod()]
        public void GetStockCodeListTest()
        {
            int cnt = 0;
            DaishinPublisher publisher = new DaishinPublisher();
            foreach (string code in publisher.GetStockCodeList().Keys)
            {
                Console.WriteLine($"{cnt++}, {code}");

            }
            Assert.IsTrue(cnt > 0);
        }
    }
}