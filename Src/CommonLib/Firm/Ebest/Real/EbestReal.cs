using CommonLib.Firm.Ebest.Block;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Firm.Ebest.Real
{
    public class EbestReal<TOutBlock> : RealBase<TOutBlock> 
        where TOutBlock : BlockBase
    {
        public void AdviseRealData()
        {
            Real?.AdviseRealData();
        }

        public void UnadviseRealData()
        {
            Real?.UnadviseRealData();
        }
    }

    public class EbestReal<TInBlock, TOutBlock> : RealBase<TOutBlock> 
        where TInBlock : BlockBase 
        where TOutBlock : BlockBase
    {
        public void AdviseRealData(TInBlock block)
        {
            Real?.SetFieldData(block);
            Real?.AdviseRealData();
        }

        public void UnadviseRealData(TInBlock block)
        {
            Real?.SetFieldData(block);
            Real?.AdviseRealData();
        }
    }
}
