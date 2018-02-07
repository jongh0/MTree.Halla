using FirmLib.Ebest.Block;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirmLib.Ebest.Real
{
    public class EbestReal<TOutBlock> : RealBase<TOutBlock> where TOutBlock : BlockBase
    {
        public void AdviseRealData()
        {
            Real.AdviseRealData();
        }

        public void AdviseRealData<TInBlock>(TInBlock inBlock) where TInBlock : BlockBase
        {
            if (inBlock != default(TInBlock))
                Real.SetFieldData(inBlock);
            Real.AdviseRealData();
        }

        public void UnadviseRealData()
        {
            Real.UnadviseRealData();
        }

        public void UnadviseRealData<TInBlock>(TInBlock inBlock) where TInBlock : BlockBase
        {
            if (inBlock != default(TInBlock))
                Real.SetFieldData(inBlock);
            Real.UnadviseRealData();
        }
    }
}
