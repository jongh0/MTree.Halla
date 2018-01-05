using Microsoft.VisualStudio.TestTools.UnitTesting;
using DaishinPublisher;
using DataStructure;
using RealTimeProvider;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DaishinPublisher.Tests
{
    [TestClass()]
    public class DaishinPublisherTests
    {
        [TestMethod()]
        public void GetQuoteTest()
        {
            string code = "000020";
            StockMaster master = new StockMaster();
            DaishinPublisher_ publisher = new DaishinPublisher_();
            int startTick = Environment.TickCount;
            bool result = publisher.GetQuote(code, ref master);
            Trace.WriteLine(">>>>>>>>>>>>>>> " + (Environment.TickCount - startTick));
            Debugger.Break();
        }

        [TestMethod()]
        public void SubscribeStockTest()
        {
            DaishinPublisher_ publisher = new DaishinPublisher_();
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
            DaishinPublisher_ publisher = new DaishinPublisher_();
            Dictionary<string, CodeEntity> list = publisher.GetCodeList();
            Assert.IsTrue(cnt > 0);
        }

        [TestMethod()]
        public void IsSubscribableTest()
        {
            DaishinPublisher_ publisher = new DaishinPublisher_();
            publisher.IsSubscribable();
            Assert.Fail();
        }

        [TestMethod()]
        public void SubscribeIndexTest()
        {
            DaishinPublisher_ publisher = new DaishinPublisher_();
            publisher.SubscribeIndex("001");
            while (true) ;
        }

        [TestMethod()]
        public void GetIndexMasterTest()
        {
            DaishinPublisher_ publisher = new DaishinPublisher_();
            publisher.GetIndexMaster("001");
            while (true) ;
        }

        [TestMethod()]
        public void GetThemeListTest()
        {
            DaishinPublisher_ publisher = new DaishinPublisher_();
            //publisher.GetCodeMap(CodeMapTypes.Theme);
            publisher.GetCodeMap(CodeMapTypes.Group);
            while (true) ;
        }
    }
}