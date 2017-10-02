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
        //int count = 0;
        double tick = 0;

        public void StartConsume()
        {
            TaskUtility.Run("Consumer.StockConclusionQueue", QueueTaskCancelToken, ProcessStockConclusionQueue);
            TaskUtility.Run("Consumer.IndexConclusionQueue", QueueTaskCancelToken, ProcessIndexConclusionQueue);
            TaskUtility.Run("Consumer.BiddingPriceQueue", QueueTaskCancelToken, ProcessBiddingPriceQueue);
            ServiceClient.RegisterContract(ClientId, new SubscribeContract(SubscribeTypes.StockConclusion));
            ServiceClient.RegisterContract(ClientId, new SubscribeContract(SubscribeTypes.IndexConclusion));
            ServiceClient.RegisterContract(ClientId, new SubscribeContract(SubscribeTypes.BiddingPrice));
        }

        public void StopConsume()
        {
            StopQueueTask();
            CloseChannel();
        }

        private void ProcessStockConclusionQueue()
        {
            if (StockConclusionQueue.TryDequeue(out var conclusion) == true)
            {
                var interval = (DateTime.Now - conclusion.Time);
                tick += interval.TotalMilliseconds;

                //Console.WriteLine($"{interval.ToString()}, {conclusion.Price}, {conclusion.Code} consumed");
            }
            else
                Thread.Sleep(10);
        }

        private void ProcessIndexConclusionQueue()
        {
            if (IndexConclusionQueue.TryDequeue(out var conclusion) == true)
            {
                var interval = (DateTime.Now - conclusion.Time);
                tick += interval.TotalMilliseconds;

                //Console.WriteLine($"{interval.ToString()}, {conclusion.Price}, {conclusion.Code} consumed");
            }
            else
                Thread.Sleep(10);
        }

        private void ProcessBiddingPriceQueue()
        {
            if (BiddingPriceQueue.TryDequeue(out var biddingPrice) == true)
            {
                var interval = (DateTime.Now - biddingPrice.Time);
                tick += interval.TotalMilliseconds;

                //Console.WriteLine($"{interval.ToString()}, {biddingPrice.Code} consumed");
            }
            else
                Thread.Sleep(10);
        }
    }
}
