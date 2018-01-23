using CommonLib;
using CommonLib.Utility;
using Configuration;
using DataStructure;
using RealTimeProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Consumer
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class RealTimeConsumer : ConsumerCallback, IConsumer
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        protected Guid ClientId { get; } = Guid.NewGuid();

        protected InstanceContext CallbackInstance { get; set; }
        public ConsumerClient ServiceClient { get; private set; }

        #region Event
        public event Action<MessageTypes, string> MessageNotified;
        public event Action<BiddingPrice> BiddingPriceConsumed;
        public event Action<CircuitBreak> CircuitBreakConsumed;
        public event Action<IndexConclusion> IndexConclusionConsumed;
        public event Action<StockConclusion> StockConclusionConsumed;
        public event Action<ETFConclusion> ETFConclusionConsumed;
        public event Action<List<StockMaster>> StockMasterConsumed;
        public event Action<List<IndexMaster>> IndexMasterConsumed;
        public event Action<Dictionary<string, object>> CodeMapConsumed;
        public event Action<List<Candle>> ChartConsumed;

        public event Action<RealTimeConsumer> ChannelOpened;
        public event Action<RealTimeConsumer> ChannelClosed;
        public event Action<RealTimeConsumer> ChannelFaulted;
        #endregion

        public RealTimeConsumer()
        {
            try
            {
                CallbackInstance = new InstanceContext(this);

                Task.Run(() =>
                {
                    Thread.Sleep(1000);
                    OpenChannel();
                });
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }
        
        public void OpenChannel()
        {
            try
            {
                _logger.Info($"[{GetType().Name}] Open channel");

                ServiceClient = new ConsumerClient(CallbackInstance, "RealTimeConsumerConfig");
                ServiceClient.InnerChannel.Opened += ServiceClient_Opened;
                ServiceClient.InnerChannel.Closed += ServiceClient_Closed;
                ServiceClient.InnerChannel.Faulted += ServiceClient_Faulted;
                ServiceClient.Open();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        public void CloseChannel()
        {
            try
            {
                if (ServiceClient != null)
                {
                    _logger.Info($"[{GetType().Name}] Close channel");

                    ServiceClient.UnregisterConsumerContractAll(ClientId);
                    ServiceClient.Close();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        protected virtual void ServiceClient_Opened(object sender, EventArgs e)
        {
            _logger.Info($"[{GetType().Name}] Channel opened");
            ChannelOpened?.BeginInvoke(this, null, null);
        }

        protected virtual void ServiceClient_Closed(object sender, EventArgs e)
        {
            _logger.Info($"[{GetType().Name}] Channel closed");
            ChannelClosed?.BeginInvoke(this, null, null);
        }

        protected virtual void ServiceClient_Faulted(object sender, EventArgs e)
        {
            _logger.Error($"[{GetType().Name}] Channel faulted");
            ChannelFaulted?.BeginInvoke(this, null, null);
        }

        public override void NotifyMessage(MessageTypes type, string message)
        {
            _logger.Info($"[{GetType().Name}] NotifyMessage, type: {type.ToString()}, message: {message}");

            try
            {
                MessageNotified?.Invoke(type, message);

                if (type == MessageTypes.CloseClient)
                {
                    Task.Run(() =>
                    {
                        Thread.Sleep(2000);
                        StopQueueTask();
                        CloseChannel();
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        public override void ConsumeStockMaster(List<StockMaster> stockMasters)
        {
            StockMasterConsumed?.Invoke(stockMasters);
        }

        public override void ConsumeIndexMaster(List<IndexMaster> indexMasters)
        {
            IndexMasterConsumed?.Invoke(indexMasters);
        }

        public override void ConsumeCodeMap(Dictionary<string, object> codeMap)
        {
            CodeMapConsumed?.Invoke(codeMap);
        }

        public override void ConsumeChart(List<Candle> candles)
        {
            ChartConsumed?.Invoke(candles);
        }

        public void RegisterContract(SubscribeContract contract)
        {
            try
            {
                switch (contract.Type)
                {
                    case SubscribeTypes.StockConclusion:
                        TaskUtility.Run($"RealTimeConsumer.StockConclusionQueue", QueueTaskCancelToken, ProcessStockConclusionQueue);
                        break;
                    case SubscribeTypes.IndexConclusion:
                        TaskUtility.Run($"RealTimeConsumer.IndexConclusionQueue", QueueTaskCancelToken, ProcessIndexConclusionQueue);
                        break;
                    case SubscribeTypes.ETFConclusion:
                        TaskUtility.Run($"RealTimeConsumer.ETFConclusionQueue", QueueTaskCancelToken, ProcessETFConclusionQueue);
                        break;
                    case SubscribeTypes.BiddingPrice:
                        TaskUtility.Run($"RealTimeConsumer.BiddingPriceQueue", QueueTaskCancelToken, ProcessBiddingPriceQueue);
                        break;
                    case SubscribeTypes.CircuitBreak:
                        TaskUtility.Run("RealTimeConsumer.CircuitBreakQueue", QueueTaskCancelToken, ProcessCircuitBreakQueue);
                        break;
                }

                ServiceClient.RegisterConsumerContract(ClientId, contract);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        protected virtual void ProcessStockConclusionQueue()
        {
            try
            {
                if (StockConclusionQueue.TryDequeue(out var conclusion) == true)
                    StockConclusionConsumed?.Invoke(conclusion);
                else
                    Thread.Sleep(10);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        protected virtual void ProcessIndexConclusionQueue()
        {
            try
            {
                if (IndexConclusionQueue.TryDequeue(out var conclusion) == true)
                    IndexConclusionConsumed?.Invoke(conclusion);
                else
                    Thread.Sleep(10);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        protected virtual void ProcessETFConclusionQueue()
        {
            try
            {
                if (ETFConclusionQueue.TryDequeue(out var conclusion) == true)
                    ETFConclusionConsumed?.Invoke(conclusion);
                else
                    Thread.Sleep(10);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        protected virtual void ProcessBiddingPriceQueue()
        {
            try
            {
                if (BiddingPriceQueue.TryDequeue(out var biddingPrice) == true)
                    BiddingPriceConsumed?.Invoke(biddingPrice);
                else
                    Thread.Sleep(10);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        protected virtual void ProcessCircuitBreakQueue()
        {
            try
            {
                if (CircuitBreakQueue.TryDequeue(out var circuitBreak) == true)
                    CircuitBreakConsumed?.Invoke(circuitBreak);
                else
                    Thread.Sleep(10);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }
    }
}
