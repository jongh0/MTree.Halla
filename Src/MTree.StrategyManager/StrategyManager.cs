using MTree.Configuration;
using MTree.Consumer;
using MTree.DataStructure;
using MTree.RealTimeProvider;
using MTree.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTree.StrategyManager
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class StrategyManager : ConsumerBase
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public List<string> ConcernCodeList { get; set; } = new List<string>();

        public DbHandler DbHandler { get; set; }
        public TradeHandler TradeHandler { get; set; }

        public StrategyManager()
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
                BiddingPrice biddingPrice;
                if (BiddingPriceQueue.TryDequeue(out biddingPrice) == true)
                {
                }
                else
                    Thread.Sleep(10);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private void ProcessCircuitBreakQueue()
        {
            try
            {
                CircuitBreak circuitBreak;
                if (CircuitBreakQueue.TryDequeue(out circuitBreak) == true)
                {
                }
                else
                    Thread.Sleep(10);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private void ProcessStockConclusionQueue()
        {
            try
            {
                StockConclusion conclusion;
                if (StockConclusionQueue.TryDequeue(out conclusion) == true)
                {
                }
                else
                    Thread.Sleep(10);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private void ProcessIndexConclusionQueue()
        {
            try
            {
                IndexConclusion conclusion;
                if (IndexConclusionQueue.TryDequeue(out conclusion) == true)
                {
                }
                else
                    Thread.Sleep(10);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public override void ConsumeBiddingPrice(BiddingPrice biddingPrice)
        {
            BiddingPriceQueue.Enqueue(biddingPrice);
        }

        public override void ConsumeStockConclusion(StockConclusion conclusion)
        {
            StockConclusionQueue.Enqueue(conclusion);
        }

        public override void ConsumeIndexConclusion(IndexConclusion conclusion)
        {
            IndexConclusionQueue.Enqueue(conclusion);
        }

        public override void ConsumeCircuitBreak(CircuitBreak circuitBreak)
        {
            CircuitBreakQueue.Enqueue(circuitBreak);
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

                        logger.Info("Process will be closed");
                        Thread.Sleep(1000 * 5);

                        Environment.Exit(0);
                    });
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            base.NotifyMessage(type, message);
        }
    }
}
