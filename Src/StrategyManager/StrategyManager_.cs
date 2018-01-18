using Configuration;
using Consumer;
using DataStructure;
using RealTimeProvider;
using CommonLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trader;
using CommonLib.Utility;
using System.Diagnostics;

namespace StrategyManager
{
    public class StrategyManager_
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private List<string> _concernCodeList = new List<string>();

        private RealTimeConsumer _realTimeConsumer;
        private HistoryConsumer _historyConsumer;
        private RealTimeTrader _trader;

        public StrategyManager_()
        {
            _concernCodeList.Add("005930"); // 삼성전자
            _concernCodeList.Add("035420"); // Naver

            _historyConsumer = new HistoryConsumer();
            _realTimeConsumer = new RealTimeConsumer();
            _realTimeConsumer.ChannelOpened += RealTimeConsumer_ChannelOpened;

            var traderConfiguration = string.Empty;

            switch (Config.General.TraderType)
            {
                case TraderTypes.Ebest:
                case TraderTypes.EbestSimul:
                    traderConfiguration = "EbestTraderConfig";
                    break;
                case TraderTypes.Kiwoom:
                case TraderTypes.KiwoomSimul:
                    traderConfiguration = "EbestTraderConfig";
                    break;
                case TraderTypes.Virtual:
                    traderConfiguration = "EbestTraderConfig";
                    break;
            }

            _trader = new RealTimeTrader(traderConfiguration);
        }

        private void RealTimeConsumer_ChannelOpened(RealTimeConsumer consumer)
        {
            if (consumer == null) return;

            consumer.RegisterContract(new SubscribeContract(SubscribeTypes.StockConclusion, SubscribeScopes.Partial, _concernCodeList));
        }
    }
}
