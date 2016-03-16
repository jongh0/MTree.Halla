using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTree.DataStructure;

namespace MTree.Consumer
{
    public class ChartableIndex : IChartable
    {
        public string Code { get; set; }

        public DateTime LastConcludedTime { get; set; }

        public float LastValue { get; set; }

        public Chart GetChart(ChartTypes chartType, DateTime target, TimeSpan interval)
        {
            throw new NotImplementedException();
        }
    }
}
