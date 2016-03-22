using MongoDB.Driver;
using MTree.Configuration;
using MTree.DataStructure;
using MTree.DbProvider;
using MTree.Utility;
using System;
using System.Collections.Generic;
using System.Text;

namespace MTree.Consumer
{
    public enum ChartTypes
    {
        Tick,
        Min,
        Day,
        Week,
        Month,
    }

    [Serializable]
    public class Chart
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public string Code { get; set; }

        public ChartTypes ChartType { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public SortedList<DateTime, Candle> Candles { get; set; } = new SortedList<DateTime, Candle>();

        public Chart(ChartTypes chartType, DateTime startDate, DateTime endDate)
        {
            ChartType = chartType;

            if (startDate == null) startDate = Config.General.DefaultStartDate;
            if (endDate == null) endDate = DateTime.Now;

            StartDate = DateTimeUtility.StartDateTime(startDate);
            EndDate = DateTimeUtility.EndDateTime(endDate);
        }

        private async void FillCandles()
        {
            try
            {
                IMongoCollection<Candle> collection = MongoDbProvider.Instance.GetDatabase(DbTypes.Chart).GetCollection<Candle>(Code);

                var builder = Builders<Candle>.Filter;
                var filter = builder.Gte(i => i.Time, StartDate) & builder.Lte(i => i.Time, EndDate);
                var result = await collection.Find(filter).ToListAsync();

                foreach (var candle in result)
                {
                    if (Candles.ContainsKey(candle.Time) == false)
                        Candles.Add(candle.Time, candle);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public Candle CandleAt(DateTime dateTime)
        {
            if (dateTime == null) return null;

            if (Candles.IndexOfKey(dateTime) == -1)
                return null;

            return Candles[dateTime];
        }

        public Candle NextCandle(Candle baseCandle)
        {
            if (baseCandle == null) return null;

            try
            {
                int index = Candles.IndexOfValue(baseCandle);
                if (index >= 0 && Candles.Count > index + 1)
                    return Candles.Values[index + 1];
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return null;
        }

        public Candle PrevCandle(Candle baseCandle)
        {
            if (baseCandle == null) return null;

            try
            {
                int index = Candles.IndexOfValue(baseCandle);
                if (index > 0)
                    return Candles.Values[index - 1];
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return null;
        }

        public static CandleTypes ConvertToCandleType(ChartTypes type)
        {
            switch (type)
            {
                case ChartTypes.Min:    return CandleTypes.Min;
                case ChartTypes.Day:    return CandleTypes.Day;
                case ChartTypes.Week:   return CandleTypes.Week;
                case ChartTypes.Month:  return CandleTypes.Month;
                default:                return CandleTypes.Tick;
            }
        }

        public static ChartTypes ConvertToChartType(CandleTypes type)
        {
            switch (type)
            {
                case CandleTypes.Min:   return ChartTypes.Min;
                case CandleTypes.Day:   return ChartTypes.Day;
                case CandleTypes.Week:  return ChartTypes.Week;
                case CandleTypes.Month: return ChartTypes.Month;
                default:                return ChartTypes.Tick;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{nameof(Code)}: {Code}");
            sb.AppendLine($"{nameof(ChartType)}: {ChartType}");
            sb.AppendLine($"{nameof(Candles)}: {Candles.Count}");

            return sb.ToString();
        }
    }
}
