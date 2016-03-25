using System;
using System.ServiceModel;
using MongoDB.Driver;
using MTree.DbProvider;
using System.Threading;
using MTree.DataStructure;
using MTree.Configuration;
using MTree.Consumer;
using MTree.Utility;
using MTree.RealTimeProvider;
using System.Collections.Concurrent;
using System.Text;

namespace MTree.HistorySaver
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class HistorySaver : ConsumerBase
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public HistorySaver()
        {
            try
            {
                TaskUtility.Run("HistorySaver.BiddingPriceQueue", QueueTaskCancelToken, ProcessBiddingPriceQueue);
                TaskUtility.Run("HistorySaver.CircuitBreakQueue", QueueTaskCancelToken, ProcessCircuitBreakQueue);
                TaskUtility.Run("HistorySaver.StockConclusionQueue", QueueTaskCancelToken, ProcessStockConclusionQueue);
                TaskUtility.Run("HistorySaver.IndexConclusionQueue", QueueTaskCancelToken, ProcessIndexConclusionQueue);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        protected override void ServiceClient_Opened(object sender, EventArgs e)
        {
            base.ServiceClient_Opened(sender, e);

            ServiceClient.RegisterContract(ClientId, new SubscribeContract(SubscribeTypes.StockMaster));
            ServiceClient.RegisterContract(ClientId, new SubscribeContract(SubscribeTypes.CircuitBreak));
            ServiceClient.RegisterContract(ClientId, new SubscribeContract(SubscribeTypes.BiddingPrice));
            ServiceClient.RegisterContract(ClientId, new SubscribeContract(SubscribeTypes.StockConclusion));
            ServiceClient.RegisterContract(ClientId, new SubscribeContract(SubscribeTypes.IndexConclusion));
        }

        private void ProcessBiddingPriceQueue()
        {
            try
            {
                BiddingPrice item;
                if (BiddingPriceQueue.TryDequeue(out item) == true)
                    DbAgent.Instance.InsertItem(item);
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
                CircuitBreak item;
                if (CircuitBreakQueue.TryDequeue(out item) == true)
                    DbAgent.Instance.InsertItem(item);
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
                StockConclusion item;
                if (StockConclusionQueue.TryDequeue(out item) == true)
                    DbAgent.Instance.InsertItem(item);
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
                IndexConclusion item;
                if (IndexConclusionQueue.TryDequeue(out item) == true)
                    DbAgent.Instance.InsertItem(item);
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

        public override void ConsumeStockMaster(StockMaster stockMaster)
        {
            int startTick = Environment.TickCount;

            try
            {
                DbAgent.Instance.InsertItem(stockMaster);
                logger.Info($"{stockMaster.Code} stock master saved, Tick: {Environment.TickCount - startTick}");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public override void NotifyMessage(MessageTypes type, string message)
        {
            try
            {
                if (type == MessageTypes.CloseClient)
                {
                    var sb = new StringBuilder();
                    sb.AppendLine("HistorySaver result");
                    sb.AppendLine($"StockMaster : {DbAgent.Instance.StockMasterCount}");
                    sb.AppendLine($"StockConclusion : {DbAgent.Instance.StockConclusionCount}");
                    sb.AppendLine($"IndexConclusion : {DbAgent.Instance.IndexConclusionCount}");
                    sb.AppendLine($"BiddingPrice : {DbAgent.Instance.BiddingPriceCount}");
                    sb.AppendLine($"CircuitBreak : {DbAgent.Instance.CircuitBreakCount}");

                    logger.Info(sb.ToString());
                    PushUtility.NotifyMessage(sb.ToString());
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
