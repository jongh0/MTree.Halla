using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using MTree.DataStructure;
using MTree.Utility;
using System.Threading;

namespace MTree.RealTimeProvider
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public partial class RealTimeProvider : RealTimeBase, IRealTimePublisher, IRealTimeConsumer
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public RealTimeProvider()
        {
            GeneralTask.Run("RealTimeProvider.BiddingPriceQueue", QueueTaskCancelToken, ProcessBiddingPriceQueue);
            GeneralTask.Run("RealTimeProvider.StockConclusionQueue", QueueTaskCancelToken, ProcessStockConclusionQueue);
            GeneralTask.Run("RealTimeProvider.IndexConclusionQueue", QueueTaskCancelToken, ProcessIndexConclusionQueue);
        }

        public void NoOperation()
        {
        }

        private void ProcessBiddingPriceQueue()
        {
            try
            {
                BiddingPrice biddingPrice;
                if (BiddingPriceQueue.TryDequeue(out biddingPrice) == true)
                {
                    foreach (var subscription in BiddingPriceSubscriptions)
                    {
                        if (subscription.Value.Way == SubscriptionWay.All ||
                            subscription.Value.ContainCode(biddingPrice.Code) == true)
                        {
                            subscription.Value.Callback.ConsumeBiddingPrice(biddingPrice);
                        }
                    }
                }
                else
                {
                    Thread.Sleep(10);
                }
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
                {
                    foreach (var subscription in StockConclusionSubscriptions)
                    {
                        if (subscription.Value.Way == SubscriptionWay.All ||
                            subscription.Value.ContainCode(conclusion.Code) == true)
                        {
                            subscription.Value.Callback.ConsumeStockConclusion(conclusion);
                        }
                    }
                }
                else
                {
                    Thread.Sleep(10);
                }
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
                {
                    foreach (var subscription in IndexConclusionSubscriptions)
                    {
                        if (subscription.Value.Way == SubscriptionWay.All ||
                            subscription.Value.ContainCode(conclusion.Code) == true)
                        {
                            subscription.Value.Callback.ConsumeIndexConclusion(conclusion);
                        }
                    }
                }
                else
                {
                    Thread.Sleep(10);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
    }
}
