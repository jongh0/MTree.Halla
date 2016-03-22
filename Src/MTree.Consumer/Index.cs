using System;
using MTree.DataStructure;
using System.Collections.Concurrent;
using System.Text;
using MTree.Configuration;

namespace MTree.Consumer
{
    public class Index : IChartable
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static ConcurrentDictionary<string, Index> Indexes { get; set; } = new ConcurrentDictionary<string, Index>();

        public string Code { get; set; }

        public DateTime LastTime { get; set; }

        public float LastValue { get; set; }

        public Index(string code)
        {
            Code = code;
        }

        public Chart GetChart(ChartTypes chartType, DateTime startDate, DateTime endDate)
        {
            return new Chart(chartType, startDate, endDate);
        }

        public static Index GetIndex(string code)
        {
            try
            {
                if (Indexes.ContainsKey(code) == false)
                {
                    var index = new Index(code);
                    Indexes.TryAdd(code, index);
                    return index;
                }

                return Indexes[code];
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return null;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{nameof(Code)}: {Code}");
            sb.AppendLine($"{nameof(LastTime)}: {LastTime}");
            sb.AppendLine($"{nameof(LastValue)}: {LastValue}");

            return sb.ToString();
        }
    }
}
