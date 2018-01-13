using CommonLib.Firm.Ebest.Block;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Firm.Ebest.Real
{
    public class SC1Real : RealBase
    {
        public override string ResName => "SC1";

        public event Action<SC1OutBlock> BlockReceived;

        protected override void OnReceiveRealData(string trCode)
        {
            base.OnReceiveRealData(trCode);

            if (BlockReceived != null && Real.GetFieldData(out SC1OutBlock block) == true)
                BlockReceived.Invoke(block);
        }
    }
}
