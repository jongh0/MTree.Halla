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
            int interval = 3;
            Chart chart = new Chart(code, chartType, startDate, endDate, interval);
            chart.WaitInitialing();
            
            Assert.Fail();
        }
        [TestMethod()]
        public void MinChartTest()
        {
            string code = "000020";
            ChartTypes chartType = ChartTypes.Min;
            DateTime startDate = new DateTime(2016, 05, 10);
            DateTime endDate = DateTime.Now;
            int interval = 3;
            Chart chart = new Chart(code, chartType, startDate, endDate, interval);
            chart.WaitInitialing();

            Assert.Fail();
        }

        [TestMethod()]
        public void DayChartTest()
        {
            string code = "000020";
            ChartTypes chartType = ChartTypes.Day;
            DateTime startDate = new DateTime(2016, 05, 10);
            DateTime endDate = DateTime.Now;
            Chart chart = new Chart(code, chartType, startDate, endDate);
            chart.WaitInitialing();

            Assert.Fail();
        }

        [TestMethod()]
        public void WeekChartTest()
        {
            string code = "000020";
            ChartTypes chartType = ChartTypes.Week;
            DateTime startDate = new DateTime(2016, 05, 10);
            DateTime endDate = DateTime.Now;
            Chart chart = new Chart(code, chartType, startDate, endDate);
            chart.WaitInitialing();

            Assert.Fail();
        }

        [TestMethod()]
        public void MonthChartTest()
        {
            string code = "000020";
            ChartTypes chartType = ChartTypes.Month;
            DateTime startDate = new DateTime(2016, 05, 10);
            DateTime endDate = DateTime.Now;
            Chart chart = new Chart(code, chartType, startDate, endDate);
            chart.WaitInitialing();

            Assert.Fail();
        }

        [TestMethod()]
        public void YearChartTest()
        {
            string code = "000020";
            ChartTypes chartType = ChartTypes.Year;
            DateTime startDate = new DateTime(2016, 05, 10);
            DateTime endDate = DateTime.Now;
            Chart chart = new Chart(code, chartType, startDate, endDate);
            chart.WaitInitialing();

            Assert.Fail();
        }

    }
}