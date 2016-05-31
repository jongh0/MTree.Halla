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
    }
}
