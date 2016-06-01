using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.Utility
{
    public class ConvertUtility
    {
        public static int ToInt32(string value, int defaultValue = 0)
        {
            int result;
            if (int.TryParse(value, out result) == false)
                result = defaultValue;

            return result;
        }

        public static uint ToUInt32(string value, uint defaultValue = 0)
        {
            uint result;
            if (uint.TryParse(value, out result) == false)
                result = defaultValue;

            return result;
        }

        public static long ToInt64(string value, long defaultValue = 0)
        {
            long result;
            if (long.TryParse(value, out result) == false)
                result = defaultValue;

            return result;
        }

        public static ulong ToUInt64(string value, ulong defaultValue = 0)
        {
            ulong result;
            if (ulong.TryParse(value, out result) == false)
                result = defaultValue;

            return result;
        }

        public static float ToSingle(string value, float defaultValue = 0)
        {
            float result;
            if (float.TryParse(value, out result) == false)
                result = defaultValue;

            return result;
        }

        public static double ToDouble(string value, double defaultValue = 0)
        {
            double result;
            if (double.TryParse(value, out result) == false)
                result = defaultValue;

            return result;
        }
    }
}
