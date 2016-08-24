using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTree.Consumer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.Consumer.Tests
{
    [TestClass()]
    public class ChartTests
    {
        [TestMethod()]
        public void TickChartTest()
        {
            string code = "000020";
            ChartTypes chartType = ChartTypes.Tick;
            DateTime startDate = new DateTime(2016, 05, 10);
            DateTime endDate = DateTime.Now;
            int interval = 1;
            Chart chart = new Chart(code, chartType, startDate, endDate, interval);
            chart.WaitInitialing();
            
            Assert.Fail();
        }
    }
}