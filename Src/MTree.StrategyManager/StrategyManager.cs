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
        public List<string> ConcernCodeList { get; set; } = new List<string>();

        public DbHandler DbHandler { get; set; }
        public RealTimeHandler RealTimeHandler { get; set; }
        public TradeHandler TradeHandler { get; set; }

        public StrategyManager()
        {
            ConcernCodeList.Add("005930"); // 삼성전자
            ConcernCodeList.Add("035420"); // Naver

            DbHandler = new DbHandler();
            DbHandler.BiddingPriceReceived += BiddingPriceReceived;
            DbHandler.CircuitBreakReceived += CircuitBreakReceived;
            DbHandler.ConclusionReceived += ConclusionReceived;

            RealTimeHandler = new RealTimeHandler(ConcernCodeList);
            RealTimeHandler.BiddingPriceReceived += BiddingPriceReceived;
            RealTimeHandler.CircuitBreakReceived += CircuitBreakReceived;
            RealTimeHandler.ConclusionReceived += ConclusionReceived;
            RealTimeHandler.MessageReceived += MessageReceived;

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

        private void MessageReceived(object sender, Consumer.MessageReceivedEventArgs e)
        {
        }

        private void ConclusionReceived(object sender, Consumer.SubscribableNotifiedEventArgs e)
        {
        }

        private void CircuitBreakReceived(object sender, Consumer.SubscribableNotifiedEventArgs e)
        {
        }

        private void BiddingPriceReceived(object sender, Consumer.SubscribableNotifiedEventArgs e)
        {
        }
    }
}
