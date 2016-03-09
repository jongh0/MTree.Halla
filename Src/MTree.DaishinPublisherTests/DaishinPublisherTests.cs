using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTree.DaishinPublisher;
using MTree.DataStructure;
using System;
using System.Collections.Generic;
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
            //bool result = publisher.GetQuote(code, ref master);
            //Assert.IsTrue(result);
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
    }
}