using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Utility
{
    public static class AssemblyUtility
    {
        public static string VersionName
        {
            get
            {
                var ver = Assembly.GetEntryAssembly().GetName().Version;
                return $"v{ver.Major}.{ver.Minor}.{ver.Build}";
            }
        }
    }
}
