using MTree.Consumer;
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

        private List<float> priceList = new List<float>();

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

        public void ConsumeStockMaster(List<StockMaster> stockMasters)
        {
            try
            {
                //foreach (var stockMaster in stockMasters)
                if (stockMasters.Count == 1)
                {
                    currentMaster = stockMasters[0];
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

            priceList.Add(conclusion.Price);

            try
            {
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
                    if (field.ToString().Contains("MovingAverage"))
                    {
                        var strArr = field.ToString().Split('_');
                        var term = int.Parse(strArr[1]);
                        var maType = (Core.MAType)Enum.Parse(typeof(Core.MAType), strArr[2]);
                        
                        int outBegIdx;
                        int outNBElement = 0;
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
