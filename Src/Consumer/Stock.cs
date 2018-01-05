using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using Configuration;
using DataStructure;
using DbProvider;
using System;
using System.Collections.Concurrent;
using System.Text;

namespace Consumer
{
    public class Stock : IChartable, ICodeMap
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public static ConcurrentDictionary<string, Stock> Stocks { get; set; } = new ConcurrentDictionary<string, Stock>();

        public string Code { get; set; }

        public string Name { get; set; }

        public DateTime LastTime { get; set; }

        public float Open { get; set; }
        public float High { get; set; }
        public float Low { get; set; }
        public float Close { get; set; }
        public float Volume { get; set; }

        public float LastValue { get; set; }

        public MarketTypes MarketType { get; set; }

        private Stock(string code, string name = "")
        {
            Code = code;
            Name = name;
        }

        public Chart GetChart(ChartTypes chartType, DateTime startDate, DateTime endDate)
        {
            return new Chart(Code, chartType, startDate, endDate);
        }

        public StockMaster GetMaster(DateTime date)
        {
            try
            {
                var targetDate = new DateTime(date.Year, date.Month, date.Day);
                var builder = Builders<StockMaster>.Filter;
                var filter = builder.Eq(i => i.Time, targetDate);

                return DbAgent.Instance.Find(Code, filter).FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return null;
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
                _logger.Error(ex);
            }

            return null;
        }

        public void UpdateLastConclusion(StockConclusion conclusion)
        {
            if (LastTime.ToString(Config.General.DateFormat) != conclusion.Time.ToString(Config.General.DateFormat))
            {
                Open = High = Low = Close = conclusion.Price;
            }
            else
            {
                High = Math.Max(High, conclusion.Price);
                Low = Math.Min(Low, conclusion.Price);
                Close = conclusion.Price;
            }
            Volume += conclusion.Amount;

            LastTime = conclusion.Time;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{nameof(Code)}: {Code}");
            sb.AppendLine($"{nameof(LastTime)}: {LastTime}");
            sb.AppendLine($"{nameof(LastValue)}: {LastValue}");
            sb.AppendLine($"{nameof(MarketType)}: {MarketType}");

            return sb.ToString();
        }
    }
}
