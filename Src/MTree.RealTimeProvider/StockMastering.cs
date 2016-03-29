using MTree.DataStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.RealTimeProvider
{
    public class StockMastering
    {
        public MasteringStates DaishinState { get; set; } = MasteringStates.Ready;
        public MasteringStates EbestState { get; set; } = MasteringStates.Ready;
        public MasteringStates KiwoomState { get; set; } = MasteringStates.Ready;

        public StockMaster Stock { get; } = new StockMaster();
    }
}
