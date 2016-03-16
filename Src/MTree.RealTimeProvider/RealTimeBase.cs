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

        protected int MaxCommunicateInterval { get; } = 1000 * 60 * 2;
        protected int LastWcfCommunicateTick { get; set; } = Environment.TickCount; // 마지막으로 WCF 통신을 한 시간
        protected System.Timers.Timer CommunicateTimer { get; set; }

        protected ConcurrentQueue<BiddingPrice> BiddingPriceQueue { get; } = new ConcurrentQueue<BiddingPrice>();
        protected ConcurrentQueue<StockConclusion> StockConclusionQueue { get; } = new ConcurrentQueue<StockConclusion>();
        protected ConcurrentQueue<IndexConclusion> IndexConclusionQueue { get; } = new ConcurrentQueue<IndexConclusion>();

        protected CancellationTokenSource QueueTaskCancelSource { get; } = new CancellationTokenSource();
        protected CancellationToken QueueTaskCancelToken { get; set; }

        public RealTimeBase()
        {
            QueueTaskCancelToken = QueueTaskCancelSource.Token;

            CommunicateTimer = new System.Timers.Timer(MaxCommunicateInterval);
            CommunicateTimer.Elapsed += OnCommunicateTimer;
            CommunicateTimer.AutoReset = true;
        }

        protected void StopQueueTask()
        {
            QueueTaskCancelSource.Cancel();
            logger.Info($"[{GetType().Name}] Queue task stopped");
        }

        protected void StopCommunicateTimer()
        {
            CommunicateTimer.Stop();
            logger.Info($"[{GetType().Name}] Communicate timer stopped");
        }

        protected virtual void OnCommunicateTimer(object sender, System.Timers.ElapsedEventArgs e)
        {
        }
    }
}
