using MTree.Configuration;
using MTree.Consumer;
using MTree.DataStructure;
using MTree.RealTimeProvider;
using MTree.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTree.StrategyManager
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class RealTimeHandler : ConsumerBase, INotifyPropertyChanged, INotifySubscribableReceived, INotifyMessageReceived
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private List<string> ConcernCodeList { get; set; }

        public RealTimeHandler(List<string> codes)
        {
            try
            {
                ConcernCodeList = codes;

                TaskUtility.Run("RealTimeHandler.CircuitBreakQueue", QueueTaskCancelToken, ProcessCircuitBreakQueue);
                TaskUtility.Run($"RealTimeHandler.StockConclusionQueue", QueueTaskCancelToken, ProcessStockConclusionQueue);
                TaskUtility.Run($"RealTimeHandler.IndexConclusionQueue", QueueTaskCancelToken, ProcessIndexConclusionQueue);
                if (Config.General.SkipBiddingPrice == false)
                    TaskUtility.Run($"RealTimeHandler.BiddingPriceQueue", QueueTaskCancelToken, ProcessBiddingPriceQueue);
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
                if (IndexConclusionQueue.TryDequeue(out conclusion) == true)
                    NotifyConclusionReceived(conclusion);
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
                if (StockConclusionQueue.TryDequeue(out conclusion) == true)
                    NotifyConclusionReceived(conclusion);
                else
                    Thread.Sleep(10);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private void ProcessBiddingPriceQueue()
        {
            try
            {
                BiddingPrice biddingPrice;
                if (BiddingPriceQueue.TryDequeue(out biddingPrice) == true)
                    NotifyBiddingPriceReceived(biddingPrice);
                else
                    Thread.Sleep(10);
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
                if (CircuitBreakQueue.TryDequeue(out circuitBreak) == true)
                    NotifyCircuitBreakReceived(circuitBreak);
                else
                    Thread.Sleep(10);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        protected override void ServiceClient_Opened(object sender, EventArgs e)
        {
            base.ServiceClient_Opened(sender, e);

            try
            {
                ServiceClient.RegisterContract(ClientId, new SubscribeContract(SubscribeTypes.CircuitBreak, SubscribeScopes.Partial, ConcernCodeList));
                ServiceClient.RegisterContract(ClientId, new SubscribeContract(SubscribeTypes.StockConclusion, SubscribeScopes.Partial, ConcernCodeList));
                //ServiceClient.RegisterContract(ClientId, new SubscribeContract(SubscribeTypes.IndexConclusion));
                //if (Config.General.SkipBiddingPrice == false)
                //    ServiceClient.RegisterContract(ClientId, new SubscribeContract(SubscribeTypes.BiddingPrice));
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public override void NotifyMessage(MessageTypes type, string message)
        {
            NotifyMessageReceived(type, message);
            base.NotifyMessage(type, message);
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        #region INotifySubscribableReceived
        public event SubscribableReceivedEventHandler BiddingPriceReceived;
        public event SubscribableReceivedEventHandler CircuitBreakReceived;
        public event SubscribableReceivedEventHandler ConclusionReceived;

        private void NotifyBiddingPriceReceived(BiddingPrice biddingPrice)
        {
            BiddingPriceReceived?.Invoke(this, new SubscribableNotifiedEventArgs(biddingPrice));
        }

        private void NotifyCircuitBreakReceived(CircuitBreak circuitBreak)
        {
            CircuitBreakReceived?.Invoke(this, new SubscribableNotifiedEventArgs(circuitBreak));
        }

        private void NotifyConclusionReceived(Conclusion conclusion)
        {
            ConclusionReceived?.Invoke(this, new SubscribableNotifiedEventArgs(conclusion));
        }
        #endregion

        #region INotifyMessageReceived
        public event MessageReceivedEventHandler MessageReceived;

        private void NotifyMessageReceived(MessageTypes type, string message = "")
        {
            MessageReceived?.Invoke(this, new MessageReceivedEventArgs(type, message));
        } 
        #endregion
    }
}
