﻿using CommonLib.Surrogates;
using DataStructure;
using MongoDB.Bson;
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
        public int BiddingPriceQueueCount { get { return BiddingPriceQueue.Count; } }
        public int CircuitBreakQueueCount { get { return CircuitBreakQueue.Count; } }
        public int StockConclusionQueueCount { get { return StockConclusionQueue.Count; } }
        public int IndexConclusionQueueCount { get { return IndexConclusionQueue.Count; } }
        public int ETFConclusionQueueCount { get { return ETFConclusionQueue.Count; } }
        #endregion

        #region Queue Task
        protected CancellationTokenSource QueueTaskCancelSource { get; } = new CancellationTokenSource();
        public CancellationToken QueueTaskCancelToken { get; set; }
        #endregion

        private System.Timers.Timer RefreshTimer { get; set; }

        public SubscribingBase()
        {
            QueueTaskCancelToken = QueueTaskCancelSource.Token;
            ProtoBuf.Meta.RuntimeTypeModel.Default.Add(typeof(ObjectId), true).SetSurrogate(typeof(ObjectIdSurrogate));
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
