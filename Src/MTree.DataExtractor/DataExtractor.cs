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

        List<float> priceList = new List<float>();

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
            finally
            {
                priceList.Clear();
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
            finally
            {
                priceList.Clear();
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

                        if (priceList.Count < term)
                        {
                            columns.Add("0");
                            continue;
                        }

                        var inReal = priceList.GetRange(priceList.Count - term, term).ToArray();

                        int outBegIdx;
                        int outNBElement;
                        var outReal = new double[term];
                        
                        Core.MovingAverage(0, inReal.Length - 1, inReal, term, maType, out outBegIdx, out outNBElement, outReal);

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
