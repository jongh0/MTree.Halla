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
            string code = "000040";
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
            DaishinPublisher publisher = new DaishinPublisher();
            Dictionary<string, CodeEntity> list = publisher.GetCodeList();
            for (int i = 0; i < 400; i++)
            {
                bool result = publisher.SubscribeStock("A" + list.ToArray()[i].Key);
                Assert.IsTrue(result);
            }
            
            while (true) ;
        }

        [TestMethod()]
        public void GetStockCodeListTest()
        {
            int cnt = 0;
            DaishinPublisher publisher = new DaishinPublisher();
            Dictionary<string, CodeEntity> list = publisher.GetCodeList();
            Assert.IsTrue(cnt > 0);
        }

        [TestMethod()]
        public void IsSubscribableTest()
        {
            DaishinPublisher publisher = new DaishinPublisher();
            publisher.IsSubscribable();
            Assert.Fail();
        }
    }
}