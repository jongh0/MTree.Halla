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

            switch (Config.General.TraderType)
            {
                case TraderTypes.Ebest:
                case TraderTypes.EbestSimul:
                    TradeHandler = new TradeHandler("EbestTraderConfig");
                    break;

                case TraderTypes.Kiwoom:
                case TraderTypes.KiwoomSimul:
                    TradeHandler = new TradeHandler("KiwoomTraderConfig");
                    break;

                default:
                    TradeHandler = new TradeHandler("VirtualTraderConfig");
                    break;
            }
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
