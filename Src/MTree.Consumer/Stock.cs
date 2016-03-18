using MTree.DataStructure;
using System;
using System.Collections.Concurrent;

namespace MTree.Consumer
{
    public class Stock : IChartable
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static ConcurrentDictionary<string, Stock> Stocks { get; set; } = new ConcurrentDictionary<string, Stock>();

        public string Code { get; set; }

        public DateTime LastConcludedTime { get; set; }

        public float LastValue { get; set; }

        public MarketTypes MarketType { get; set; }

        public Stock()
        {
        }

        public Stock(string code)
        {
            Code = code;
        }

        public Chart GetChart(ChartTypes chartType, DateTime target, TimeSpan interval)
        {
            throw new NotImplementedException();
        }

        public StockMaster GetMaster(DateTime target)
        {
            throw new NotImplementedException();
        }

        public static Stock GetStock(string code)
        {
            try
            {
                if (Stocks.ContainsKey(code) == false)
                {
                    var stock = new Stock(code);
                    Stocks.TryAdd(code, stock);
                    return stock;
                }

                return Stocks[code];
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return null;
        }
    }
}
