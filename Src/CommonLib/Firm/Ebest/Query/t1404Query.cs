using CommonLib.Firm.Ebest.Block;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XA_DATASETLib;

namespace CommonLib.Firm.Ebest.Query
{
    public class t1404Query : QueryBase
    {
        public override string ResName => "t1404";

        public event Action<t1404OutBlock> BlockReceived;
        public event Action<t1404OutBlock1> Block1Received;

        protected override void OnReceiveData(string trCode)
        {
            base.OnReceiveData(trCode);

            if (BlockReceived != null && Query.GetFieldData(out t1404OutBlock block) == true)
                BlockReceived.Invoke(block);

            if (Block1Received != null && Query.GetFieldData(out t1404OutBlock1 block1) == true)
                Block1Received.Invoke(block1);
        }
    }
}
