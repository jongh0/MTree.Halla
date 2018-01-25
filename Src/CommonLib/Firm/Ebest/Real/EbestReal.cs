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
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public void AdviseRealData(TInBlock block)
        {
            _logger.Info($"AdviseRealData: {block}");

            Real?.SetFieldData(block);
            Real?.AdviseRealData();
        }

        public void UnadviseRealData(TInBlock block)
        {
            _logger.Info($"UnadviseRealData: {block}");

            Real?.SetFieldData(block);
            Real?.AdviseRealData();
        }
    }
}
