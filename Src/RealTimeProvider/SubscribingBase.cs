﻿using DataStructure;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RealTimeProvider
{
    public class SubscribingBase
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        #region Queue
        public ConcurrentQueue<BiddingPrice> BiddingPriceQueue { get; } = new ConcurrentQueue<BiddingPrice>();
        public ConcurrentQueue<CircuitBreak> CircuitBreakQueue { get; } = new ConcurrentQueue<CircuitBreak>();
        public ConcurrentQueue<StockConclusion> StockConclusionQueue { get; } = new ConcurrentQueue<StockConclusion>();
        public ConcurrentQueue<IndexConclusion> IndexConclusionQueue { get; } = new ConcurrentQueue<IndexConclusion>();
        public ConcurrentQueue<ETFConclusion> ETFConclusionQueue { get; } = new ConcurrentQueue<ETFConclusion>();
        #endregion

        #region Queue Count
        public int BiddingPriceQueueCount => BiddingPriceQueue.Count;
        public int CircuitBreakQueueCount => CircuitBreakQueue.Count;
        public int StockConclusionQueueCount => StockConclusionQueue.Count;
        public int IndexConclusionQueueCount => IndexConclusionQueue.Count;
        public int ETFConclusionQueueCount => ETFConclusionQueue.Count;
        #endregion

        #region Queue Task
        protected CancellationTokenSource QueueTaskCancelSource { get; } = new CancellationTokenSource();
        public CancellationToken QueueTaskCancelToken { get; set; }
        #endregion

        private System.Timers.Timer RefreshTimer { get; set; }

        public SubscribingBase()
        {
            QueueTaskCancelToken = QueueTaskCancelSource.Token;
        }

        protected void StopQueueTask()
        {
            QueueTaskCancelSource.Cancel();
            _logger.Info($"[{GetType().Name}] Queue task stopped");
        }

        protected void WaitQueueTask()
        {
            _logger.Info($"[{GetType().Name}] Wait queue task done");

            while (BiddingPriceQueue.Count > 0 ||
                   CircuitBreakQueue.Count > 0 ||
                   StockConclusionQueue.Count > 0 ||
                   IndexConclusionQueue.Count > 0)
            {
                Thread.Sleep(100);
            }

            _logger.Info($"[{GetType().Name}] Queue task done");
        }

        public void StartRefreshTimer()
        {
            if (RefreshTimer == null)
            {
                RefreshTimer = new System.Timers.Timer();
                RefreshTimer.AutoReset = true;
                RefreshTimer.Interval = 1000;
                RefreshTimer.Elapsed += RefreshTimer_Elapsed;
            }

            RefreshTimer?.Start();
        }

        public void StopRefreshTimer()
        {
            RefreshTimer?.Stop();
        }

        public virtual void RefreshTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
        }
    }
}
