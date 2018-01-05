using MongoDB.Driver;
using MTree.Configuration;
using MTree.DataStructure;
using MTree.DbProvider;
using MTree.Utility;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTree.Consumer
{
    public class Chart
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public string Code { get; private set; }

        public ChartTypes ChartType { get; private set; }
        
        public DateTime StartDate { get; private set; }

        public DateTime EndDate { get; private set; }

        public int ChartInterval { get; private set; }

        public bool IsInitializing { get; private set; }

        private ManualResetEvent WaitInitializingEvent { get; set; } = new ManualResetEvent(false);
        
        public SortedList<DateTime, Candle> Candles { get; private set; } = new SortedList<DateTime, Candle>();

        private DataLoader dataLoader = new DataLoader();

        public Chart(string code, ChartTypes chartType, DateTime startDate, DateTime endDate, int interval = 1)
        {
            Code = code;
            ChartType = chartType;
            SetRange(startDate, endDate);
            ChartInterval = interval;
            FillCandles();
        }

        /// <summary>
        /// Chart 범위 설정을 다시 한다.
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        public void SetRange(DateTime startDate, DateTime endDate)
        {
            try
            {
                if (startDate == null) startDate = Config.General.DefaultStartDate;
                if (endDate == null) endDate = DateTime.Now;

                StartDate = DateTimeUtility.StartDateTime(startDate);
                EndDate = DateTimeUtility.EndDateTime(endDate);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        /// <summary>
        /// Candle이 다 채워질 때 까지 대기한다.
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public bool WaitInitializing(int timeout = Timeout.Infinite)
        {
            if (IsInitializing == false) return true;

            return WaitInitializingEvent.WaitOne(timeout);
        }

        /// <summary>
        /// Chart를 새로 갱신한다. 기존 데이터는 사라진다.
        /// </summary>
        private async void FillCandles()
        {
            try
            {
                IsInitializing = true;
                WaitInitializingEvent.Reset();

                SortedList<DateTime, Candle> temp;

                if (ChartType == ChartTypes.Tick || ChartType == ChartTypes.Min)
                {
                    await ExtractTickCandles();

                    if (ChartType == ChartTypes.Tick && ChartInterval > 1)
                    {
                        temp = ChartConverter.ConvertToTickChart(Candles, ChartInterval);
                        Candles.Clear();
                        Candles = temp;
                    }
                    else if (ChartType == ChartTypes.Min)
                    {
                        temp = ChartConverter.ConvertToMinChart(Candles, ChartInterval);
                        Candles.Clear();
                        Candles = temp;
                    }
                }
                else
                {
                    await ExtractDayCandles();

                    if (ChartType == ChartTypes.Week)
                    {
                        temp = ChartConverter.ConvertToWeekChart(Candles);
                        Candles.Clear();
                        Candles = temp;
                    }
                    else if (ChartType == ChartTypes.Month)
                    {
                        temp = ChartConverter.ConvertToMonthChart(Candles);
                        Candles.Clear();
                        Candles = temp;
                    }
                    else if (ChartType == ChartTypes.Year)
                    {
                        temp = ChartConverter.ConvertToYearChart(Candles);
                        Candles.Clear();
                        Candles = temp;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                IsInitializing = false;
                WaitInitializingEvent.Set();
            }
        }

        /// <summary>
        /// DB로부터 Day Type Chart 불러온다. Async로 동작하며 Initializing, WaitInitializing()를 사용해서 동작중인지 확인해야 한다
        /// </summary>
        private async Task ExtractDayCandles()
        { 
            int startTick = Environment.TickCount;

            try
            {
                // Candle 리스트 초기화
                Candles.Clear();

                // CandleType, Time으로 Query 생성
                var builder = Builders<Candle>.Filter;
                var filter = builder.Eq(i => i.CandleType, CandleTypes.Day) &
                             builder.Gte(i => i.Time, StartDate) &
                             builder.Lte(i => i.Time, EndDate);

                // Async Query 수행
                var result = await DbAgent.Instance.Find(Code, filter).ToListAsync();

                // Candle 리스트에 삽입
                foreach (var candle in result)
                {
                    if (Candles.ContainsKey(candle.Time) == false)
                        Candles.Add(candle.Time, candle);
                    else
                        logger.Warn($"Already exists in candle list, {candle.Code}/{candle.Time.ToString(Config.General.DateTimeFormat)}");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                var duration = Environment.TickCount - startTick;
                logger.Info($"Candle list filled, Tick: {duration}, {this.ToString()}");
            }
        }

        /// <summary>
        /// DB로부터 Tick Type Chart을 불러온다.
        /// </summary>
        private async Task ExtractTickCandles()
        {
            try
            {
                // Candle 리스트 초기화
                Candles.Clear();
                var builder = Builders<StockConclusion>.Filter;
                var filter = builder.Gte(i => (i as Subscribable).Time, StartDate) & builder.Lte(i => (i as Subscribable).Time, EndDate);
                
                var result = await DbAgent.Instance.Find(Code, filter).ToListAsync();
                
                // Candle 리스트에 삽입
                foreach (var conclusion in result)
                {
                    if (conclusion.MarketTimeType != MarketTimeTypes.Normal)
                        continue;

                    var candle = ChartConverter.ConvertToTickCandle(conclusion);

                    while (Candles.ContainsKey(candle.Time) == true)
                        candle.Time = candle.Time.AddMilliseconds(1);

                    Candles.Add(candle.Time, candle);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                logger.Info($"Candle list filled, {this.ToString()}");
            }
        }

        public Candle CandleAt(DateTime dateTime)
        {
            if (dateTime == null) return null;

            int startTick = Environment.TickCount;

            try
            {
                // Sec, Milisecond는 버리고 검색한다
                dateTime = new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0, 0);

                int index = Candles.IndexOfKey(dateTime);
                if (index != -1)
                    return Candles.Values[index];

                // Index로 못 찾으면 해당 시간 바로 이전의 Candle을 리턴한다
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
                    logger.Error($"Candle search take long time, {Code}/{ChartType}/{dateTime.ToString(Config.General.DateTimeFormat)}");
            }

            logger.Warn($"Can not find candle at {Code}/{dateTime.ToString(Config.General.DateTimeFormat)}");
            return null;
        }

        public Candle NextCandle(Candle baseCandle)
        {
            if (baseCandle == null) return null;

            try
            {
                // Index로 검색해서 바로 다음 Index Candle을 리턴한다
                int index = Candles.IndexOfValue(baseCandle);
                if (index >= 0 && Candles.Count > index + 1)
                    return Candles.Values[index + 1];
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            logger.Warn($"Can not find candle next {baseCandle.Code}/{baseCandle.Time.ToString(Config.General.DateTimeFormat)}");
            return null;
        }

        public Candle PrevCandle(Candle baseCandle)
        {
            if (baseCandle == null) return null;

            try
            {
                // Index로 검색해서 바로 이전 Index Candle을 리턴한다
                int index = Candles.IndexOfValue(baseCandle);
                if (index > 0)
                    return Candles.Values[index - 1];
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            logger.Warn($"Can not find candle prev {baseCandle.Code}/{baseCandle.Time.ToString(Config.General.DateTimeFormat)}");
            return null;
        }

        public override string ToString()
        {
            return $"{Code}/{ChartType}/{StartDate.ToString(Config.General.DateTimeFormat)}/{EndDate.ToString(Config.General.DateTimeFormat)}/{Candles.Count}";
        }
    }
}
