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
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public event Action<TOutBlock> OutBlockReceived;

        protected override void OnReceiveRealData(string trCode)
        {
            if (OutBlockReceived != null && Real.GetFieldData(out TOutBlock block) == true)
            {
                _logger.Info($"OnReceiveRealData\n{block.ToString()}");
                OutBlockReceived.Invoke(block);
            }
        }
    }

    public class EbestReal<TInBlock, TOutBlock> : RealBase<TOutBlock> where TInBlock : BlockBase where TOutBlock : BlockBase
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public event Action<TOutBlock> OutBlockReceived;

        protected override void OnReceiveRealData(string trCode)
        {
            if (OutBlockReceived != null && Real.GetFieldData(out TOutBlock block) == true)
            {
                _logger.Info($"OnReceiveRealData\n{block.ToString()}");
                OutBlockReceived.Invoke(block);
            }
        }

        public void AdviseRealData(TInBlock block)
        {
            try
            {
                Real.SetFieldData(block);
                AdviseRealData();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        public void UnadviseRealData(TInBlock block)
        {
            try
            {
                Real.SetFieldData(block);
                UnadviseRealData();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }
    }
}
