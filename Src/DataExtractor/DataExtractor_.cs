using Consumer;
using DataStructure;
using RealTimeProvider;
using CommonLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using TicTacTec.TA.Library;
using static TicTacTec.TA.Library.Core;

namespace DataExtractor
{
    delegate RetCode TADelegate_C_Int(int startIdx, int endIdx, float[] inReal, out int outBegIdx, out int outNBElement, int[] outReal);
    delegate RetCode TADelegate_C_Double(int startIdx, int endIdx, float[] inReal, out int outBegIdx, out int outNBElement, double[] outReal);
    delegate RetCode TADelegate_CT_Double(int startIdx, int endIdx, float[] inReal, int optInTimePeriod, out int outBegIdx, out int outNBElement, double[] outReal);
    delegate RetCode TADelegate_CT_Int(int startIdx, int endIdx, float[] inReal, int optInTimePeriod, out int outBegIdx, out int outNBElement, int[] outReal);
    delegate RetCode TADelegate_CTD_Double(int startIdx, int endIdx, float[] inReal, int optInTimePeriod, double optInNbDev, out int outBegIdx, out int outNBElement, double[] outReal);
    delegate RetCode TADelegate_CV_Double(int startIdx, int endIdx, float[] inReal, float[] inVolume, out int outBegIdx, out int outNBElement, double[] outReal);
    delegate RetCode TADelegate_CTM_Double(int startIdx, int endIdx, float[] inReal, int optInTimePeriod, MAType optInMAType, out int outBegIdx, out int outNBElement, double[] outReal);
    delegate RetCode TADelegate_HL_Double(int startIdx, int endIdx, float[] inHigh, float[] inLow, out int outBegIdx, out int outNBElement, double[] outReal);
    delegate RetCode TADelegate_HLAM_Double(int startIdx, int endIdx, float[] inHigh, float[] inLow, double optInAcceleration, double optInMaximum, out int outBegIdx, out int outNBElement, double[] outReal);
    delegate RetCode TADelegate_HLC_Double(int startIdx, int endIdx, float[] inHigh, float[] inLow, float[] inClose, out int outBegIdx, out int outNBElement, double[] outReal);
    delegate RetCode TADelegate_HLCV_Double(int startIdx, int endIdx, float[] inHigh, float[] inLow, float[] inClose, float[] inVolume, out int outBegIdx, out int outNBElement, double[] outReal);
    delegate RetCode TADelegate_HLCVT_Double(int startIdx, int endIdx, float[] inHigh, float[] inLow, float[] inClose, float[] inVolume, int optInTimePeriod, out int outBegIdx, out int outNBElement, double[] outReal);
    delegate RetCode TADelegate_HLCVTT_Double(int startIdx, int endIdx, float[] inHigh, float[] inLow, float[] inClose, float[] inVolume, int optInFastPeriod, int optInSlowPeriod, out int outBegIdx, out int outNBElement, double[] outReal);
    delegate RetCode TADelegate_CTTM_Double(int startIdx, int endIdx, float[] inReal, int optInFastPeriod, int optInSlowPeriod, MAType optInMAType, out int outBegIdx, out int outNBElement, double[] outReal);
    delegate RetCode TADelegate_OHLC_Int(int startIdx, int endIdx, float[] inOpen, float[] inHigh, float[] inLow, float[] inClose, out int outBegIdx, out int outNBElement, int[] outInteger);
    delegate RetCode TADelegate_OHLC_Double(int startIdx, int endIdx, float[] inOpen, float[] inHigh, float[] inLow, float[] inClose, out int outBegIdx, out int outNBElement, double[] outReal);
    delegate RetCode TADelegate_OHLCP_Int(int startIdx, int endIdx, float[] inOpen, float[] inHigh, float[] inLow, float[] inClose, double optInPenetration, out int outBegIdx, out int outNBElement, int[] outInteger);
    delegate RetCode TADelegate_HLCT_Double(int startIdx, int endIdx, float[] inHigh, float[] inLow, float[] inClose, int optInTimePeriod, out int outBegIdx, out int outNBElement, double[] outReal);
    delegate RetCode TADelegate_HLT_Double(int startIdx, int endIdx, float[] inHigh, float[] inLow, int optInTimePeriod, out int outBegIdx, out int outNBElement, double[] outReal);
    
    public class DataExtractor_
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

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

        private List<double> minLimit = new List<double>();
        private List<double> maxLimit = new List<double>();

        public bool IncludeTAValues { get; set; }

        #region Queue Task
        public CancellationToken QueueTaskCancelToken { get; set; }
        #endregion

        private ConsumerBase Consumer { get; set; }
        private bool isSubscribingDone = false;
        private ManualResetEvent WaitSubscribingEvent { get; set; } = new ManualResetEvent(false);

        public DataExtractor_(ConsumerBase consumer)
        {
            try
            {
                Consumer = consumer;

                Consumer.ConsumeStockMasterEvent += ConsumeStockMaster;
                Consumer.NotifyMessageEvent += NotifyMessage;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }
        public bool WaitSubscribingDone(int timeout = Timeout.Infinite)
        {
            return WaitSubscribingEvent.WaitOne(timeout);
        }

        private void NotifyMessage(MessageTypes type, string message)
        {
            isSubscribingDone = true;
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
                _logger.Error(ex);
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
                _logger.Error(ex);
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
                _logger.Info("New stock master received");
                isSubscribingDone = false;
                WaitSubscribingEvent.Reset();

                if (stockMasters.Count == 1)
                {
                    currentMaster = stockMasters[0];
                    if (stock == null)
                        stock = Stock.GetStock(stockMasters[0].Code);
                    dayChart = stock.GetChart(ChartTypes.Day, currentMaster.Time.AddMonths(-12), currentMaster.Time);
                    dayChart.WaitInitializing();
                    InitCandles();
                }
                else
                {
                    _logger.Error(new NotImplementedException());
                }

            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private void ProcessStockConclusionQueue()
        {
            try
            {
                if (Consumer.StockConclusionQueue.TryDequeue(out var conclusion) == true)
                {
                    if (conclusion.MarketTimeType == MarketTimeTypes.Normal)
                    {
                        using (var fs = File.Open(extractingPath, FileMode.Append))
                        using (var sw = new StreamWriter(fs))
                        {
                            WriteContent(sw, conclusion, currentMaster);
                        }
                        using (var fs = File.Open(extractingPath.Substring(0, extractingPath.Length - 4) + "_range.csv", FileMode.Create))
                        using (var sw = new StreamWriter(fs))
                        {
                            sw.WriteLine(string.Join(delimeter, minLimit));
                            sw.WriteLine(string.Join(delimeter, maxLimit));
                        }
                    }
                }
                else
                {
                    if (isSubscribingDone == true)
                    {
                        isSubscribingDone = false;
                        WaitSubscribingEvent.Set();

                        _logger.Info("Subscribing completed");
                    }
                    Thread.Sleep(10);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
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

                if (IncludeTAValues == true)
                {
                    foreach (var field in Enum.GetValues(typeof(StockTAField)))
                    {
                        var strArr = field.ToString().Split('_');
                        var fieldName = strArr[0];
                        if (fieldName == "Aroon")
                        {
                            columns.Add(field.ToString() + "_Up");
                            columns.Add(field.ToString() + "_Down");
                        }
                        else if (fieldName == "BollingerBands")
                        {
                            columns.Add(field.ToString() + "_Upper");
                            columns.Add(field.ToString() + "_Middle");
                            columns.Add(field.ToString() + "_Lower");
                        }
                        else if (fieldName == "HilbertTransformPhasorComponents")
                        {
                            columns.Add(field.ToString() + "_InPhase");
                            columns.Add(field.ToString() + "_Quadrature");
                        }
                        else if (fieldName == "HilbertTransformSineWave")
                        {
                            columns.Add(field.ToString() + "_Sine");
                            columns.Add(field.ToString() + "_LeadSine");
                        }
                        else if (fieldName == "MovingAverageConvergenceDivergence")
                        {
                            columns.Add(field.ToString());
                            columns.Add(field.ToString() + "_Signal");
                            columns.Add(field.ToString() + "_Hist");
                        }
                        else if (fieldName == "MacdWithMAType")
                        {
                            columns.Add(field.ToString());
                            columns.Add(field.ToString() + "_Signal");
                            columns.Add(field.ToString() + "_Hist");
                        }
                        else if (fieldName == "Stochastic")
                        {
                            columns.Add(field.ToString() + "_SlowK");
                            columns.Add(field.ToString() + "_SlowD");
                        }
                        else if (fieldName == "StochasticFast")
                        {
                            columns.Add(field.ToString() + "_FastK");
                            columns.Add(field.ToString() + "_FastD");
                        }
                        else if (fieldName == "StochasticRelativeStrengthIndex")
                        {
                            columns.Add(field.ToString() + "_FastK");
                            columns.Add(field.ToString() + "_FastD");
                        }
                        else
                            columns.Add(field.ToString());
                    }
                }

                sw.WriteLine(string.Join(delimeter, columns));
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
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

                if (IncludeTAValues == true)
                {
                    foreach (var field in Enum.GetValues(typeof(StockTAField)))
                    {
                        var strArr = field.ToString().Split('_');
                        var fieldName = strArr[0];
                        var chartType = strArr.Length > 1 ? (ChartTypes)Enum.Parse(typeof(ChartTypes), strArr[1]) : ChartTypes.Day;
                        var term = strArr.Length > 2 ? int.Parse(strArr[2]) : 0;

                        if (fieldName == "MovingAverage")
                        {
                            var maType = strArr.Length > 3 ? (MAType)Enum.Parse(typeof(MAType), strArr[3]) : MAType.Sma;

                            if (chartType == ChartTypes.Tick)
                                GetTickMovingAverage(term, maType, ref columns);
                            else if (chartType == ChartTypes.Day)
                                GetTAValue(MovingAverage, term, maType, ref columns);
                        }
                        else if (fieldName == "AccumulationDistributionLine")
                            GetTAValue((TADelegate_HLCV_Double)Ad, ref columns);
                        else if (fieldName == "AccumulationDistributionOscillator")
                        {
                            var longTerm = strArr.Length > 3 ? int.Parse(strArr[3]) : 0;
                            GetTAValue(AdOsc, term, longTerm, ref columns);
                        }
                        else if (fieldName == "AbsolutePriceOscillator")
                        {
                            var longTerm = strArr.Length > 3 ? int.Parse(strArr[3]) : 0;
                            var maType = strArr.Length > 4 ? (MAType)Enum.Parse(typeof(MAType), strArr[4]) : MAType.Sma;
                            GetTAValue(Apo, term, longTerm, maType, ref columns);
                        }
                        else if (fieldName == "Aroon")
                            GetAroon(term, ref columns);
                        else if (fieldName == "AroonOsc")
                            GetTAValue(AroonOsc, term, ref columns);
                        else if (fieldName == "AverageTrueRange")
                            GetTAValue(Atr, term, ref columns);
                        else if (fieldName == "AverageDirectionalIndexRating")
                            GetTAValue(Adxr, term, ref columns);
                        else if (fieldName == "AverageDirectionalIndex")
                            GetTAValue(Adx, term, ref columns);
                        else if (fieldName == "MinusDI")
                            GetTAValue(MinusDI, term, ref columns);
                        else if (fieldName == "PlusDI")
                            GetTAValue(PlusDI, term, ref columns);
                        else if (fieldName == "MinusDM")
                            GetTAValue(MinusDM, term, ref columns);
                        else if (fieldName == "PlusDM")
                            GetTAValue(PlusDM, term, ref columns);
                        else if (fieldName == "BollingerBands")
                        {
                            var dev = strArr.Length > 3 ? float.Parse(strArr[3]) : 2;
                            var maType = strArr.Length > 4 ? (MAType)Enum.Parse(typeof(MAType), strArr[4]) : MAType.Sma;
                            GetBbands(term, dev, maType, ref columns);
                        }
                        else if (fieldName == "BalanceOfPower")
                            GetTAValue((TADelegate_OHLC_Double)Bop, ref columns);
                        else if (fieldName == "CommodityChannelIndex")
                            GetTAValue(Cci, term, ref columns);
                        else if (fieldName == "TwoCrows")
                            GetTAValue(Cdl2Crows, ref columns);
                        else if (fieldName == "ThreeBlackCrows")
                            GetTAValue(Cdl3BlackCrows, ref columns);
                        else if (fieldName == "ThreeInsideUpDown")
                            GetTAValue(Cdl3Inside, ref columns);
                        else if (fieldName == "ThreeLineStrike")
                            GetTAValue(Cdl3LineStrike, ref columns);
                        else if (fieldName == "ThreeOutsideUpDown")
                            GetTAValue(Cdl3Outside, ref columns);
                        else if (fieldName == "ThreeStarsInTheSouth")
                            GetTAValue(Cdl3StarsInSouth, ref columns);
                        else if (fieldName == "ThreeAdvancingWhiteSoldiers")
                            GetTAValue(Cdl3WhiteSoldiers, ref columns);
                        else if (fieldName == "AbandonedBaby")
                        {
                            double penetration = strArr.Length > 2 ? double.Parse(strArr[2]) / 100 : 0.3;
                            GetTAValue(CdlAbandonedBaby, penetration, ref columns);
                        }
                        else if (fieldName == "AdvanceBlock")
                            GetTAValue(CdlAdvanceBlock, ref columns);
                        else if (fieldName == "BeltHold")
                            GetTAValue(CdlBeltHold, ref columns);
                        else if (fieldName == "Breakaway")
                            GetTAValue(CdlBreakaway, ref columns);
                        else if (fieldName == "ClosingMarubozu")
                            GetTAValue(CdlClosingMarubozu, ref columns);
                        else if (fieldName == "ConcealingBabySwallow")
                            GetTAValue(CdlConcealBabysWall, ref columns);
                        else if (fieldName == "Counterattack")
                            GetTAValue(CdlCounterAttack, ref columns);
                        else if (fieldName == "DarkCloudCover")
                        {
                            double penetration = strArr.Length > 2 ? double.Parse(strArr[2]) / 100 : 0.3;
                            GetTAValue(CdlDarkCloudCover, penetration, ref columns);
                        }
                        else if (fieldName == "Doji")
                            GetTAValue(CdlDoji, ref columns);
                        else if (fieldName == "DojiStar")
                            GetTAValue(CdlDojiStar, ref columns);
                        else if (fieldName == "DragonflyDoji")
                            GetTAValue(CdlDragonflyDoji, ref columns);
                        else if (fieldName == "EngulfingPattern")
                            GetTAValue(CdlEngulfing, ref columns);
                        else if (fieldName == "EveningDojiStar")
                        {
                            double penetration = strArr.Length > 2 ? double.Parse(strArr[2]) / 100 : 0.3;
                            GetTAValue(CdlEveningDojiStar, penetration, ref columns);
                        }
                        else if (fieldName == "EveningStar")
                        {
                            double penetration = strArr.Length > 2 ? double.Parse(strArr[2]) / 100 : 0.3;
                            GetTAValue(CdlEveningStar, penetration, ref columns);
                        }
                        else if (fieldName == "UpDownGapSideBySideWhiteLines")
                            GetTAValue(CdlGapSideSideWhite, ref columns);
                        else if (fieldName == "GravestoneDoji")
                            GetTAValue(CdlGravestoneDoji, ref columns);
                        else if (fieldName == "Hammer")
                            GetTAValue(CdlHammer, ref columns);
                        else if (fieldName == "HangingMan")
                            GetTAValue(CdlHangingMan, ref columns);
                        else if (fieldName == "HaramiPattern")
                            GetTAValue(CdlHarami, ref columns);
                        else if (fieldName == "HaramiCrossPattern")
                            GetTAValue(CdlHaramiCross, ref columns);
                        else if (fieldName == "HighWaveCandle")
                            GetTAValue(CdlHignWave, ref columns);
                        else if (fieldName == "HikkakePattern")
                            GetTAValue(CdlHikkake, ref columns);
                        else if (fieldName == "ModifiedHikkakePattern")
                            GetTAValue(CdlHikkakeMod, ref columns);
                        else if (fieldName == "HomingPigeon")
                            GetTAValue(CdlHomingPigeon, ref columns);
                        else if (fieldName == "IdenticalThreeCrows")
                            GetTAValue(CdlIdentical3Crows, ref columns);
                        else if (fieldName == "InNeck")
                            GetTAValue(CdlInNeck, ref columns);
                        else if (fieldName == "InvertedHammer")
                            GetTAValue(CdlInvertedHammer, ref columns);
                        else if (fieldName == "Kicking")
                            GetTAValue(CdlKicking, ref columns);
                        else if (fieldName == "KickingByLenghth")
                            GetTAValue(CdlKickingByLength, ref columns);
                        else if (fieldName == "LadderBottom")
                            GetTAValue(CdlLadderBottom, ref columns);
                        else if (fieldName == "LongLeggedDoji")
                            GetTAValue(CdlLongLeggedDoji, ref columns);
                        else if (fieldName == "LongLineCandle")
                            GetTAValue(CdlLongLine, ref columns);
                        else if (fieldName == "Marubozu")
                            GetTAValue(CdlMarubozu, ref columns);
                        else if (fieldName == "MatchingLow")
                            GetTAValue(CdlMatchingLow, ref columns);
                        else if (fieldName == "MatHold")
                        {
                            double penetration = strArr.Length > 2 ? double.Parse(strArr[2]) / 100 : 0.3;
                            GetTAValue(CdlMatHold, penetration, ref columns);
                        }
                        else if (fieldName == "MorningDojiStar")
                        {
                            double penetration = strArr.Length > 2 ? double.Parse(strArr[2]) / 100 : 0.3;
                            GetTAValue(CdlMorningDojiStar, penetration, ref columns);
                        }
                        else if (fieldName == "MorningStar")
                        {
                            double penetration = strArr.Length > 2 ? double.Parse(strArr[2]) / 100 : 0.3;
                            GetTAValue(CdlMorningStar, penetration, ref columns);
                        }
                        else if (fieldName == "OnNeckPattern")
                            GetTAValue(CdlOnNeck, ref columns);
                        else if (fieldName == "PiercingPattern")
                            GetTAValue(CdlPiercing, ref columns);
                        else if (fieldName == "RickshawMan")
                            GetTAValue(CdlRickshawMan, ref columns);
                        else if (fieldName == "RisingFallingThreeMethods")
                            GetTAValue(CdlRiseFall3Methods, ref columns);
                        else if (fieldName == "SeparatingLines")
                            GetTAValue(CdlSeperatingLines, ref columns);
                        else if (fieldName == "ShootingStar")
                            GetTAValue(CdlShootingStar, ref columns);
                        else if (fieldName == "ShortLineCandle")
                            GetTAValue(CdlShortLine, ref columns);
                        else if (fieldName == "SpinningTop")
                            GetTAValue(CdlSpinningTop, ref columns);
                        else if (fieldName == "StalledPattern")
                            GetTAValue(CdlStalledPattern, ref columns);
                        else if (fieldName == "StickSandwich")
                            GetTAValue(CdlStickSandwhich, ref columns);
                        else if (fieldName == "Takuri")
                            GetTAValue(CdlTakuri, ref columns);
                        else if (fieldName == "TasukiGap")
                            GetTAValue(CdlTasukiGap, ref columns);
                        else if (fieldName == "ThrustingPattern")
                            GetTAValue(CdlThrusting, ref columns);
                        else if (fieldName == "TristarPattern")
                            GetTAValue(CdlTristar, ref columns);
                        else if (fieldName == "Unique3River")
                            GetTAValue(CdlUnique3River, ref columns);
                        else if (fieldName == "UpsideGapTwoCrows")
                            GetTAValue(CdlUpsideGap2Crows, ref columns);
                        else if (fieldName == "UpsideDownsideGapThreeMethods")
                            GetTAValue(CdlXSideGap3Methods, ref columns);
                        else if (fieldName == "ChandeMomentumOscillator")
                            GetTAValue(Cmo, term, ref columns);
                        else if (fieldName == "HilbertTransformDominantCyclePeriod")
                            GetTAValue(HtDcPeriod, ref columns);
                        else if (fieldName == "HilbertTransformDominantCyclePhase")
                            GetTAValue(HtDcPhase, ref columns);
                        else if (fieldName == "HilbertTransformPhasorComponents")
                            GetHtPhasor(ref columns);
                        else if (fieldName == "HilbertTransformSineWave")
                            GetHtSine(ref columns);
                        else if (fieldName == "HilbertTransformInstantaneousTrendline")
                            GetTAValue(HtTrendline, ref columns);
                        else if (fieldName == "HilbertTransformTrendVsCycleMode")
                            GetTAValue(HtTrendMode, ref columns);
                        else if (fieldName == "LinearRegression")
                            GetTAValue(LinearReg, term, ref columns);
                        else if (fieldName == "LinearRegressionAngle")
                            GetTAValue(LinearRegAngle, term, ref columns);
                        else if (fieldName == "LinearRegressionIntercept")
                            GetTAValue(LinearRegIntercept, term, ref columns);
                        else if (fieldName == "LinearRegressionSlope")
                            GetTAValue(LinearRegSlope, term, ref columns);
                        else if (fieldName == "MovingAverageConvergenceDivergence")
                        {
                            int optInFastPeriod = strArr.Length > 2 ? int.Parse(strArr[2]) : 12;
                            int optInSlowPeriod = strArr.Length > 3 ? int.Parse(strArr[3]) : 26;
                            int optInSignalPeriod = strArr.Length > 4 ? int.Parse(strArr[4]) : 9;

                            GetMacd(optInFastPeriod, optInSlowPeriod, optInSignalPeriod, ref columns);
                        }
                        else if (fieldName == "MacdWithMAType")
                        {
                            int optInFastPeriod = strArr.Length > 2 ? int.Parse(strArr[2]) : 12;
                            int optInSlowPeriod = strArr.Length > 3 ? int.Parse(strArr[3]) : 26;
                            int optInSignalPeriod = strArr.Length > 4 ? int.Parse(strArr[4]) : 9;
                            var maType = strArr.Length > 5 ? (MAType)Enum.Parse(typeof(MAType), strArr[5]) : MAType.Sma;

                            GetMacd(optInFastPeriod, optInSlowPeriod, optInSignalPeriod, maType, ref columns);
                        }
                        else if (fieldName == "Max")
                            GetTAValue(Max, term, ref columns);
                        else if (fieldName == "MaxIndex")
                            GetTAValue(MaxIndex, term, ref columns);
                        else if (fieldName == "MedPrice")
                            GetTAValue((TADelegate_HL_Double)MedPrice, ref columns);
                        else if (fieldName == "MoneyFlowIndex")
                            GetTAValue(Mfi, term, ref columns);
                        else if (fieldName == "MidPoint")
                            GetTAValue(MidPoint, term, ref columns);
                        else if (fieldName == "MidPrice")
                            GetTAValue(MidPrice, term, ref columns);
                        else if (fieldName == "Min")
                            GetTAValue(Min, term, ref columns);
                        else if (fieldName == "MinIndex")
                            GetTAValue(MinIndex, term, ref columns);
                        else if (fieldName == "Momentum")
                            GetTAValue(Mom, term, ref columns);
                        else if (fieldName == "NormalizedAverageTrueRange")
                            GetTAValue(Natr, term, ref columns);
                        else if (fieldName == "OnBalanceVolume")
                            GetTAValue((TADelegate_CV_Double)Obv, ref columns);
                        else if (fieldName == "PercentagePriceOscillator")
                        {
                            int optInFastPeriod = strArr.Length > 2 ? int.Parse(strArr[2]) : 12;
                            int optInSlowPeriod = strArr.Length > 3 ? int.Parse(strArr[3]) : 26;
                            var maType = strArr.Length > 4 ? (MAType)Enum.Parse(typeof(MAType), strArr[4]) : MAType.Sma;
                            GetTAValue(Ppo, optInFastPeriod, optInSlowPeriod, maType, ref columns);
                        }
                        else if (fieldName == "RelativeStrengthIndex")
                            GetTAValue(Rsi, term, ref columns);
                        else if (fieldName == "ParabolicSAR")
                        {
                            double acceleration = strArr.Length > 2 ? double.Parse(strArr[2]) / 100 : 0.01;
                            double Maximum = strArr.Length > 3 ? double.Parse(strArr[3]) / 100 : 0.2;
                            GetTAValue(Sar, acceleration, Maximum, ref columns);
                        }
                        else if (fieldName == "ParabolicSARExt")
                        {
                            var outReal = new double[dayChart.Candles.Count];
                            double optInStartValue = strArr.Length > 2 ? double.Parse(strArr[2]) / 1000 : 0;
                            double optInOffsetOnReverse = strArr.Length > 3 ? double.Parse(strArr[3]) / 1000 : 0;
                            double optInAccelerationInitLong = strArr.Length > 4 ? double.Parse(strArr[4]) / 1000 : 0.02;
                            double optInAccelerationLong = strArr.Length > 5 ? double.Parse(strArr[5]) / 1000 : 0.02;
                            double optInAccelerationMaxLong = strArr.Length > 6 ? double.Parse(strArr[6]) / 1000 : 0.2;
                            double optInAccelerationInitShort = strArr.Length > 7 ? double.Parse(strArr[7]) / 1000 : 0.02;
                            double optInAccelerationShort = strArr.Length > 8 ? double.Parse(strArr[8]) / 1000 : 0.02;
                            double optInAccelerationMaxShort = strArr.Length > 9 ? double.Parse(strArr[9]) / 1000 : 0.2;

                            SarExt(0, dayChart.Candles.Count - 1, high, low, optInStartValue, optInOffsetOnReverse, optInAccelerationInitLong, optInAccelerationLong, optInAccelerationMaxLong, optInAccelerationInitShort, optInAccelerationShort, optInAccelerationMaxShort, out outBegIdx, out outNBElement, outReal);
                            if (outNBElement > 0)
                                columns.Add(outReal[outNBElement - 1].ToString());
                            else
                                columns.Add("0"); // Chart가 모자를때
                        }
                        else if (fieldName == "StandardDeviation")
                        {
                            double optInNbDev = strArr.Length > 3 ? double.Parse(strArr[3]) : 2;
                            GetTAValue(StdDev, term, optInNbDev, ref columns);
                        }
                        else if (fieldName == "Stochastic")
                        {
                            double[] outSlowK = new double[dayChart.Candles.Count];
                            double[] outSlowD = new double[dayChart.Candles.Count];

                            int optInFastK_Period = strArr.Length > 2 ? int.Parse(strArr[2]) : 0;
                            int optInSlowK_Period = strArr.Length > 3 ? int.Parse(strArr[3]) : 0;
                            MAType optInSlowK_MAType = strArr.Length > 4 ? (MAType)Enum.Parse(typeof(MAType), strArr[4]) : MAType.Sma;
                            int optInSlowD_Period = strArr.Length > 5 ? int.Parse(strArr[5]) : 0;
                            MAType optInSlowD_MAType = strArr.Length > 6 ? (MAType)Enum.Parse(typeof(MAType), strArr[6]) : MAType.Sma;

                            Stoch(0, dayChart.Candles.Count - 1, high, low, close, optInFastK_Period, optInSlowK_Period, optInSlowK_MAType, optInSlowD_Period, optInSlowD_MAType, out outBegIdx, out outNBElement, outSlowK, outSlowD);
                            if (outNBElement > 0)
                            {
                                columns.Add(outSlowK[outNBElement - 1].ToString());
                                columns.Add(outSlowD[outNBElement - 1].ToString());
                            }
                            else
                            {
                                columns.Add("0"); // Chart가 모자를때
                                columns.Add("0"); // Chart가 모자를때
                            }
                        }
                        else if (fieldName == "StochasticFast")
                        {
                            double[] outFastK = new double[dayChart.Candles.Count];
                            double[] outFastD = new double[dayChart.Candles.Count];
                            int optInFastK_Period = strArr.Length > 2 ? int.Parse(strArr[2]) : 0;
                            int optInFastD_Period = strArr.Length > 3 ? int.Parse(strArr[3]) : 0;
                            MAType optInFastD_MAType = strArr.Length > 4 ? (MAType)Enum.Parse(typeof(MAType), strArr[4]) : MAType.Sma;
                            StochF(0, dayChart.Candles.Count - 1, high, low, close, optInFastK_Period, optInFastD_Period, optInFastD_MAType, out outBegIdx, out outNBElement, outFastK, outFastD);
                            if (outNBElement > 0)
                            {
                                columns.Add(outFastK[outNBElement - 1].ToString());
                                columns.Add(outFastD[outNBElement - 1].ToString());
                            }
                            else
                            {
                                columns.Add("0"); // Chart가 모자를때
                                columns.Add("0"); // Chart가 모자를때
                            }
                        }
                        else if (fieldName == "StochasticRelativeStrengthIndex")
                        {
                            double[] outFastK = new double[dayChart.Candles.Count];
                            double[] outFastD = new double[dayChart.Candles.Count];
                            int optInFastK_Period = strArr.Length > 3 ? int.Parse(strArr[3]) : 0;
                            int optInFastD_Period = strArr.Length > 4 ? int.Parse(strArr[4]) : 0;
                            MAType optInFastD_MAType = strArr.Length > 5 ? (MAType)Enum.Parse(typeof(MAType), strArr[5]) : MAType.Sma;
                            StochRsi(0, dayChart.Candles.Count - 1, close, term, optInFastK_Period, optInFastD_Period, optInFastD_MAType, out outBegIdx, out outNBElement, outFastK, outFastD);
                            if (outNBElement > 0)
                            {
                                columns.Add(outFastK[outNBElement - 1].ToString());
                                columns.Add(outFastD[outNBElement - 1].ToString());
                            }
                            else
                            {
                                columns.Add("0"); // Chart가 모자를때
                                columns.Add("0"); // Chart가 모자를때
                            }
                        }
                        else if (fieldName == "TrueRange")
                            GetTAValue(TrueRange, ref columns);
                        else if (fieldName == "TripleSmoothEma")
                            GetTAValue(Trix, term, ref columns);
                        else if (fieldName == "TimeSeriesForecast")
                            GetTAValue(Tsf, term, ref columns);
                        else if (fieldName == "UltimateOscillator")
                        {
                            var outReal = new double[dayChart.Candles.Count];
                            int optInTimePeriod1 = strArr.Length > 2 ? int.Parse(strArr[2]) : 4;
                            int optInTimePeriod2 = strArr.Length > 3 ? int.Parse(strArr[3]) : 8;
                            int optInTimePeriod3 = strArr.Length > 4 ? int.Parse(strArr[4]) : 16;
                            UltOsc(0, dayChart.Candles.Count - 1, high, low, close, optInTimePeriod1, optInTimePeriod2, optInTimePeriod3, out outBegIdx, out outNBElement, outReal);
                            if (outNBElement > 0)
                                columns.Add(outReal[outNBElement - 1].ToString());
                            else
                                columns.Add("0"); // Chart가 모자를때
                        }
                        else if (fieldName == "Variance")
                        {
                            double optInNbDev = strArr.Length > 3 ? double.Parse(strArr[3]) : 2;
                            GetTAValue(Variance, term, optInNbDev, ref columns);
                        }
                        else if (fieldName == "WeightedClosePrice")
                            GetTAValue(WclPrice, ref columns);
                        else if (fieldName == "WilliamsR")
                            GetTAValue(WillR, term, ref columns);
                    }
                }

                sw.WriteLine(string.Join(delimeter, columns));
                for (int i = 0; i < columns.Count; i++)
                {
                    var value = Convert.ToDouble(columns[i]);
                    if (i + 1 > minLimit.Count)
                    {
                        minLimit.Add(value);
                        maxLimit.Add(value);
                    }
                    else
                    {
                        if (minLimit[i] > value) minLimit[i] = value;
                        if (maxLimit[i] < value) maxLimit[i] = value;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            finally
            {
                columns.Clear();
            }
        }

        private void GetTickMovingAverage(int term, MAType maType, ref List<string> columns)
        {
            int elementCount = term;
            do
            {
                if (priceList.Count < elementCount)
                {
                    columns.Add(priceList.Average().ToString());
                    break;
                }

                var inReal = priceList.GetRange(priceList.Count - elementCount, elementCount).ToArray();
                var outReal = new double[elementCount];

                MovingAverage(0, inReal.Length - 1, inReal, term, maType, out outBegIdx, out outNBElement, outReal);
                if (outNBElement != 0)
                    columns.Add(outReal[0].ToString());
                else
                    elementCount++;
            } while (outNBElement == 0);
        }
        private void GetAroon(int term, ref List<string> columns)
        {
            var outDownReal = new double[dayChart.Candles.Count];
            var outUpReal = new double[dayChart.Candles.Count];
            Aroon(0, dayChart.Candles.Count - 1, high, low, term, out outBegIdx, out outNBElement, outDownReal, outUpReal);
            columns.Add(outDownReal[outNBElement - 1].ToString());
            columns.Add(outUpReal[outNBElement - 1].ToString());
        }
        private void GetBbands(int term, float dev, MAType maType, ref List<string> columns)
        {
            var outUpperReal = new double[dayChart.Candles.Count];
            var outMiddleReal = new double[dayChart.Candles.Count];
            var outLowerReal = new double[dayChart.Candles.Count];
            Bbands(0, dayChart.Candles.Count - 1, close, term, dev, dev, maType, out outBegIdx, out outNBElement, outUpperReal, outMiddleReal, outLowerReal);
            columns.Add(outUpperReal[outNBElement - 1].ToString());
            columns.Add(outMiddleReal[outNBElement - 1].ToString());
            columns.Add(outLowerReal[outNBElement - 1].ToString());
        }
        private void GetHtPhasor(ref List<string> columns)
        {
            var outInPhase = new double[dayChart.Candles.Count];
            var outQuadrature = new double[dayChart.Candles.Count];
            HtPhasor(0, dayChart.Candles.Count - 1, close, out outBegIdx, out outNBElement, outInPhase, outQuadrature);
            columns.Add(outInPhase[outNBElement - 1].ToString());
            columns.Add(outQuadrature[outNBElement - 1].ToString());
        }
        private void GetHtSine(ref List<string> columns)
        {
            var outSine = new double[dayChart.Candles.Count];
            var outLeadSine = new double[dayChart.Candles.Count];
            HtSine(0, dayChart.Candles.Count - 1, close, out outBegIdx, out outNBElement, outSine, outLeadSine);
            columns.Add(outSine[outNBElement - 1].ToString());
            columns.Add(outLeadSine[outNBElement - 1].ToString());
        }
        private void GetMacd(int optInFastPeriod, int optInSlowPeriod, int optInSignalPeriod, ref List<string> columns)
        {
            
            var outMACD = new double[dayChart.Candles.Count];
            var outMACDSignal = new double[dayChart.Candles.Count];
            var outMACDHist = new double[dayChart.Candles.Count];

            Macd(0, dayChart.Candles.Count - 1, close, optInFastPeriod, optInSlowPeriod, optInSignalPeriod, out outBegIdx, out outNBElement, outMACD, outMACDSignal, outMACDHist);
            columns.Add(outMACD[outNBElement - 1].ToString());
            columns.Add(outMACDSignal[outNBElement - 1].ToString());
            columns.Add(outMACDHist[outNBElement - 1].ToString());
        }
        private void GetMacd(int optInFastPeriod, int optInSlowPeriod, int optInSignalPeriod, MAType maType, ref List<string> columns)
        {
            var outMACD = new double[dayChart.Candles.Count];
            var outMACDSignal = new double[dayChart.Candles.Count];
            var outMACDHist = new double[dayChart.Candles.Count];

            MacdExt(0, dayChart.Candles.Count - 1, close, optInFastPeriod, maType, optInSlowPeriod, maType, optInSignalPeriod, maType, out outBegIdx, out outNBElement, outMACD, outMACDSignal, outMACDHist);
            if (outNBElement > 0)
            {
                columns.Add(outMACD[outNBElement - 1].ToString());
                columns.Add(outMACDSignal[outNBElement - 1].ToString());
                columns.Add(outMACDHist[outNBElement - 1].ToString());
            }
            else
            {
                columns.Add("0");
                columns.Add("0");
                columns.Add("0");
            }
        }
        
        private void GetTAValue(TADelegate_C_Int function, ref List<string> columns)
        {
            var outInt = new int[dayChart.Candles.Count];
            function(0, dayChart.Candles.Count - 1, close, out outBegIdx, out outNBElement, outInt);
            columns.Add(outInt[outNBElement - 1].ToString());
        }

        private void GetTAValue(TADelegate_C_Double function, ref List<string> columns)
        {
            var outReal = new double[dayChart.Candles.Count];
            function(0, dayChart.Candles.Count - 1, close, out outBegIdx, out outNBElement, outReal);
            columns.Add(outReal[outNBElement - 1].ToString());
        }

        private void GetTAValue(TADelegate_CT_Double function, int term, ref List<string> columns)
        {
            var outReal = new double[dayChart.Candles.Count];
            function(0, dayChart.Candles.Count - 1, close, term, out outBegIdx, out outNBElement, outReal);
            if (outNBElement > 0)
                columns.Add(outReal[outNBElement - 1].ToString());
            else
                columns.Add("0"); // Chart가 모자를때
        }
        private void GetTAValue(TADelegate_CT_Int function, int term, ref List<string> columns)
        {
            var outReal = new int[dayChart.Candles.Count];
            function(0, dayChart.Candles.Count - 1, close, term, out outBegIdx, out outNBElement, outReal);
            if (outNBElement > 0)
                columns.Add(outReal[outNBElement - 1].ToString());
            else
                columns.Add("0"); // Chart가 모자를때
        }
        private void GetTAValue(TADelegate_CTD_Double function, int term, double nbDev, ref List<string> columns)
        {
            var outReal = new double[dayChart.Candles.Count];
            function(0, dayChart.Candles.Count - 1, close, term, nbDev, out outBegIdx, out outNBElement, outReal);
            if (outNBElement > 0)
                columns.Add(outReal[outNBElement - 1].ToString());
            else
                columns.Add("0"); // Chart가 모자를때
        }
        private void GetTAValue(TADelegate_CV_Double function, ref List<string> columns)
        {
            var outReal = new double[dayChart.Candles.Count];
            function(0, dayChart.Candles.Count - 1, close, volume, out outBegIdx, out outNBElement, outReal);
            if (outNBElement > 0)
                columns.Add(outReal[outNBElement - 1].ToString());
            else
                columns.Add("0"); // Chart가 모자를때
        }

        private void GetTAValue(TADelegate_CTM_Double function, int term, MAType maType, ref List<string> columns)
        {
            var outReal = new double[dayChart.Candles.Count];
            function(0, dayChart.Candles.Count - 1, close, term, maType, out outBegIdx, out outNBElement, outReal);

            if (outNBElement > 0)
                columns.Add(outReal[outNBElement - 1].ToString());
            else
                columns.Add("0"); // Chart가 모자를때
        }
        private void GetTAValue(TADelegate_HL_Double function, ref List<string> columns)
        {
            var outReal = new double[dayChart.Candles.Count];
            function(0, dayChart.Candles.Count - 1, high, low, out outBegIdx, out outNBElement, outReal);
            if (outNBElement > 0)
                columns.Add(outReal[outNBElement - 1].ToString());
            else
                columns.Add("0"); // Chart가 모자를때
        }
        private void GetTAValue(TADelegate_HLAM_Double function, double optInAcceleration, double optInMaximum, ref List<string> columns)
        {
            var outReal = new double[dayChart.Candles.Count];
            function(0, dayChart.Candles.Count - 1, high, low, optInAcceleration, optInMaximum, out outBegIdx, out outNBElement, outReal);
            if (outNBElement > 0)
                columns.Add(outReal[outNBElement - 1].ToString());
            else
                columns.Add("0"); // Chart가 모자를때
        }
        private void GetTAValue(TADelegate_HLC_Double function, ref List<string> columns)
        {
            var outReal = new double[dayChart.Candles.Count];
            function(0, dayChart.Candles.Count - 1, high, low, close, out outBegIdx, out outNBElement, outReal);
            columns.Add(outReal[outNBElement - 1].ToString());
        }
        private void GetTAValue(TADelegate_HLCV_Double function,ref List<string> columns)
        {
            var outReal = new double[dayChart.Candles.Count];
            function(0, dayChart.Candles.Count - 1, high, low, close, volume, out outBegIdx, out outNBElement, outReal);
            columns.Add(outReal[outNBElement - 1].ToString());
        }
        private void GetTAValue(TADelegate_HLCVT_Double function, int period, ref List<string> columns)
        {
            var outReal = new double[dayChart.Candles.Count];
            function(0, dayChart.Candles.Count - 1, high, low, close, volume, period, out outBegIdx, out outNBElement, outReal);
            if (outNBElement > 0)
                columns.Add(outReal[outNBElement - 1].ToString());
            else
                columns.Add("0"); // Chart가 모자를때
        }

        private void GetTAValue(TADelegate_HLCVTT_Double function, int fastPeriod, int slowPeriod, ref List<string> columns)
        {
            var outReal = new double[dayChart.Candles.Count];
            function(0, dayChart.Candles.Count - 1, high, low, close, volume, fastPeriod, slowPeriod, out outBegIdx, out outNBElement, outReal);
            columns.Add(outReal[outNBElement - 1].ToString());
        }
        private void GetTAValue(TADelegate_CTTM_Double function, int fastPeriod, int slowPeriod, MAType maType, ref List<string> columns)
        {
            var outReal = new double[dayChart.Candles.Count];
            function(0, dayChart.Candles.Count - 1, close, fastPeriod, slowPeriod, maType, out outBegIdx, out outNBElement, outReal);
            columns.Add(outReal[outNBElement - 1].ToString());
        }
        private void GetTAValue(TADelegate_HLCT_Double function, int term, ref List<string> columns)
        {
            var outReal = new double[dayChart.Candles.Count];
            function(0, dayChart.Candles.Count - 1, high, low, close, term, out outBegIdx, out outNBElement, outReal);
            if (outNBElement > 0)
                columns.Add(outReal[outNBElement - 1].ToString());
            else
                columns.Add("0");
        }
        private void GetTAValue(TADelegate_OHLC_Double function, ref List<string> columns)
        {
            var outReal = new double[dayChart.Candles.Count];
            function(0, dayChart.Candles.Count - 1, open, high, low, close, out outBegIdx, out outNBElement, outReal);
            columns.Add(outReal[outNBElement - 1].ToString());
        }
        private void GetTAValue(TADelegate_OHLC_Int function, ref List<string> columns)
        {
            var outInt = new int[dayChart.Candles.Count];
            function(0, dayChart.Candles.Count - 1, open, high, low, close, out outBegIdx, out outNBElement, outInt);
            columns.Add(outInt[outNBElement - 1].ToString());
        }
        private void GetTAValue(TADelegate_OHLCP_Int function, double penetration, ref List<string> columns)
        {
            var outInt = new int[dayChart.Candles.Count];
            function(0, dayChart.Candles.Count - 1, open, high, low, close, penetration, out outBegIdx, out outNBElement, outInt);
            columns.Add(outInt[outNBElement - 1].ToString());
        }
        private void GetTAValue(TADelegate_HLT_Double function, int term, ref List<string> columns)
        {
            var outReal = new double[dayChart.Candles.Count];
            function(0, dayChart.Candles.Count - 1, high, low, term, out outBegIdx, out outNBElement, outReal);
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
