using MTree.DataStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.RealTimeProvider
{
    public class IndexMastering
    {
        public MasteringStates EbestState { get; set; } = MasteringStates.Ready;

        public IndexMaster Index { get; } = new IndexMaster();
    }
}
