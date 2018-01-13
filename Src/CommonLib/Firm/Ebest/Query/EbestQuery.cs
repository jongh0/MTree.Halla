using CommonLib.Firm.Ebest.Block;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Firm.Ebest.Query
{
    public class EbestQuery<TInBlock, TOutBlock> : QueryBase<TInBlock> 
        where TInBlock : BlockBase where TOutBlock : BlockBase
    {
        public event Action<TOutBlock> OutBlockReceived;

        public TOutBlock OutBlock { get; private set; }

        protected override void OnReceiveData(string trCode)
        {
            if (Query.GetFieldData(out TOutBlock block) == true)
            {
                OutBlock = block;
                OutBlockReceived?.Invoke(block);
            }

            base.OnReceiveData(trCode);
        }
    }

    public class EbestQuery<TInBlock, TOutBlock1, TOutBlock2> : QueryBase<TInBlock> 
        where TInBlock : BlockBase where TOutBlock1 : BlockBase where TOutBlock2 : BlockBase
    {
        public event Action<TOutBlock1> OutBlock1Received;
        public event Action<TOutBlock2> OutBlock2Received;

        public TOutBlock1 OutBlock1 { get; private set; }
        public TOutBlock2 OutBlock2 { get; private set; }

        protected override void OnReceiveData(string trCode)
        {
            if (Query.GetFieldData(out TOutBlock1 block1) == true)
            {
                OutBlock1 = block1;
                OutBlock1Received?.Invoke(block1);
            }

            if (Query.GetFieldData(out TOutBlock2 block2) == true)
            {
                OutBlock2 = block2;
                OutBlock2Received?.Invoke(block2);
            }

            base.OnReceiveData(trCode);
        }
    }
}
