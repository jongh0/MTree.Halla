using Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CommonLib.Converter
{
    public class NumberToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return string.Empty;
            else if (value is int intValue && intValue == 0)
                return string.Empty;
            else if (value is long longValue && longValue == 0)
                return string.Empty;

            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType == typeof(int))
            {
                if (int.TryParse(value.ToString(), out var intValue) == true)
                    return intValue;
            }
            else if (targetType == typeof(long))
            {
                if (long.TryParse(value.ToString(), out var intValue) == true)
                    return intValue;
            }

            return 0;
        }
    }
}
