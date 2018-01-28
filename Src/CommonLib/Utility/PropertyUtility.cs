﻿using CommonLib.Attribute;
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

        public static string PrintValues(object obj, string seperator = ", ", bool excludeEmptyProperty = true)
        {
            return Print(obj, seperator, excludeEmptyProperty, includePropertyName: false);
        }

        public static string PrintNameValues(object obj, string seperator = ", ", bool excludeEmptyProperty = true)
        {
            return Print(obj, seperator, excludeEmptyProperty, includePropertyName: true);
        }

        private static string Print(object obj, string seperator, bool excludeEmptyProperty, bool includePropertyName)
        {
            if (obj == null) return string.Empty;

            List<string> strList = new List<string>();

            try
            {
                foreach (var property in GetProperties(obj.GetType()))
                {
                    var value = property.GetValue(obj);
                    if (value == null) continue;

                    var propertyName = (includePropertyName == true) ? $"{property.Name}: " : string.Empty;
                    var type = value.GetType();

                    if (value is DateTime dateTime)
                    {
                        strList.Add($"{propertyName}{dateTime.ToString(Config.General.DateTimeFormat)}");
                    }
                    //else if (value.GetType().GetInterfaces().Contains(typeof(IEnumerable<>)))
                    //{
                    //    var enumerable = obj as IEnumerable<object>;
                    //    var str = string.Join(" | ", enumerable.Select(i => i.ToString()).ToArray());
                    //    if (excludeEmptyProperty == true && string.IsNullOrEmpty(str) == true)
                    //        continue;

                    //    strList.Add($"{propertyName}{str}");
                    //}
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
