using DataStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeProvider
{
    public class IndexMastering
    {
        public MasteringStates DaishinState { get; set; } = MasteringStates.Ready;

        public IndexMaster Index { get; } = new IndexMaster();
    }
}
