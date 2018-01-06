using CommonLib.Attributes;
using CommonLib.Extensions;
using Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using XA_DATASETLib;

namespace CommonLib.Firm.Ebest.Query
{
    public abstract class BlockBase
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public bool Parse(XAQueryClass query)
        {
            if (query == null) return false;

            var sb = new StringBuilder();
            var type = GetType();
            var blockName = type.Name;

            try
            {
                foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.SetProperty))
                {
                    if (Attribute.IsDefined(property, typeof(ParseIgnoreAttribute)) == true) continue;

                    var data = query.GetFieldData(blockName, property.Name, 0);
                    if (string.IsNullOrEmpty(data) == false)
                    {
                        sb.AppendLine($"{property.Name}: {data}");
                        property.SetValueByType(this, data);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            finally
            {
                _logger.Debug($"{blockName}.Parse\n{sb.ToString()}");
            }

            return false;
        }

        public override string ToString()
        {
            List<string> strList = new List<string>();

            try
            {
                foreach (var property in GetType().GetProperties())
                {
                    object value = property.GetValue(this);

                    if (value is DateTime dateTime)
                        strList.Add($"{property.Name}: {dateTime.ToString(Config.General.DateTimeFormat)}");
                    else
                        strList.Add($"{property.Name}: {value}");
                }

                return string.Join(", ", strList.ToArray());
            }
            catch
            {
            }
            finally
            {
                strList.Clear();
            }

            return string.Empty;
        }
    }
}
