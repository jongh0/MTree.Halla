using CommonLib.Attribute;
using Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Utility
{
    public class PropertyUtility
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private static ConcurrentDictionary<Type, IEnumerable<PropertyInfo>> _propDic = new ConcurrentDictionary<Type, IEnumerable<PropertyInfo>>();

        public static IEnumerable<PropertyInfo> GetProperties(Type type)
        {
            if (_propDic.TryGetValue(type, out var properties) == false)
            {
                properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => System.Attribute.IsDefined(p, typeof(IgnorePropertyAttribute)) == false);
                _propDic.TryAdd(type, properties);
            }

            return properties;
        }

        public static string PrintValues(object obj, string seperator = ", ")
        {
            if (obj == null) return string.Empty;

            List<string> strList = new List<string>();

            try
            {
                foreach (var property in GetProperties(obj.GetType()))
                {
                    var value = property.GetValue(obj);

                    if (value is DateTime dateTime)
                        strList.Add($"{dateTime.ToString(Config.General.DateTimeFormat)}");
                    else
                        strList.Add($"{value}");
                }

                return string.Join(seperator, strList.ToArray());
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

        public static string PrintNameValues(object obj, string seperator = ", ")
        {
            if (obj == null) return string.Empty;

            List<string> strList = new List<string>();

            try
            {
                foreach (var property in GetProperties(obj.GetType()))
                {
                    var value = property.GetValue(obj);

                    if (value is DateTime dateTime)
                        strList.Add($"{property.Name}: {dateTime.ToString(Config.General.DateTimeFormat)}");
                    else
                        strList.Add($"{property.Name}: {value}");
                }

                return string.Join(seperator, strList.ToArray());
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
