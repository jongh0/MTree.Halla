#define VERIFY_LATENCY

using MTree.Configuration;
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
using System.Timers;

namespace MTree.Publisher
{
    public class BrokerageFirmBase : PublisherBase
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        // Login
        public LoginInfo LoginInstance { get; } = new LoginInfo();
        private int WaitLoginTimeout { get; } = 1000 * 60;
        private ManualResetEvent WaitLoginEvent { get; } = new ManualResetEvent(false);

        // TestMode Lock
        protected object ConclusionLock { get; } = new object();
        protected object BiddingLock { get; } = new object();

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

#if VERIFY_LATENCY
        public DataCounter Counter { get; set; } = new DataCounter(DataTypes.DaishinPublisher);

        public TrafficMonitor TrafficMonitor { get; set; }
#endif

        protected void StartBiddingPriceQueueTask()
        {
            if (Config.General.SkipBiddingPrice == false)
                TaskUtility.Run($"{GetType().Name}.BiddingPriceQueue", QueueTaskCancelToken, ProcessBiddingPriceQueue);
        }

        protected void StartCircuitBreakQueueTask()
        {
            TaskUtility.Run($"{GetType().Name}.CircuitBreakQueue", QueueTaskCancelToken, ProcessCircuitBreakQueue);
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
                {
#if VERIFY_LATENCY
                    TrafficMonitor.CheckLatency(biddingPrice); 
#endif
                    ServiceClient.PublishBiddingPrice(biddingPrice);
                }
                else
                {
                    Thread.Sleep(10);
                }
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
                if (ServiceClient.State == CommunicationState.Opened &&
                    CircuitBreakQueue.TryDequeue(out circuitBreak) == true)
                {
                    ServiceClient.PublishCircuitBreak(circuitBreak);
                }
                else
                {
                    Thread.Sleep(10);
                }
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
                {
#if VERIFY_LATENCY
                    TrafficMonitor.CheckLatency(conclusion); 
#endif
                    ServiceClient.PublishStockConclusion(conclusion);
                }
                else
                {
                    Thread.Sleep(10);
                }
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
                {
#if VERIFY_LATENCY
                    TrafficMonitor.CheckLatency(conclusion); 
#endif
                    ServiceClient.PublishIndexConclusion(conclusion);
                }
                else
                {
                    Thread.Sleep(10);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        protected bool WaitQuoting()
        {
            if (WaitQuotingEvent.WaitOne(WaitQuotingTimeout) == false)
            {
                logger.Error($"{GetType().Name} wait quoting timeout");
                return false;
            }

            return true;
        }

        protected void SetQuoting()
        {
            WaitQuotingEvent.Set();
        }

        protected bool WaitLogin()
        {
            if (WaitLoginEvent.WaitOne(WaitLoginTimeout) == false)
            {
                logger.Error($"{GetType().Name} wait login timeout");
                return false;
            }

            return true;
        }

        protected void SetLogin()
        {
            Thread.Sleep(1000 * 3); // 로그인후 대기

            logger.Info($"{GetType().Name} set login");
            WaitLoginEvent.Set();
        }

        protected void WaitQuoteInterval()
        {
            if (QuoteInterval > 0)
                Thread.Sleep(QuoteInterval);
        }
    }
}
