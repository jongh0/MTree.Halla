using MTree.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.StrategyManager
{
    public class StrategyManager
    {
        public RealTimeHandler RealTimeHandle { get; set; }
        public TradeHandler TradeHandle { get; set; }

        public StrategyManager()
        {
            RealTimeHandle = new RealTimeHandler();

            if (Config.General.SimulMode == true)
                TradeHandle = new TradeHandler("SimTraderConfig");
            else
                TradeHandle = new TradeHandler("KiwoomTraderConfig");
        }
    }
}
