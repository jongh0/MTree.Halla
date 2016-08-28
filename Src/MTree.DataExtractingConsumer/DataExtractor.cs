﻿using MTree.Consumer;
using MTree.DataStructure;
using MTree.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using TicTacTec.TA.Library;

namespace MTree.DataExtractingConsumer
{
    public class DataExtractor
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly string delimeter = ",";

        private string extractingPath = null;
        private StockMaster currentMaster = null;

        private Stock stock;
        private Chart dayChart;

        private List<float> priceList = new List<float>();

        private int numOfCandles;
        private float[] open;
        private float[] high;
        private float[] low;
        private float[] close;
        private float[] volume;

        private int outBegIdx;
        private int outNBElement = 0;

        #region Queue Task
        public CancellationToken QueueTaskCancelToken { get; set; }
        #endregion

        private ConsumerBase Consumer { get; set; }

        public DataExtractor(ConsumerBase consumer)
        {
            try
            {
                Consumer = consumer;

                Consumer.ConsumeStockMasterEvent += ConsumeStockMaster;                
                
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public void StartExtract(string path)
        {
            try
            {
                extractingPath = path;
                using (var fs = File.Open(extractingPath, FileMode.Create))
                using (var sw = new StreamWriter(fs))
                {
                    WriteHeader(sw);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            TaskUtility.Run("Dashboard.StockConclusionQueue", QueueTaskCancelToken, ProcessStockConclusionQueue);
        }

        private void SplitCandles(Candle[] candles, ref float[] open, ref float[] high, ref float[] low, ref float[] close, ref float[] volume)
        {
            open = new float[candles.Length];
            high = new float[candles.Length];
            low = new float[candles.Length];
            close = new float[candles.Length];
            volume = new float[candles.Length];

            try
            {
                for (int i = 0; i < candles.Length; i++)
                {
                    open[i] = candles[i].Open;
                    high[i] = candles[i].High;
                    low[i] = candles[i].Low;
                    close[i] = candles[i].Close;
                    volume[i] = candles[i].Volume;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
        
        private void InitCandles()
        {
            Candle[] candles = new Candle[dayChart.Candles.Count];
            dayChart.Candles.Values.CopyTo(candles, 0);

            // 전날까지의 Candle + 당일 Candle을 위한 Buffer(1)
            numOfCandles = candles.Length + 1;
            
            open = new float[numOfCandles];
            high = new float[numOfCandles];
            low = new float[numOfCandles];
            close = new float[numOfCandles];
            volume = new float[numOfCandles];

            SplitCandles(candles, ref open, ref high, ref low, ref close, ref volume);
        }

        public void ConsumeStockMaster(List<StockMaster> stockMasters)
        {
            try
            {
                if (stockMasters.Count == 1)
                {
                    currentMaster = stockMasters[0];
                    if (stock == null)
                        stock = Stock.GetStock(stockMasters[0].Code);
                    dayChart = stock.GetChart(ChartTypes.Day, currentMaster.Time.AddMonths(-2), currentMaster.Time);
                    dayChart.WaitInitialing();
                    InitCandles();
                }
                else
                {
                    logger.Error(new NotImplementedException());
                }

            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private void ProcessStockConclusionQueue()
        {
            try
            {
                StockConclusion conclusion;
                if (Consumer.StockConclusionQueue.TryDequeue(out conclusion) == true)
                {
                    if (conclusion.MarketTimeType == MarketTimeTypes.Normal)
                    {
                        using (var fs = File.Open(extractingPath, FileMode.Append))
                        using (var sw = new StreamWriter(fs))
                        {
                            WriteContent(sw, conclusion, currentMaster);
                        }
                    }
                }
                else
                {
                    Thread.Sleep(10);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private void WriteHeader(StreamWriter sw)
        {
            List<string> columns = new List<string>();

            try
            {
                foreach (var field in Enum.GetValues(typeof(StockConclusionField)))
                {
                    columns.Add(field.ToString());
                }

                foreach (var field in Enum.GetValues(typeof(StockMasterField)))
                {
                    columns.Add(field.ToString());
                }

                foreach (var field in Enum.GetValues(typeof(StockTAField)))
                {
                    columns.Add(field.ToString());
                }
                
                sw.WriteLine(string.Join(delimeter, columns));
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                columns.Clear();
            }
        }

        private void WriteContent(StreamWriter sw, StockConclusion conclusion, StockMaster master)
        {
            if (conclusion == null || master == null) return;

            List<string> columns = new List<string>();
            try
            {
                priceList.Add(conclusion.Price);
                stock.UpdateLastConclusion(conclusion);

                high[high.Length - 1] = stock.High;
                low[low.Length - 1] = stock.Low;
                close[close.Length - 1] = stock.Close;

                foreach (var field in Enum.GetValues(typeof(StockConclusionField)))
                {
                    var property = conclusion.GetType().GetProperty(field.ToString());
                    object value = property.GetValue(conclusion);

                    columns.Add(GetNormalizedValue(value));
                }

                foreach (var field in Enum.GetValues(typeof(StockMasterField)))
                {
                    var property = master.GetType().GetProperty(field.ToString());
                    object value = property.GetValue(master);

                    columns.Add(GetNormalizedValue(value));
                }

                foreach (var field in Enum.GetValues(typeof(StockTAField)))
                {
                    var strArr = field.ToString().Split('_');
                    var fieldName = strArr[0];
                    var chartType = strArr.Length > 1 ? (ChartTypes)Enum.Parse(typeof(ChartTypes), strArr[1]) : ChartTypes.Day;
                    var term = strArr.Length > 2 ? int.Parse(strArr[2]) : 0;

                    if (fieldName == "MovingAverage")
                    {
                        var maType = strArr.Length > 3 ? (Core.MAType)Enum.Parse(typeof(Core.MAType), strArr[3]) : Core.MAType.Sma;

                        if (chartType == ChartTypes.Tick)
                            GetTickMovingAverage(term, maType, ref columns);
                        else if (chartType == ChartTypes.Day)
                            GetTickMovingAverage(term, maType, ref columns);
                    }
                    else if (fieldName == "AccumulationDistributionLine")
                        GetAdl(ref columns);
                    else if (fieldName == "AccumulationDistributionOscillator")
                    {
                        var longTerm = strArr.Length > 3 ? int.Parse(strArr[3]) : 0;
                        GetAdOsc(term, longTerm, ref columns);
                    }
                    else if (fieldName == "AbsolutePriceOscillator")
                    {
                        var longTerm = strArr.Length > 3 ? int.Parse(strArr[3]) : 0;
                        var maType = strArr.Length > 4 ? (Core.MAType)Enum.Parse(typeof(Core.MAType), strArr[4]) : Core.MAType.Sma;
                        GetAPO(term, longTerm, maType, ref columns);
                    }
                    else if (fieldName == "Aroon")
                    {
                        var upDown = strArr.Length > 3 ? strArr[3] : "Up";
                        bool isUp = (upDown == "Up");
                        GetAroon(term, ref columns, isUp);
                    }
                    else if (fieldName == "AroonOsc")
                        GetAroonOsc(term, ref columns);
                    else if (fieldName == "AverageTrueRange")
                        GetAtr(term, ref columns);
                    else if (fieldName == "AverageDirectionalIndexRating")
                        GetAdxr(term, ref columns);
                    else if (fieldName == "AverageDirectionalIndex")
                        GetAdx(term, ref columns);
                    else if (fieldName == "MinusDI")
                        GetMinusDI(term, ref columns);
                    else if (fieldName == "PlusDI")
                        GetPlusDI(term, ref columns);
                    else if (fieldName == "MinusDM")
                        GetMinusDM(term, ref columns);
                    else if (fieldName == "PlusDM")
                        GetPlusDM(term, ref columns);
                }
                sw.WriteLine(string.Join(delimeter, columns));
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                columns.Clear();
            }
        }

        private void GetTickMovingAverage(int term, Core.MAType maType, ref List<string> columns)
        {
            int elementCount = term;
            do
            {
                if (priceList.Count < elementCount)
                {
                    columns.Add("0");
                    break;
                }

                var inReal = priceList.GetRange(priceList.Count - elementCount, elementCount).ToArray();
                var outReal = new double[elementCount];

                Core.MovingAverage(0, inReal.Length - 1, inReal, term, maType, out outBegIdx, out outNBElement, outReal);
                if (outNBElement != 0)
                    columns.Add(outReal[0].ToString());
                else
                    elementCount++;
            } while (outNBElement == 0);
        }
        private void GetDayMovingAverage(int term, Core.MAType maType, ref List<string> columns)
        {
            var outReal = new double[dayChart.Candles.Count];
            Core.MovingAverage(0, dayChart.Candles.Count - 1, close, term, maType, out outBegIdx, out outNBElement, outReal);
            columns.Add(outReal[outNBElement - 1].ToString());
        }
        private void GetAdl(ref List<string> columns)
        {
            var outReal = new double[dayChart.Candles.Count];
            Core.Ad(0, dayChart.Candles.Count - 1, high, low, close, volume, out outBegIdx, out outNBElement, outReal);
            columns.Add(outReal[outNBElement - 1].ToString());
        }

        private void GetAdOsc(int fastPeriod, int slowPeriod, ref List<string> columns)
        {
            var outReal = new double[dayChart.Candles.Count];
            Core.AdOsc(0, dayChart.Candles.Count - 1, high, low, close, volume, fastPeriod, slowPeriod, out outBegIdx, out outNBElement, outReal);
            columns.Add(outReal[outNBElement - 1].ToString());
        }
        private void GetAPO(int fastPeriod, int slowPeriod, Core.MAType maType, ref List<string> columns)
        {
            var outReal = new double[dayChart.Candles.Count];
            Core.Apo(0, dayChart.Candles.Count - 1, close, fastPeriod, slowPeriod, maType, out outBegIdx, out outNBElement, outReal);
            columns.Add(outReal[outNBElement - 1].ToString());
        }
        private void GetAroon(int term,  ref List<string> columns, bool isUp)
        {
            var outDownReal = new double[dayChart.Candles.Count];
            var outUpReal = new double[dayChart.Candles.Count];
            Core.Aroon(0, dayChart.Candles.Count - 1, high, low, term, out outBegIdx, out outNBElement, outDownReal, outUpReal);
            if(isUp == true)
                columns.Add(outDownReal[outNBElement - 1].ToString());
            else
                columns.Add(outUpReal[outNBElement - 1].ToString());
        }
        private void GetAroonOsc(int term, ref List<string> columns)
        {
            var outReal = new double[dayChart.Candles.Count];            
            Core.AroonOsc(0, dayChart.Candles.Count - 1, high, low, term, out outBegIdx, out outNBElement, outReal);
            columns.Add(outReal[outNBElement - 1].ToString());
            
        }

        private void GetAtr(int term, ref List<string> columns)
        {
            var outReal = new double[dayChart.Candles.Count];
            Core.Atr(0, dayChart.Candles.Count - 1, high, low, close, term, out outBegIdx, out outNBElement, outReal);
            columns.Add(outReal[outNBElement - 1].ToString());
        }

        private void GetAdxr(int term, ref List<string> columns)
        {
            var outReal = new double[dayChart.Candles.Count];
            Core.Adxr(0, dayChart.Candles.Count - 1, high, low, close, term, out outBegIdx, out outNBElement, outReal);
            columns.Add(outReal[outNBElement - 1].ToString());
        }

        private void GetAdx(int term, ref List<string> columns)
        {
            var outReal = new double[dayChart.Candles.Count];            
            Core.Adx(0, dayChart.Candles.Count - 1, high, low, close, term, out outBegIdx, out outNBElement, outReal);
            columns.Add(outReal[outNBElement - 1].ToString());
        }

        private void GetMinusDI(int term, ref List<string> columns)
        {
            var outReal = new double[dayChart.Candles.Count];
            Core.MinusDI(0, dayChart.Candles.Count - 1, high, low, close, term, out outBegIdx, out outNBElement, outReal);
            columns.Add(outReal[outNBElement - 1].ToString());
        }

        private void GetPlusDI(int term, ref List<string> columns)
        {
            var outReal = new double[dayChart.Candles.Count];
            Core.PlusDI(0, dayChart.Candles.Count - 1, high, low, close, term, out outBegIdx, out outNBElement, outReal);
            columns.Add(outReal[outNBElement - 1].ToString());
        }

        private void GetMinusDM(int term, ref List<string> columns)
        {
            var outReal = new double[dayChart.Candles.Count];
            Core.MinusDM(0, dayChart.Candles.Count - 1, high, low, term, out outBegIdx, out outNBElement, outReal);
            columns.Add(outReal[outNBElement - 1].ToString());
        }

        private void GetPlusDM(int term, ref List<string> columns)
        {
            var outReal = new double[dayChart.Candles.Count];
            Core.PlusDM(0, dayChart.Candles.Count - 1, high, low, term, out outBegIdx, out outNBElement, outReal);
            columns.Add(outReal[outNBElement - 1].ToString());
        }

        private string GetNormalizedValue(object value)
        {
            Type type = value.GetType();

            if (type == typeof(DateTime))
            {
                DateTime dateTime = (DateTime)value;
                return dateTime.Ticks.ToString();
            }
            else if (type == typeof(bool))
            {
                return Convert.ToInt32(value).ToString();
            }
            else if (type.IsEnum)
            {
                return ((int)value).ToString();
            }
            else
            {
                return value.ToString();
            }
        }
    }
}
