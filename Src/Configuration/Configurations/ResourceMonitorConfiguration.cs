using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration
{
    public class ResourceMonitorConfiguration
    {
        public bool UseResourceMonitor { get; set; } = false;

        public int SamplingFrequence { get; set; } = 5;
    }
}
