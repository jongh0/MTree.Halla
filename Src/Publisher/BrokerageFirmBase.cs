﻿using Configuration;
using DataStructure;
using CommonLib;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using FirmLib;
using CommonLib.Utility;

namespace Publisher
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class BrokerageFirmBase : PublisherBase
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        // Login
        public LoginInformation LoginInfo { get; } = new LoginInformation();
        private int WaitLoginTimeout => 1000 * 60;
        private ManualResetEvent WaitLoginEvent { get; } = new ManualResetEvent(false);

        // TestMode Lock
        protected object ConclusionLock { get; } = new object();
        protected object BiddingLock { get; } = new object();

        // Quote interval
        protected int QuoteInterval { get; set; } = 0;

        // Quote lock
        protected int QuoteLockTimeout => 1000 * 15;
        protected object QuoteLock { get; } = new object();

        // Quoting timeout
        private int WaitQuotingTimeout => 1000 * 15;
        private AutoResetEvent WaitQuotingEvent { get; } = new AutoResetEvent(false);

        // Quoting instance
        protected StockMaster QuotingStockMaster { get; set; } = null;
        protected IndexMaster QuotingIndexMaster { get; set; } = null;

        protected ConcurrentDictionary<string, IndexConclusion> PrevIndexConclusions { get; } = new ConcurrentDictionary<string, IndexConclusion>();

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
        protected void StartEtfConclusionQueueTask()
        {
            TaskUtility.Run($"{GetType().Name}.EtfConclusionQueue", QueueTaskCancelToken, ProcessETFConclusionQueue);
        }
        private void ProcessBiddingPriceQueue()
        {
            try
            {
                if (ServiceClient?.State == CommunicationState.Opened &&
                    BiddingPriceQueue.TryDequeue(out var biddingPrice) == true)
                {
                    ServiceClient.PublishBiddingPrice(biddingPrice);
                }
                else
                {
                    Thread.Sleep(10);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private void ProcessCircuitBreakQueue()
        {
            try
            {
                if (ServiceClient?.State == CommunicationState.Opened &&
                    CircuitBreakQueue.TryDequeue(out var circuitBreak) == true)
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
                _logger.Error(ex);
            }
        }

        private void ProcessStockConclusionQueue()
        {
            try
            {
                if (ServiceClient?.State == CommunicationState.Opened &&
                    StockConclusionQueue.TryDequeue(out var conclusion) == true)
                {
                    ServiceClient.PublishStockConclusion(conclusion);
                }
                else
                {
                    Thread.Sleep(10);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private void ProcessIndexConclusionQueue()
        {
            try
            {
                if (ServiceClient?.State == CommunicationState.Opened &&
                    IndexConclusionQueue.TryDequeue(out var conclusion) == true)
                {
                    ServiceClient.PublishIndexConclusion(conclusion);
                }
                else
                {
                    Thread.Sleep(10);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }
        private void ProcessETFConclusionQueue()
        {
            try
            {
                if (ServiceClient?.State == CommunicationState.Opened &&
                    ETFConclusionQueue.TryDequeue(out var conclusion) == true)
                {
                    ServiceClient.PublishETFConclusion(conclusion);
                }
                else
                {
                    Thread.Sleep(10);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        protected bool WaitQuoting()
        {
            if (WaitQuotingEvent.WaitOne(WaitQuotingTimeout) == false)
            {
                _logger.Warn($"{GetType().Name} wait quoting timeout");
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
                _logger.Error($"{GetType().Name} wait login timeout");
                return false;
            }

            return true;
        }

        protected void SetLogin()
        {
            Thread.Sleep(1000 * 3); // 로그인후 대기

            _logger.Info($"{GetType().Name} set login");
            WaitLoginEvent.Set();
        }

        protected void WaitQuoteInterval()
        {
            if (QuoteInterval > 0)
                Thread.Sleep(QuoteInterval);
        }
    }
}
