using MTree.DataStructure;
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

        private StringBuilder Content { get; set; } = new StringBuilder();

        public void Extract(List<StockConclusion> conclusionList, List<StockMaster> masterList, string path)
        {
            if (conclusionList == null || masterList == null) return;
            if (string.IsNullOrEmpty(path) == true) return;

            Content.Clear();

            foreach (var conclusion in conclusionList)
            {
                WriteContent(conclusion, GetMaster(masterList, conclusion.Time));
            }

            File.WriteAllText(path, Content.ToString());
            Content.Clear();
        }

        public void Extract(List<IndexConclusion> conclusionList, List<IndexMaster> masterList, string path)
        {
            if (conclusionList == null || masterList == null) return;
            if (string.IsNullOrEmpty(path) == true) return;

            Content.Clear();

            foreach (var conclusion in conclusionList)
            {
                WriteContent(conclusion, GetMaster(masterList, conclusion.Time));
            }

            File.WriteAllText(path, Content.ToString());
            Content.Clear();
        }

        private T GetMaster<T>(List<T> masterList, DateTime targetDate)
        {
            try
            {
                return masterList.Where(i => (i as Subscribable).Time == targetDate).FirstOrDefault(); // 속도 느릴텐데..
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return default(T);
        }

        private void WriteContent(StockConclusion conclusion, StockMaster master)
        {
            if (conclusion == null || master == null) return;

            List<string> columns = new List<string>();

            foreach (var field in Enum.GetValues(typeof(StockConclusionField)))
            {
                var property = conclusion.GetType().GetProperty(field.ToString());
                object value = property.GetValue(conclusion);

                columns.Add(GetNormalizedValue(property, value));
            }

            foreach (var field in Enum.GetValues(typeof(StockMasterField)))
            {
                var property = conclusion.GetType().GetProperty(field.ToString());
                object value = property.GetValue(conclusion);

                columns.Add(GetNormalizedValue(property, value));
            }

            Content.AppendLine(string.Join(delimeter, columns));
        }

        private void WriteContent(IndexConclusion conclusion, IndexMaster master)
        {
            if (conclusion == null || master == null) return;
        }

        private string GetNormalizedValue(PropertyInfo property, object value)
        {
            if (value is DateTime)
            {
                DateTime dateTime = (DateTime)value;
                return dateTime.Ticks.ToString();
            }
            else
            {
                return value.ToString();
            }
        }
    }
}
