using MTree.DataStructure;
using MTree.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MTree.DataExtractor
{
    public class DataExtractor
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly string delimeter = ",";

        private object masterBak;

        private StringBuilder Content { get; set; } = new StringBuilder();

        public void Extract(List<StockConclusion> conclusionList, List<StockMaster> masterList, string path)
        {
            if (conclusionList == null || masterList == null) return;
            if (string.IsNullOrEmpty(path) == true) return;

            try
            {
                masterBak = null;
                Content.Clear();

                WriteHeader(ExtractTypes.Stock);

                foreach (var conclusion in conclusionList)
                {
                    WriteContent(conclusion, GetMaster(masterList, conclusion.Time));
                }

                File.WriteAllText(path, Content.ToString());
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                Content.Clear();
            }
        }

        public void Extract(List<IndexConclusion> conclusionList, List<IndexMaster> masterList, string filePath)
        {
            if (conclusionList == null || masterList == null) return;
            if (string.IsNullOrEmpty(filePath) == true) return;

            try
            {
                masterBak = null;
                Content.Clear();

                WriteHeader(ExtractTypes.Index);

                foreach (var conclusion in conclusionList)
                {
                    WriteContent(conclusion, GetMaster(masterList, conclusion.Time));
                }

                File.WriteAllText(filePath, Content.ToString());
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                Content.Clear();
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

        private void WriteHeader(ExtractTypes extractType)
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
                }

                Content.AppendLine(string.Join(delimeter, columns));
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

        private void WriteContent(StockConclusion conclusion, StockMaster master)
        {
            if (conclusion == null || master == null) return;

            List<string> columns = new List<string>();

            try
            {
                foreach (var field in Enum.GetValues(typeof(StockConclusionField)))
                {
                    var property = conclusion.GetType().GetProperty(field.ToString());
                    object value = property.GetValue(conclusion);

                    columns.Add(GetNormalizedValue(property, value));
                }

                foreach (var field in Enum.GetValues(typeof(StockMasterField)))
                {
                    var property = master.GetType().GetProperty(field.ToString());
                    object value = property.GetValue(master);

                    columns.Add(GetNormalizedValue(property, value));
                }

                Content.AppendLine(string.Join(delimeter, columns));
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

        private void WriteContent(IndexConclusion conclusion, IndexMaster master)
        {
            if (conclusion == null || master == null) return;
        }

        private string GetNormalizedValue(PropertyInfo property, object value)
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
