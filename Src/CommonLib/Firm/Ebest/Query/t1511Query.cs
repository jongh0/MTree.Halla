using CommonLib.Firm.Ebest.Block;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XA_DATASETLib;

namespace CommonLib.Firm.Ebest.Query
{
    public class t1511Query : QueryBase
    {
        public override string ResName => "t1511";

        public event Action<t1511OutBlock> BlockReceived;

        protected override void OnReceiveData(string trCode)
        {
            base.OnReceiveData(trCode);

            if (BlockReceived != null && Query.GetFieldData(out t1511OutBlock block) == true)
                BlockReceived?.Invoke(block);
        }
    }
}
