using MTree.DataStructure;
using MTree.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TicTacTec.TA.Library;

namespace MTree.DataExtractor
{
    public class DataExtractor
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly string delimeter = ",";

        private object masterBak;

        int term = 5;
        Queue<float> priceQueue = new Queue<float>();

        public void Extract(List<StockConclusion> conclusionList, List<StockMaster> masterList, string path)
        {
            if (conclusionList == null || masterList == null) return;
            if (string.IsNullOrEmpty(path) == true) return;

            try
            {
                masterBak = null;

                using (var fs = File.Open(path, FileMode.Create))
                using (var sw = new StreamWriter(fs))
                {
                    WriteHeader(sw, ExtractTypes.Stock);

                    foreach (var conclusion in conclusionList)
                    {
                        WriteContent(sw, conclusion, GetMaster(masterList, conclusion.Time));
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public void Extract(List<IndexConclusion> conclusionList, List<IndexMaster> masterList, string path)
        {
            if (conclusionList == null || masterList == null) return;
            if (string.IsNullOrEmpty(path) == true) return;

            try
            {
                masterBak = null;

                using (var fs = File.Open(path, FileMode.Create))
                using (var sw = new StreamWriter(fs))
                {
                    WriteHeader(sw, ExtractTypes.Index);

                    foreach (var conclusion in conclusionList)
                    {
                        WriteContent(sw, conclusion, GetMaster(masterList, conclusion.Time));
                    }

                    sw.Flush();
                    fs.Flush(true);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private T GetMaster<T>(List<T> masterList, DateTime targetDate)
        {
            try
            {
                targetDate = DateTimeUtility.DateOnly(targetDate);

                if (masterBak != null && (masterBak as Subscribable).Time == targetDate)
                    return (T)masterBak;

                masterBak = masterList.Where(i => (i as Subscribable).Time == targetDate).FirstOrDefault();
                return (T)masterBak;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return default(T);
        }

        private void WriteHeader(StreamWriter sw, ExtractTypes extractType)
        {
            List<string> columns = new List<string>();

            try
            {
                if (extractType == ExtractTypes.Stock)
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
                }
                else
                {
                    foreach (var field in Enum.GetValues(typeof(IndexConclusionField)))
                    {
                        columns.Add(field.ToString());
                    }

                    foreach (var field in Enum.GetValues(typeof(IndexMasterField)))
                    {
                        columns.Add(field.ToString());
                    }

                    foreach (var field in Enum.GetValues(typeof(IndexTAField)))
                    {
                        columns.Add(field.ToString());
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

        private void WriteContent(StreamWriter sw, StockConclusion conclusion, StockMaster master)
        {
            if (conclusion == null || master == null) return;

            List<string> columns = new List<string>();

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
                    if ((StockTAField)field == StockTAField.MovingAverage)
                    {
                        int outBegIdx;
                        int outNBElement;
                        double[] outReal = new double[1];

                        priceQueue.Enqueue(conclusion.Price);

                        if (priceQueue.Count > term)
                            priceQueue.Dequeue();

                        // MovingAverage(int startIdx, int endIdx, float[] inReal, int optInTimePeriod, MAType optInMAType, out int outBegIdx, out int outNBElement, double[] outReal)
                        Core.MovingAverage(0, priceQueue.Count - 1, priceQueue.ToArray(), term, Core.MAType.Sma, out outBegIdx, out outNBElement, outReal);
                        columns.Add(outReal[0].ToString());
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

        private void WriteContent(StreamWriter sw, IndexConclusion conclusion, IndexMaster master)
        {
            if (conclusion == null || master == null) return;

            List<string> columns = new List<string>();

            try
            {
                foreach (var field in Enum.GetValues(typeof(IndexConclusionField)))
                {
                    var property = conclusion.GetType().GetProperty(field.ToString());
                    object value = property.GetValue(conclusion);

                    columns.Add(GetNormalizedValue(value));
                }

                foreach (var field in Enum.GetValues(typeof(IndexMaster)))
                {
                    var property = master.GetType().GetProperty(field.ToString());
                    object value = property.GetValue(master);

                    columns.Add(GetNormalizedValue(value));
                }

                foreach (var field in Enum.GetValues(typeof(IndexTAField)))
                {
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
