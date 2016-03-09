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
    public class RealTimeBase
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        protected object LockObject { get; } = new object();

        protected string ProcessName { get; set; } = string.Empty;

        protected ConcurrentQueue<BiddingPrice> BiddingPriceQueue { get; } = new ConcurrentQueue<BiddingPrice>();
        protected ConcurrentQueue<StockConclusion> StockConclusionQueue { get; } = new ConcurrentQueue<StockConclusion>();
        protected ConcurrentQueue<IndexConclusion> IndexConclusionQueue { get; } = new ConcurrentQueue<IndexConclusion>();

        protected CancellationTokenSource QueueTaskCancelSource { get; } = new CancellationTokenSource();
        protected CancellationToken QueueTaskCancelToken { get; set; }

        public RealTimeBase() : base()
        {
            Initialize();

            QueueTaskCancelToken = QueueTaskCancelSource.Token;
        }

        protected virtual void Initialize()
        {
        }

        protected void StopQueueTask()
        {
            QueueTaskCancelSource.Cancel();
            logger.Info("Queue task canceled");
        }
    }
}
