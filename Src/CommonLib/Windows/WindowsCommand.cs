using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Windows
{
    public class WindowsCommand
    {
        public static void Shutdown()
        {
            Process.Start("shutdown", "/s /t 0");
        }
    }
}
