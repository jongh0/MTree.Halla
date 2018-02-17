using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Utility
{
    public class PerformanceCounterUtility
    {
        private static PerformanceCounter _cpuCounter;
        private static PerformanceCounter _memCounter;

        public static float CpuUsagePercent => _cpuCounter.NextValue();

        public static float AvailableMemory => _memCounter.NextValue();

        static PerformanceCounterUtility()
        {
            _cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            _memCounter = new PerformanceCounter("Memory", "Available MBytes");
        }
    }
}
