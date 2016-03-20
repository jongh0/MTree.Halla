using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.DataStructure
{
    public enum ChartTypes
    {
        Tick,
        Min,
        Day,
        Week,
        Month,
    }

    [BsonDiscriminator(RootClass = true)]
    [Serializable]
    public class Chart
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("C")]
        public string Code { get; set; }

        [BsonElement("CT")]
        public ChartTypes ChartType { get; set; }

        [BsonElement("IA")]
        public bool IsAdjusted { get; set; }

        [BsonIgnore]
        public SortedList<DateTime, Candle> TickCandles { get; set; }

        [BsonIgnore]
        public SortedList<DateTime, Candle> MinCandles { get; set; }

        [BsonIgnore]
        public SortedList<DateTime, Candle> DayCandles { get; set; }

        [BsonIgnore]
        public SortedList<DateTime, Candle> WeekCandles { get; set; }

        [BsonIgnore]
        public SortedList<DateTime, Candle> MonthCandles { get; set; }

        public Chart ConvertType(ChartTypes chartType, TimeSpan interval)
        {
            throw new NotImplementedException();
        }

        public Candle CandleAt(DateTime dateTime)
        {
            throw new NotImplementedException();
        }

        public Candle NextCandle(Candle baseCandle) // Sample code
        {
            if (baseCandle == null) return null;

            try
            {
                var candels = GetCandles(baseCandle.CandleType);
                if (candels == null) return null;

                int index = candels.IndexOfValue(baseCandle);
                if (index >= 0 && candels.Count > index + 1)
                    return candels.Values[index + 1];
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return null;
        }

        public Candle PrevCandle(Candle baseCandle) // Sample code
        {
            if (baseCandle == null) return null;

            try
            {
                var candels = GetCandles(baseCandle.CandleType);
                if (candels == null) return null;

                int index = candels.IndexOfValue(baseCandle);
                if (index > 0)
                    return candels.Values[index - 1];
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return null;
        }

        private SortedList<DateTime, Candle> GetCandles(CandleTypes type) // Sample code
        {
            switch (type)
            {
                case CandleTypes.Tick:  return TickCandles;
                case CandleTypes.Min:   return MinCandles;
                case CandleTypes.Day:   return DayCandles;
                case CandleTypes.Week:  return WeekCandles;
                case CandleTypes.Month: return MonthCandles;
                default:                return null;
            }
        }
    }
}
