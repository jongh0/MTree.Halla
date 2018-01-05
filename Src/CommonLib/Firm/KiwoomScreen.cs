using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib
{
    public class KiwoomScreen
    {
        private static int screenNum = 5000;
        public static string GetScreenNum()
        {
            if (screenNum < 9999)
                screenNum++;
            else
                screenNum = 5000;

            return screenNum.ToString();
        }
    }
}
