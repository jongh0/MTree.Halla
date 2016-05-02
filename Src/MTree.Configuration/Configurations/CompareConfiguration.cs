using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.Configuration
{
    public class ValidatorConfiguration
    {
        public string BeyondComparePath { get; set; } = @"C:\Program Files (x86)\Beyond Compare 3\BComp.exe";

        public bool UseSimultaneousCompare { get; set; } = false;

        public int ThreaLimit { get; set; } = 4;
    }
}
