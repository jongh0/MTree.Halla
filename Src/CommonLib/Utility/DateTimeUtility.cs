using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Utility
{
    public class DateTimeUtility
    {
        public static DateTime StartDateTime(DateTime dateTime)
        {
            if (dateTime == null) dateTime = DateTime.Now;

            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
        }

        public static DateTime EndDateTime(DateTime dateTime)
        {
            if (dateTime == null) dateTime = DateTime.Now;

            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day).AddDays(1).AddMilliseconds(-1);
        }

        public static DateTime DateOnly(DateTime dateTime)
        {
            if (dateTime == null) dateTime = DateTime.Now;

            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
        }
    }
}
