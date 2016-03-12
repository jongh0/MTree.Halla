using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTree.EbestPublisher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTree.EbestPublisher.Tests
{
    [TestClass()]
    public class EbestPublisherTests
    {
        [TestMethod()]
        public void LoginTest()
        {
            EbestPublisher publisher = new EbestPublisher();
            Assert.IsTrue(publisher.Login());
        }

        [TestMethod()]
        public void GetQuoteTest()
        {
            var task = Task.Run(() => { EbestPublisher publisher = new EbestPublisher(); });
            Thread.Sleep(1000);
            Assert.Fail();
        }
    }
}