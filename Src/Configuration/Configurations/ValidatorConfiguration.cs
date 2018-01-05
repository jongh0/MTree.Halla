using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Configuration
{
    public class ValidatorConfiguration
    {
        public string BeyondComparePath { get; set; } = @"C:\Program Files (x86)\Beyond Compare 3\BComp.exe";

        public bool LaunchValidatorAfterMarketEnd { get; set; } = true;

        public bool UseSimultaneousCompare { get; set; } = false;

        public int ThreadLimit { get; set; } = 4;
    }
}
