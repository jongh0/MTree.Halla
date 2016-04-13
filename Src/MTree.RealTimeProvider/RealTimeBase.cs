using MTree.DataStructure;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTree.RealTimeProvider
{
    public enum MessageTypes
    {
        KeepAlive,
        CloseClient,
        MasteringDone,
        DaishinSessionDisconnected,
    }

    public enum MarketInfoTypes
    {
        WorkDate,
        StartTime,
        EndTime,
    }

    public class RealTimeBase
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        #region Queue
        protected ConcurrentQueue<BiddingPrice> BiddingPriceQueue { get; } = new ConcurrentQueue<BiddingPrice>();
        protected ConcurrentQueue<CircuitBreak> CircuitBreakQueue { get; } = new ConcurrentQueue<CircuitBreak>();
        protected ConcurrentQueue<StockConclusion> StockConclusionQueue { get; } = new ConcurrentQueue<StockConclusion>();
        protected ConcurrentQueue<IndexConclusion> IndexConclusionQueue { get; } = new ConcurrentQueue<IndexConclusion>();
        #endregion

        #region Queue Count
        public int BiddingPriceQueueCount { get { return BiddingPriceQueue.Count; } }
        public int CircuitBreakQueueCount { get { return CircuitBreakQueue.Count; } }
        public int StockConclusionQueueCount { get { return StockConclusionQueue.Count; } }
        public int IndexConclusionQueueCount { get { return IndexConclusionQueue.Count; } }
        #endregion

        #region Queue Task
        protected CancellationTokenSource QueueTaskCancelSource { get; } = new CancellationTokenSource();
        protected CancellationToken QueueTaskCancelToken { get; set; } 
        #endregion

        public RealTimeBase()
        {
            QueueTaskCancelToken = QueueTaskCancelSource.Token;
        }

        protected void StopQueueTask()
        {
            QueueTaskCancelSource.Cancel();
            logger.Info($"[{GetType().Name}] Queue task stopped");
        }

        protected void WaitQueueTask()
        {
            logger.Info($"[{GetType().Name}] Wait queue task done");

            while (BiddingPriceQueue.Count > 0 ||
                   CircuitBreakQueue.Count > 0 ||
                   StockConclusionQueue.Count > 0 ||
                   IndexConclusionQueue.Count > 0)
            {
                Thread.Sleep(100);
            }

            logger.Info($"[{GetType().Name}] Queue task done");
        }
    }
}
