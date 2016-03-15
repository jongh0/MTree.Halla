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

        // Login
        protected LoginInfo LoginInstance { get; } = new LoginInfo();
        private int WaitLoginTimeout { get; } = 1000 * 10;
        private ManualResetEvent WaitLoginEvent { get; } = new ManualResetEvent(false);

        // Last firm communication tick
        protected int LastFirmCommunicateTick { get; set; } = Environment.TickCount;

        // Quote interval
        protected int QuoteInterval { get; set; } = 0;

        // Quote lock
        protected int QuoteLockTimeout { get; } = 1000 * 10;
        protected object QuoteLock { get; } = new object();

        // Quoting timeout
        private int WaitQuotingTimeout { get; } = 1000 * 10;
        private AutoResetEvent WaitQuotingEvent { get; } = new AutoResetEvent(false);

        // Quoting instance
        protected StockMaster QuotingStockMaster { get; set; } = null;
        protected IndexMaster QuotingIndexMaster { get; set; } = null;

        protected ConcurrentDictionary<string, IndexConclusion> PrevIndexConclusions { get; } = new ConcurrentDictionary<string, IndexConclusion>();

        public BrokerageFirmBase() : base()
        {
        }

        protected void StartBiddingPriceQueueTask()
        {
            TaskUtility.Run($"{GetType().Name}.BiddingPriceQueue", QueueTaskCancelToken, ProcessBiddingPriceQueue);
        }

        protected void StartStockConclusionQueueTask()
        {
            TaskUtility.Run($"{GetType().Name}.StockConclusionQueue", QueueTaskCancelToken, ProcessStockConclusionQueue);
        }

        protected void StartIndexConclusionQueueTask()
        {
            TaskUtility.Run($"{GetType().Name}.IndexConclusionQueue", QueueTaskCancelToken, ProcessIndexConclusionQueue);
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

        protected bool WaitQuoting()
        {
            return WaitQuotingEvent.WaitOne(WaitQuotingTimeout);
        }

        protected void SetQuoting()
        {
            WaitQuotingEvent.Set();
        }

        protected bool WaitLogin()
        {
            return WaitLoginEvent.WaitOne(WaitLoginTimeout);
        }

        protected void SetLogin()
        {
            WaitLoginEvent.Set();
        }

        protected void WaitQuoteInterval()
        {
            if (QuoteInterval > 0)
                Thread.Sleep(QuoteInterval);
        }
    }
}
