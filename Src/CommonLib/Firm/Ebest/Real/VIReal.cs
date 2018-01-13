using CommonLib.Firm.Ebest.Block;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Firm.Ebest.Real
{
    public class VIReal : RealBase
    {
        public override string ResName => "VI";

        public event Action<VIOutBlock> BlockReceived;

        protected override void OnReceiveRealData(string trCode)
        {
            base.OnReceiveRealData(trCode);

            if (BlockReceived != null && Real.GetFieldData(out VIOutBlock block) == true)
                BlockReceived.Invoke(block);
        }
    }
}
