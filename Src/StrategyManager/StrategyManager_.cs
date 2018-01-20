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

            if (ProcessUtility.WaitIfNotExists(ProcessTypes.RealTimeProvider) == false)
            {
                _logger.Error($"{nameof(ProcessTypes.RealTimeProvider)} process not exists");
                return;
            }

            _realTimeConsumer = new RealTimeConsumer();
            _realTimeConsumer.ChannelOpened += RealTimeConsumer_ChannelOpened;
            _realTimeConsumer.MessageNotified += RealTimeConsumer_MessageNotified;
            _realTimeConsumer.StockMasterConsumed += RealTimeConsumer_StockMasterConsumed;
            _realTimeConsumer.StockConclusionConsumed += RealTimeConsumer_StockConclusionConsumed;

            string traderConfiguration;
            ProcessTypes processType;

            switch (Config.General.TraderType)
            {
                case TraderTypes.Ebest:
                case TraderTypes.EbestSimul:
                    traderConfiguration = "EbestTraderConfig";
                    processType = ProcessTypes.EbestTrader;
                    break;

                case TraderTypes.Kiwoom:
                case TraderTypes.KiwoomSimul:
                    traderConfiguration = "KiwoomTraderConfig";
                    processType = ProcessTypes.KiwoomTrader;
                    break;

                default:
                    traderConfiguration = "VirtualTraderConfig";
                    processType = ProcessTypes.VirtualTrader;
                    break;
            }

            if (ProcessUtility.WaitIfNotExists(processType) == false)
            {
                _logger.Error($"{processType} process not exists");
                return;
            }

            _trader = new RealTimeTrader(traderConfiguration);
        }

        private void RealTimeConsumer_StockMasterConsumed(List<StockMaster> stockMasters)
        {
        }

        private void RealTimeConsumer_StockConclusionConsumed(StockConclusion conclusion)
        {
        }

        private void RealTimeConsumer_MessageNotified(MessageTypes type, string message)
        {
            if (type == MessageTypes.CloseClient)
            {
                Task.Run(() =>
                {
                    _logger.Info("Process will be closed");
                    Thread.Sleep(1000 * 5);

                    Environment.Exit(0);
                });
            }
        }

        private void RealTimeConsumer_ChannelOpened(RealTimeConsumer consumer)
        {
            if (consumer == null) return;

            consumer.RegisterContract(new SubscribeContract(SubscribeTypes.StockConclusion, SubscribeScopes.Partial, _concernCodeList));
        }
    }
}
