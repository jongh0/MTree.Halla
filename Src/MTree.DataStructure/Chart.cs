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

    [Serializable]
    public class Chart
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public string Code { get; set; }

        public ChartTypes ChartType { get; set; } = ChartTypes.Tick;

        public bool IsAdjusted { get; set; } = false;

        public SortedList<DateTime, Candle> TickCandles { get; set; }

        public SortedList<DateTime, Candle> MinCandles { get; set; }

        public SortedList<DateTime, Candle> DayCandles { get; set; }

        public SortedList<DateTime, Candle> WeekCandles { get; set; }

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

        public static char ConvertToChar(ChartTypes type)
        {
            switch (type)
            {
                case ChartTypes.Min:    return 'm';
                case ChartTypes.Day:    return 'D';
                case ChartTypes.Week:   return 'W';
                case ChartTypes.Month:  return 'M';
                default: return         'T';
            }
        }

        public static ChartTypes ConvertToChartType(char type)
        {
            switch (type)
            {
                case 'm': return        ChartTypes.Min;
                case 'D': return        ChartTypes.Day;
                case 'W': return        ChartTypes.Week;
                case 'M': return        ChartTypes.Month;
                default:                return ChartTypes.Tick;
            }
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
    }
}
