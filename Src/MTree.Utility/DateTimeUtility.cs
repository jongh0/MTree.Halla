using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.Utility
{
    public class DateTimeUtility
    {
        public static DateTime StartDateTime(DateTime dateTime)
        {
            if (dateTime == null) return DateTime.Now;

            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day);
        }

        public static DateTime EndDateTime(DateTime dateTime)
        {
            if (dateTime == null) return DateTime.Now;

            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day).AddDays(1).AddMilliseconds(-1);
        }
    }
}
