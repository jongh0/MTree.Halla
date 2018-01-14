using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Extension
{
    public static class PropertyExtension
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public static void SetValueByType(this PropertyInfo prop, object obj, string data)
        {
            try
            {
                if (prop.PropertyType == typeof(string))
                {
                    prop.SetValue(obj, data);
                }
                else if (prop.PropertyType == typeof(long))
                {
                    if (long.TryParse(data, out var value) == true)
                        prop.SetValue(obj, value);
                }
                else if (prop.PropertyType == typeof(double))
                {
                    if (double.TryParse(data, out var value) == true)
                        prop.SetValue(obj, value);
                }
                else if (prop.PropertyType == typeof(float))
                {
                    if (float.TryParse(data, out var value) == true)
                        prop.SetValue(obj, value);
                }
                else if (prop.PropertyType == typeof(int))
                {
                    if (int.TryParse(data, out var value) == true)
                        prop.SetValue(obj, value);
                }
                else if (prop.PropertyType == typeof(decimal))
                {
                    if (decimal.TryParse(data, out var value) == true)
                        prop.SetValue(obj, value);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }
    }
}
