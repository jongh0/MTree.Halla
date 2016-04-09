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
    public class RealTimeHandler : ConsumerBase, INotifyPropertyChanged, INotifySubscribable
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public RealTimeHandler()
        {
            try
            {
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
                    NotifyConclusion(conclusion);
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
                    NotifyConclusion(conclusion);
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
                    NotifyBiddingPrice(biddingPrice);
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
                    NotifyCircuitBreak(circuitBreak);
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
                ServiceClient.RegisterContract(ClientId, new SubscribeContract(SubscribeTypes.Mastering));
                ServiceClient.RegisterContract(ClientId, new SubscribeContract(SubscribeTypes.CircuitBreak));
                ServiceClient.RegisterContract(ClientId, new SubscribeContract(SubscribeTypes.StockConclusion));
                ServiceClient.RegisterContract(ClientId, new SubscribeContract(SubscribeTypes.IndexConclusion));
                if (Config.General.SkipBiddingPrice == false)
                    ServiceClient.RegisterContract(ClientId, new SubscribeContract(SubscribeTypes.BiddingPrice));
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        #region INotifySubscribable
        public event SubscribableEventHandler BiddingPriceNotified;
        public event SubscribableEventHandler CircuitBreakNotified;
        public event SubscribableEventHandler ConclusionNotified; 

        private void NotifyBiddingPrice(BiddingPrice biddingPrice)
        {
            BiddingPriceNotified?.Invoke(this, new SubscribableEventArgs(biddingPrice));
        }

        private void NotifyCircuitBreak(CircuitBreak circuitBreak)
        {
            CircuitBreakNotified?.Invoke(this, new SubscribableEventArgs(circuitBreak));
        }

        private void NotifyConclusion(Conclusion conclusion)
        {
            ConclusionNotified?.Invoke(this, new SubscribableEventArgs(conclusion));
        }
        #endregion
    }
}
