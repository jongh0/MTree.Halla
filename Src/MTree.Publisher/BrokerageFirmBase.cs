using MTree.DataStructure;
using MTree.Utility;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTree.Publisher
{
    public class BrokerageFirmBase : PublisherBase
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        protected LoginInfo LoginInstance { get; } = new LoginInfo();
        protected AutoResetEvent WaitQuotingEvent { get; } = new AutoResetEvent(false);

        protected StockMaster QuotingStockMaster { get; set; }
        protected IndexMaster QuotingIndexMaster { get; set; }

        protected ConcurrentDictionary<string, IndexConclusion> PrevIndexConclusions { get; } = new ConcurrentDictionary<string, IndexConclusion>();

        public BrokerageFirmBase() : base()
        {
        }

        protected void StartBiddingPriceQueueTask()
        {
            GeneralTask.Run($"{GetType().Name}.BiddingPriceQueue", QueueTaskCancelToken, ProcessBiddingPriceQueue);
        }

        protected void StartStockConclusionQueueTask()
        {
            GeneralTask.Run($"{GetType().Name}.StockConclusionQueue", QueueTaskCancelToken, ProcessStockConclusionQueue);
        }

        protected void StartIndexConclusionQueueTask()
        {
            GeneralTask.Run($"{GetType().Name}.IndexConclusionQueue", QueueTaskCancelToken, ProcessIndexConclusionQueue);
        }

        private void ProcessBiddingPriceQueue()
        {
            try
            {
                BiddingPrice biddingPrice;
                if (ServiceClient.State == CommunicationState.Opened &&
                    BiddingPriceQueue.TryDequeue(out biddingPrice) == true)
                    ServiceClient.PublishBiddingPrice(biddingPrice);
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
                if (ServiceClient.State == CommunicationState.Opened &&
                    StockConclusionQueue.TryDequeue(out conclusion) == true)
                    ServiceClient.PublishStockConclusion(conclusion);
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
                if (ServiceClient.State == CommunicationState.Opened &&
                    IndexConclusionQueue.TryDequeue(out conclusion) == true)
                    ServiceClient.PublishIndexConclusion(conclusion);
                else
                    Thread.Sleep(10);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
    }
}
