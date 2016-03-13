using MTree.DataStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.RealTimeProvider
{
    public enum MasteringStateType
    {
        Ready,
        Running,
        Finished,
    }

    public class StockMastering
    {
        public MasteringStateType DaishinState { get; set; } = MasteringStateType.Ready;
        public MasteringStateType EbestState { get; set; } = MasteringStateType.Ready;
        public MasteringStateType KiwoomState { get; set; } = MasteringStateType.Ready;
        public MasteringStateType KrxState { get; set; } = MasteringStateType.Ready;
        public MasteringStateType NaverState { get; set; } = MasteringStateType.Ready;

        public StockMaster Stock { get; set; } = new StockMaster();
    }
}
