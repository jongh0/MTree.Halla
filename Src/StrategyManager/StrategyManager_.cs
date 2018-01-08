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

namespace StrategyManager
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class StrategyManager_ : RealTimeConsumer
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public List<string> ConcernCodeList { get; set; } = new List<string>();

        public DbHandler DbHandler { get; set; }
        public TradeHandler TradeHandler { get; set; }

        public StrategyManager_()
        {
            ConcernCodeList.Add("005930"); // 삼성전자
            ConcernCodeList.Add("035420"); // Naver

            DbHandler = new DbHandler();

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

            TaskUtility.Run("StrategyManager.CircuitBreakQueue", QueueTaskCancelToken, ProcessCircuitBreakQueue);
            TaskUtility.Run($"StrategyManager.StockConclusionQueue", QueueTaskCancelToken, ProcessStockConclusionQueue);
            TaskUtility.Run($"StrategyManager.IndexConclusionQueue", QueueTaskCancelToken, ProcessIndexConclusionQueue);
            if (Config.General.SkipBiddingPrice == false)
                TaskUtility.Run($"StrategyManager.BiddingPriceQueue", QueueTaskCancelToken, ProcessBiddingPriceQueue);
        }

        private void ProcessBiddingPriceQueue()
        {
            try
            {
                if (BiddingPriceQueue.TryDequeue(out var biddingPrice) == true)
                {
                }
                else
                    Thread.Sleep(10);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private void ProcessCircuitBreakQueue()
        {
            try
            {
                if (CircuitBreakQueue.TryDequeue(out var circuitBreak) == true)
                {
                }
                else
                    Thread.Sleep(10);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private void ProcessStockConclusionQueue()
        {
            try
            {
                if (StockConclusionQueue.TryDequeue(out var conclusion) == true)
                {
                }
                else
                    Thread.Sleep(10);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private void ProcessIndexConclusionQueue()
        {
            try
            {
                if (IndexConclusionQueue.TryDequeue(out var conclusion) == true)
                {
                }
                else
                    Thread.Sleep(10);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        public override void NotifyMessage(MessageTypes type, string message)
        {
            try
            {
                if (type == MessageTypes.CloseClient)
                {
                    Task.Run(() =>
                    {
                        if (message.Equals(ExitProgramTypes.Normal.ToString()) == true)
                        {
                        }

                        _logger.Info("Process will be closed");
                        Thread.Sleep(1000 * 5);

                        Environment.Exit(0);
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            base.NotifyMessage(type, message);
        }
    }
}
