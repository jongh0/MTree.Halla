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
using Strategy;
using Trader.Account;
using Strategy.TradeAvailable;
using Strategy.TradeDecision;

namespace StrategyManager
{
    public class StrategyManager_
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private List<string> _concernCodeList = new List<string>();

        private RealTimeConsumer _realTime;
        private HistoryConsumer _history;
        private RealTimeTrader _trader;

        private ITradeAvailable _tradeAvailable;
        private ITradeDecision _tradeDesicion;

        private TradeInformation _tradeInfos;

        public StrategyManager_()
        {
            AddConcernCode();
            BuildStrategy();
            InitializeWCF();
        }

        private void AddConcernCode()
        {
            _concernCodeList.Add("005930"); // 삼성전자
            _concernCodeList.Add("035420"); // Naver
        }

        private void BuildStrategy()
        {
            _tradeInfos = new TradeInformation();

            var availableComposite = new TradeAvailableComposite();
            availableComposite.Logic = LogicTypes.AND;
            availableComposite.Add(new DayTradeAvailable());
            availableComposite.Add(new AccountAvailable());

            _tradeAvailable = availableComposite;
            _tradeDesicion = new EvenOddTradeDecision();
        }

        #region WCF Initialization
        private void InitializeWCF()
        {
            try
            {
                _history = new HistoryConsumer();

#if true // Strategy & Trader만 테스트할 때는 Block
                if (ProcessUtility.WaitIfNotExists(ProcessTypes.RealTimeProvider) == false)
                {
                    _logger.Error($"{nameof(ProcessTypes.RealTimeProvider)} process not exists");
                    return;
                }

                _realTime = new RealTimeConsumer();
                _realTime.ChannelOpened += RealTime_ChannelOpened;
                _realTime.MessageNotified += RealTime_MessageNotified;
                _realTime.StockMasterConsumed += RealTime_StockMasterConsumed;
                _realTime.StockConclusionConsumed += RealTime_StockConclusionConsumed;
#endif

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
                _trader.ChannelOpened += Trader_ChannelOpened;
                _trader.MessageNotified += Trader_MessageNotified;
                _trader.OrderResultNotified += Trader_OrderResultNotified;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        } 
        #endregion

        #region Trader Event
        private void Trader_OrderResultNotified(StockOrderResult result)
        {
        }

        private void Trader_MessageNotified(MessageTypes type, string message)
        {
        }

        private void Trader_ChannelOpened(RealTimeTrader trader)
        {
            if (trader == null) return;

            trader.RegisterTraderContract(new TraderContract());

            var accounts = trader.GetAccountInformations();
            if (accounts == null)
            {
                _logger.Error($"{nameof(accounts)} is null");
                return;
            }

            _tradeInfos.Account = accounts.FirstOrDefault(); // 보유 계좌 중 첫 번째걸로 사용

            foreach (var account in accounts)
            {
                _logger.Info(account.ToString());
            }
        } 
        #endregion

        #region RealTime Event
        private void RealTime_StockMasterConsumed(List<StockMaster> stockMasters)
        {
        }

        private void RealTime_StockConclusionConsumed(StockConclusion conclusion)
        {
            _tradeInfos.Subscribable = conclusion;

            switch (_tradeDesicion.GetTradeType(_tradeInfos))
            {
                case TradeTypes.Buy:
                    TryBuyStock(_tradeInfos);
                    break;

                case TradeTypes.Sell:
                    TrySellStock(_tradeInfos);
                    break;
            }
        }

        private void RealTime_MessageNotified(MessageTypes type, string message)
        {
            if (type == MessageTypes.CloseClient)
            {
                Task.Run(() =>
                {
                    _trader.SendMessage(MessageTypes.CloseClient, message);

                    _logger.Info("Process will be closed");
                    Thread.Sleep(1000 * 5);

                    Environment.Exit(0);
                });
            }
        }

        private void RealTime_ChannelOpened(RealTimeConsumer consumer)
        {
            if (consumer == null) return;

            consumer.RegisterContract(new SubscribeContract(SubscribeTypes.StockConclusion, SubscribeScopes.Partial, _concernCodeList));
        } 
        #endregion

        private void TryBuyStock(TradeInformation info)
        {
            try
            {
                var order = new StockOrder();
                info.Account.CopyTo(order);
                order.OrderType = OrderTypes.BuyNew;
                order.Code = info.Subscribable.Code;
                order.Quantity = 1;
                order.PriceType = PriceTypes.MarketPrice; // 시장가

                info.Order = order;

                if (_tradeAvailable.CanBuy(info) == false) return;

                _trader.MakeOrder(order);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private void TrySellStock(TradeInformation info)
        {
            try
            {
                var order = new StockOrder();
                info.Account.CopyTo(order);
                order.OrderType = OrderTypes.SellNew;
                order.Code = info.Subscribable.Code;
                order.Quantity = 1;
                order.PriceType = PriceTypes.MarketPrice; // 시장가

                info.Order = order;

                if (_tradeAvailable.CanSell(info) == false) return;

                _trader.MakeOrder(order);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }
    }
}
