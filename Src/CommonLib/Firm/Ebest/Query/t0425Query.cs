using CommonLib.Firm.Ebest.Block;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XA_DATASETLib;

namespace CommonLib.Firm.Ebest.Query
{
    public class t0425Query : QueryBase
    {
        public override string ResName => "t0425";

        public event Action<t0425OutBlock> BlockReceived;
        public event Action<t0425OutBlock1> Block1Received;

        protected override void OnReceiveData(string trCode)
        {
            base.OnReceiveData(trCode);

            if (BlockReceived != null && Query.GetFieldData(out t0425OutBlock block) == true)
                BlockReceived.Invoke(block);

            if (Block1Received != null && Query.GetFieldData(out t0425OutBlock1 block1) == true)
                Block1Received.Invoke(block1);
        }
    }
}
