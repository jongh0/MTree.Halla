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

        public static IEnumerable<PropertyInfo> GetProperties(Type objectType, Type ignoreAttributeType = default(Type))
        {
            if (_propDic.TryGetValue(objectType, out var properties) == false)
            {
                properties = objectType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                _propDic.TryAdd(objectType, properties);
            }

            if (ignoreAttributeType != default(Type))
                return properties.Where(p => System.Attribute.IsDefined(p, ignoreAttributeType) == false);

            return properties;
        }

        public static string PrintValues(object obj, Type ignoreAttributeType = default(Type), string seperator = ", ")
        {
            if (obj == null) return string.Empty;

            List<string> strList = new List<string>();

            try
            {
                var properties = GetProperties(obj.GetType(), ignoreAttributeType);
                foreach (var property in properties)
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

        public static string PrintNameValues(object obj, Type ignoreAttributeType = default(Type), string seperator = ", ")
        {
            if (obj == null) return string.Empty;

            List<string> strList = new List<string>();

            try
            {
                var properties = GetProperties(obj.GetType(), ignoreAttributeType);
                foreach (var property in properties)
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
