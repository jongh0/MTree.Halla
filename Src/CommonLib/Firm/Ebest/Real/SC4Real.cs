using CommonLib.Firm.Ebest.Block;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Firm.Ebest.Real
{
    public class SC4Real : RealBase
    {
        public override string ResName => "SC4";

        public event Action<SC4OutBlock> BlockReceived;

        protected override void OnReceiveRealData(string trCode)
        {
            base.OnReceiveRealData(trCode);

            if (BlockReceived != null && Real.GetFieldData(out SC4OutBlock block) == true)
                BlockReceived.Invoke(block);
        }
    }
}
