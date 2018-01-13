using CommonLib.Firm.Ebest.Block;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Firm.Ebest.Real
{
    public class DVIReal : RealBase
    {
        public override string ResName => "DVI";

        public event Action<DVIOutBlock> BlockReceived;

        protected override void OnReceiveRealData(string trCode)
        {
            base.OnReceiveRealData(trCode);

            if (BlockReceived != null && Real.GetFieldData(out DVIOutBlock block) == true)
                BlockReceived.Invoke(block);
        }
    }
}
