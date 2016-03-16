using MTree.DataStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.Consumer
{
    public class ChartableStock : IChartable
    {
        public string Code { get; set; }

        public DateTime LastConcludedTime { get; set; }

        public float LastValue { get; set; }

        public MarketTypes MarketType { get; set; }

        public Chart GetChart(ChartTypes chartType, DateTime target, TimeSpan interval)
        {
            throw new NotImplementedException();
        }

        public StockMaster GetMaster(DateTime target)
        {
            throw new NotImplementedException();
        }
    }
}
