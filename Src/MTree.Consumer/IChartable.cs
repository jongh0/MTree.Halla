using MTree.DataStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.Consumer
{
    public interface IChartable
    {
        string Code { get; set; }

        float LastValue { get; set; }

        DateTime LastConcludedTime { get; set; }

        Chart GetChart(ChartTypes chartType, DateTime target, TimeSpan interval);
    }
}
