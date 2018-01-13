using CommonLib.Firm.Ebest.Block;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Firm.Ebest.Real
{
    public class EbestReal<TOutBlock> : RealBase<TOutBlock> where TOutBlock : BlockBase
    {
        public event Action<TOutBlock> OutBlockReceived;

        protected override void OnReceiveRealData(string trCode)
        {
            if (OutBlockReceived != null && Real.GetFieldData(out TOutBlock block) == true)
                OutBlockReceived.Invoke(block);
        }
    }
}
