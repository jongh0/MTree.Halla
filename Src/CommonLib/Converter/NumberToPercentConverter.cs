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
    public class NumberToPercentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return string.Empty;

            if (value is int)
                return ((int)value).ToString(Config.General.PercentFormat);
            else if (value is long)
                return ((long)value).ToString(Config.General.PercentFormat);
            else if (value is float)
                return ((float)value).ToString(Config.General.PercentFormat);
            else if (value is double)
                return ((double)value).ToString(Config.General.PercentFormat);
            else if (value is short)
                return ((short)value).ToString(Config.General.PercentFormat);
            else
                return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
