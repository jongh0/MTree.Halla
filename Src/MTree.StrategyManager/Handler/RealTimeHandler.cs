using MTree.Configuration;
using MTree.Consumer;
using MTree.RealTimeProvider;
using MTree.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MTree.StrategyManager
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class RealTimeHandler : ConsumerBase, INotifyPropertyChanged
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
        }

        private void ProcessStockConclusionQueue()
        {
            // TODO : 데이터 흐름 정의 필요함
            // Chart로 들어갔다 ML로 가야하나?
        }

        private void ProcessBiddingPriceQueue()
        {
        }

        private void ProcessCircuitBreakQueue()
        {
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
    }
}
