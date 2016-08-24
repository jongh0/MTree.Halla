﻿using MTree.Configuration;
using MTree.DataStructure;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.Consumer
{
    public static class ChartConverter
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private static void UpdateCandlePrice(ref Candle targetCandle, Candle newCandle)
        {
            targetCandle.Volume += newCandle.Volume;
            targetCandle.Value += newCandle.Value;
            if (newCandle.Volume == 0)
            {
                targetCandle.Open = targetCandle.High = targetCandle.Low = targetCandle.Close = newCandle.Open;
            }
            else
            {
                if (targetCandle.High < newCandle.High)
                    targetCandle.High = newCandle.High;
                if (targetCandle.Low > newCandle.Low)
                    targetCandle.Low = newCandle.Low;
                targetCandle.Close = newCandle.Close;
            }
        }

        public static Candle ConvertToTickCandle(Conclusion conclusion)
        {
            Candle candle = new Candle();

            candle.CandleType = CandleTypes.Tick;
            candle.Time = conclusion.Time;
            candle.Volume = (ulong)conclusion.Amount;
            candle.Value = (ulong)(conclusion.Price * conclusion.Amount);
            candle.Open = candle.High = candle.Low = candle.Close = conclusion.Price;

            return candle;
        }

        public static SortedList<DateTime, Candle> ConvertToTickChart(SortedList<DateTime, Candle> original, int interval)
        {
            SortedList<DateTime, Candle> retCandles = new SortedList<DateTime, Candle>();
            int candleIndex = 0;

            do
            {
                if (original.Values[candleIndex].CandleType > CandleTypes.Tick)
                {
                    logger.Error("Converting chart scope should be greater than orignal");
                    return null;
                }

                Candle candle = new Candle();
                candle.CandleType = CandleTypes.Tick;
                candle.Time = original.Values[candleIndex].Time;

                for (int i = 0; i < interval; i++)
                {
                    UpdateCandlePrice(ref candle, original.Values[candleIndex]);
                    candleIndex++;
                    if (candleIndex >= original.Count)
                        break;
                    if (candle.ToString(Config.General.DateFormat) != original.Values[candleIndex].Time.ToString(Config.General.DateFormat))
                        break;
                }
                retCandles.Add(candle.Time, candle);
            } while (candleIndex < original.Count);

            return retCandles;
        }

        public static SortedList<DateTime, Candle> ConvertToMinChart(SortedList<DateTime, Candle> original, int interval)
        {
            SortedList<DateTime, Candle> retCandles = new SortedList<DateTime, Candle>();
            int candleIndex = 0;

            do
            {
                if (original.Values[candleIndex].CandleType > CandleTypes.Min)
                {
                    logger.Error("Converting chart scope should be greater than orignal");
                    return null;
                }
                Candle candle = new Candle();

                candle.CandleType = CandleTypes.Min;
                DateTime baseTime = original.Values[candleIndex].Time;
                candle.Time = new DateTime(baseTime.Year, baseTime.Month, baseTime.Day,
                                           baseTime.Hour, (baseTime.Minute / interval) * interval, 0); // N의 배수 중 Time 보다 작은 최대 값 찾기

                do
                {
                    UpdateCandlePrice(ref candle, original.Values[candleIndex]);
                    candleIndex++;

                    if (candleIndex >= original.Count)
                        break;
                    if (candle.ToString(Config.General.DateFormat) != original.Values[candleIndex].Time.ToString(Config.General.DateFormat))
                        break;
                } while (original.Values[candleIndex].Time < candle.Time.AddMinutes(interval));
                retCandles.Add(candle.Time, candle);
            } while (candleIndex < original.Count);
            return retCandles;
        }

        public static SortedList<DateTime, Candle> ConvertToDayChart(SortedList<DateTime, Candle> original)
        {
            SortedList<DateTime, Candle> retCandles = new SortedList<DateTime, Candle>();
            int candleIndex = 0;

            do
            {
                if (original.Values[candleIndex].CandleType > CandleTypes.Day)
                {
                    logger.Error("Converting chart scope should be greater than orignal");
                    return null;
                }
                Candle candle = new Candle();
                candle.CandleType = CandleTypes.Day;
                candle.Time = original.Values[candleIndex].Time;

                do
                {
                    UpdateCandlePrice(ref candle, original.Values[candleIndex]);
                    candleIndex++;
                    if (candleIndex >= original.Count)
                        break;
                } while (candle.Time.Day == original.Values[candleIndex].Time.Day);
                retCandles.Add(candle.Time, candle);
            } while (candleIndex < original.Count);

            return retCandles;
        }

        public static SortedList<DateTime, Candle> ConvertToWeekChart(SortedList<DateTime, Candle> original)
        {
            DateTimeFormatInfo dfi = DateTimeFormatInfo.CurrentInfo;
            Calendar cal = dfi.Calendar;

            SortedList<DateTime, Candle> retCandles = new SortedList<DateTime, Candle>();
            int candleIndex = 0;

            do
            {
                if (original.Values[candleIndex].CandleType > CandleTypes.Week)
                {
                    logger.Error("Converting chart scope should be greater than orignal");
                    return null;
                }
                Candle candle = new Candle();
                candle.CandleType = CandleTypes.Week;
                candle.Time = original.Values[candleIndex].Time;

                do
                {
                    UpdateCandlePrice(ref candle, original.Values[candleIndex]);
                    candleIndex++;
                    if (candleIndex >= original.Count)
                        break;
                } while (cal.GetWeekOfYear(original.Values[candleIndex].Time, dfi.CalendarWeekRule, dfi.FirstDayOfWeek) ==
                         cal.GetWeekOfYear(candle.Time, dfi.CalendarWeekRule, dfi.FirstDayOfWeek));
                retCandles.Add(candle.Time, candle);
            } while (candleIndex < original.Count);

            return retCandles;
        }

        public static SortedList<DateTime, Candle> ConvertToMonthChart(SortedList<DateTime, Candle> original)
        {
            SortedList<DateTime, Candle> retCandles = new SortedList<DateTime, Candle>();
            int candleIndex = 0;

            do
            {
                if (original.Values[candleIndex].CandleType > CandleTypes.Month)
                {
                    logger.Error("Converting chart scope should be greater than orignal");
                    return null;
                }
                Candle candle = new Candle();

                candle.CandleType = CandleTypes.Month;
                candle.Time = original.Values[candleIndex].Time;

                do
                {
                    UpdateCandlePrice(ref candle, original.Values[candleIndex]);
                    candleIndex++;
                    if (candleIndex >= original.Count)
                        break;
                } while (candle.Time.Month == original.Values[candleIndex].Time.Month);
                retCandles.Add(candle.Time, candle);
            } while (candleIndex < original.Count);

            return retCandles;
        }

        public static SortedList<DateTime, Candle> ConvertToYearChart(SortedList<DateTime, Candle> original)
        {
            SortedList<DateTime, Candle> retCandles = new SortedList<DateTime, Candle>();
            int candleIndex = 0;

            do
            {
                if (original.Values[candleIndex].CandleType > CandleTypes.Month)
                {
                    logger.Error("Converting chart scope should be greater than orignal");
                    return null;
                }

                Candle candle = new Candle();

                candle.CandleType = CandleTypes.Year;
                candle.Time = original.Values[candleIndex].Time;

                do
                {
                    UpdateCandlePrice(ref candle, original.Values[candleIndex]);
                    candleIndex++;
                    if (candleIndex >= original.Count)
                        break;
                } while (candle.Time.Year == original.Values[candleIndex].Time.Year);
                retCandles.Add(candle.Time, candle);
            } while (candleIndex < original.Count);

            return retCandles;
        }

        public static CandleTypes ConvertChartTypeToCandleType(ChartTypes chartType)
        {
            switch (chartType)
            {
                case ChartTypes.Min: return CandleTypes.Min;
                case ChartTypes.Day: return CandleTypes.Day;
                case ChartTypes.Week: return CandleTypes.Week;
                case ChartTypes.Month: return CandleTypes.Month;
                default: return CandleTypes.Tick;
            }
        }
    }
}