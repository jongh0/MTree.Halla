using MTree.Consumer;
using MTree.RealTimeProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using MTree.DataStructure;
using MTree.Utility;
using System.Threading;
using System.Diagnostics;

namespace TestConsumer
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class TestConsumer : RealTimeConsumer
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        int count = 0;
        double tick = 0;

        public TestConsumer()
        {
            base.OpenChannel();
        }

        private void ProcessStockConclusionQueue()
        {
            StockConclusion conclusion;
            if (StockConclusionQueue.TryDequeue(out conclusion) == true)
            {
                logger.Info($"StockConclusion <<< {conclusion}");
                //var interval = (DateTime.Now - conclusion.Time);
                //tick += interval.TotalMilliseconds;

                //Console.WriteLine($"{interval.ToString()}, {conclusion.Id}, {conclusion.Code}, {conclusion.Price}, {conclusion.ConclusionType}");
            }
            else
                Thread.Sleep(10);
        }

        private void ProcessIndexConclusionQueue()
        {
            try
            {
                IndexConclusion conclusion;
                if (IndexConclusionQueue.TryDequeue(out conclusion) == true)
                {
                    logger.Info($"IndexConclusion <<< {conclusion}");
                }
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
                {
                    logger.Info($"BiddingPrice <<< {biddingPrice}");
                }
                else
                    Thread.Sleep(10);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public void StartConsume()
        {
            TaskUtility.Run("Consumer.StockConclusionQueue", QueueTaskCancelToken, ProcessStockConclusionQueue);
            TaskUtility.Run($"Consumer.IndexConclusionQueue", QueueTaskCancelToken, ProcessIndexConclusionQueue);
            TaskUtility.Run($"Consumer.BiddingPriceQueue", QueueTaskCancelToken, ProcessBiddingPriceQueue);
        }

        public void StopConsume()
        {
            StopQueueTask();
            CloseChannel();
        }

        protected override void ServiceClient_Opened(object sender, EventArgs e)
        {
            base.ServiceClient_Opened(sender, e);

            try
            {
                ServiceClient.RegisterContract(ClientId, new SubscribeContract(SubscribeTypes.StockConclusion));
                ServiceClient.RegisterContract(ClientId, new SubscribeContract(SubscribeTypes.IndexConclusion));
                ServiceClient.RegisterContract(ClientId, new SubscribeContract(SubscribeTypes.BiddingPrice));
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
    }
}
