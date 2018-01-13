using CommonLib.Firm.Ebest.Block;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XA_DATASETLib;

namespace CommonLib.Firm.Ebest.Query
{
    public class t0424Query : QueryBase
    {
        public override string ResName => "t0424";

        public event Action<t0424OutBlock> BlockReceived;
        public event Action<t0424OutBlock1> Block1Received;

        protected override void OnReceiveData(string trCode)
        {
            base.OnReceiveData(trCode);

            if (BlockReceived != null && Query.GetFieldData(out t0424OutBlock block) == true)
                BlockReceived.Invoke(block);

            if (Block1Received != null && Query.GetFieldData(out t0424OutBlock1 block1) == true)
                Block1Received.Invoke(block1);
        }
    }
}
