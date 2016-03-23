using MongoDB.Driver;
using MTree.Configuration;
using MTree.DataStructure;
using MTree.DbProvider;
using MTree.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

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

        public string Code { get; private set; }

        public ChartTypes ChartType { get; private set; }

        public DateTime StartDate { get; private set; }

        public DateTime EndDate { get; private set; }

        public bool Initializing { get; private set; }

        private ManualResetEvent WaitInitializingEvent { get; set; } = new ManualResetEvent(false);

        public SortedList<DateTime, Candle> Candles { get; private set; } = new SortedList<DateTime, Candle>();

        public Chart(ChartTypes chartType, DateTime startDate, DateTime endDate)
        {
            ChartType = chartType;
            SetRange(startDate, endDate);
        }

        public void SetRange(DateTime startDate, DateTime endDate)
        {
            try
            {
                if (startDate == null) startDate = Config.Instance.General.DefaultStartDate;
                if (endDate == null) endDate = DateTime.Now;

                StartDate = DateTimeUtility.StartDateTime(startDate);
                EndDate = DateTimeUtility.EndDateTime(endDate);

                FillCandles();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public bool WaitInitialing(int timeout = Timeout.Infinite)
        {
            if (Initializing == false) return true;

            return WaitInitializingEvent.WaitOne(timeout);
        }

        private async void FillCandles()
        {
            int startTick = Environment.TickCount;

            try
            {
                Initializing = true;
                WaitInitializingEvent.Reset();

                Candles.Clear();

                IMongoCollection<Candle> collection = MongoDbProvider.Instance.GetDatabase(DbTypes.Chart).GetCollection<Candle>(Code);

                var builder = Builders<Candle>.Filter;
                var filter = builder.Gte(i => i.Time, StartDate) & builder.Lte(i => i.Time, EndDate);
                var result = await collection.Find(filter).ToListAsync();

                foreach (var candle in result)
                {
                    if (Candles.ContainsKey(candle.Time) == false)
                        Candles.Add(candle.Time, candle);
                    else
                        logger.Warn($"Already exists in candle list, {candle.Code}/{candle.Time.ToString(Config.Instance.General.DateTimeFormat)}");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                Initializing = false;
                WaitInitializingEvent.Set();

                var duration = Environment.TickCount - startTick;
                logger.Info($"Candle list filled, Tick: {duration}, {this.ToString()}");
            }
        }

        public Candle CandleAt(DateTime dateTime)
        {
            if (dateTime == null) return null;

            int startTick = Environment.TickCount;

            try
            {
                dateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0, 0); // Sec, Milisecond는 버린다

                int index = Candles.IndexOfKey(dateTime);
                if (index != -1)
                    return Candles.Values[index];

                for (index = Candles.Count - 1; index >= 0; index--)
                {
                    var candle = Candles.Values[index];
                    if (candle.Time <= dateTime)
                        return candle;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                var duration = Environment.TickCount - startTick;
                if (duration > 100)
                    logger.Error($"Candle search take long time, {Code}/{ChartType}/{dateTime.ToString(Config.Instance.General.DateTimeFormat)}");
            }

            logger.Warn($"Can not find candle at {Code}/{dateTime.ToString(Config.Instance.General.DateTimeFormat)}");
            return null;
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

            logger.Warn($"Can not find candle next {baseCandle.Code}/{baseCandle.Time.ToString(Config.Instance.General.DateTimeFormat)}");
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

            logger.Warn($"Can not find candle prev {baseCandle.Code}/{baseCandle.Time.ToString(Config.Instance.General.DateTimeFormat)}");
            return null;
        }
        
        public override string ToString()
        {
            return $"{Code}/{ChartType}/{StartDate.ToString(Config.Instance.General.DateTimeFormat)}/{EndDate.ToString(Config.Instance.General.DateTimeFormat)}/{Candles.Count}";
        }
    }
}
