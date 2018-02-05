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

        public static IEnumerable<PropertyInfo> GetProperties(Type type, Type excludeAttributeType = default(Type))
        {
            if (_propDic.TryGetValue(type, out var properties) == false)
            {
                properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                _propDic.TryAdd(type, properties);
            }

            if (excludeAttributeType != default(Type))
                return properties.Where(p => System.Attribute.IsDefined(p, excludeAttributeType) == false);

            return properties;
        }

        public static string PrintValues(object obj, string seperator = ", ", bool excludeEmptyProperty = true, Type excludeAttributeType = default(Type))
        {
            return Print(obj, seperator, excludeEmptyProperty, includePropertyName: false, excludeAttributeType: excludeAttributeType);
        }

        public static string PrintNameValues(object obj, string seperator = ", ", bool excludeEmptyProperty = true, Type excludeAttributeType = default(Type))
        {
            return Print(obj, seperator, excludeEmptyProperty, includePropertyName: true, excludeAttributeType: excludeAttributeType);
        }

        private static string Print(object obj, string seperator, bool excludeEmptyProperty, bool includePropertyName, Type excludeAttributeType)
        {
            if (obj == null) return string.Empty;

            List<string> strList = new List<string>();

            try
            {
                foreach (var property in GetProperties(obj.GetType(), excludeAttributeType))
                {
                    var value = property.GetValue(obj);
                    if (value == null) continue;

                    var propertyName = (includePropertyName == true) ? $"{property.Name}: " : string.Empty;
                    var type = value.GetType();

                    if (value is DateTime dateTime)
                    {
                        strList.Add($"{propertyName}{dateTime.ToString(Config.General.DateTimeFormat)}");
                    }
                    else if (value is IEnumerable<object> enumerable)
                    {
                        var str = string.Join(" | ", enumerable.Select(i => i.ToString()).ToArray());
                        if (excludeEmptyProperty == true && string.IsNullOrEmpty(str) == true)
                            continue;

                        strList.Add($"{propertyName}{{{str}}}");
                    }
                    else
                    {
                        var str = value.ToString();
                        if (excludeEmptyProperty == true && string.IsNullOrEmpty(str) == true)
                            continue;

                        strList.Add($"{propertyName}{str}");
                    }
                }

                return string.Join(seperator, strList.ToArray());
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            finally
            {
                strList.Clear();
            }

            return string.Empty;
        }
    }
}
