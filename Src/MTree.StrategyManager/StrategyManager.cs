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
        public DbHandler DbHandler { get; set; }
        public RealTimeHandler RealTimeHandler { get; set; }
        public TradeHandler TradeHandler { get; set; }

        public StrategyManager()
        {
            DbHandler = new DbHandler();
            DbHandler.BiddingPriceNotified += BiddingPriceNotified;
            DbHandler.CircuitBreakNotified += CircuitBreakNotified;
            DbHandler.ConclusionNotified += ConclusionNotified;

            RealTimeHandler = new RealTimeHandler();
            RealTimeHandler.BiddingPriceNotified += BiddingPriceNotified;
            RealTimeHandler.CircuitBreakNotified += CircuitBreakNotified;
            RealTimeHandler.ConclusionNotified += ConclusionNotified;

            if (Config.General.VirtualTrade == true)
                TradeHandler = new TradeHandler("VirtualTraderConfig");
            else
                TradeHandler = new TradeHandler("KiwoomTraderConfig");
        }

        private void ConclusionNotified(object sender, Consumer.SubscribableEventArgs e)
        {
        }

        private void CircuitBreakNotified(object sender, Consumer.SubscribableEventArgs e)
        {
        }

        private void BiddingPriceNotified(object sender, Consumer.SubscribableEventArgs e)
        {
        }
    }
}
