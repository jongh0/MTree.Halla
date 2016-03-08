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

namespace TestConsumer
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class Consumer : ConsumerImplement, IRealTimeConsumerCallback
    {
        public Consumer()
        {
            GeneralTask.Run("Consumer.StockConclusionQueue", QueueTaskCancelToken, ProcessStockConclusionQueue);
        }

        private void ProcessStockConclusionQueue()
        {
            StockConclusion conclusion;
            if (StockConclusionQueue.TryDequeue(out conclusion) == true)
                Console.WriteLine($"{conclusion.Time.ToLongTimeString()} consumed");
            else
                Thread.Sleep(10);
        }

        public void ConsumeBiddingPrice(BiddingPrice biddingPrice)
        {
        }

        public void ConsumeCircuitBreak(CircuitBreak circuitBreak)
        {
        }

        public void ConsumeIndexConclusion(IndexConclusion conclusion)
        {
        }

        public void ConsumeStockConclusion(StockConclusion conclusion)
        {
            StockConclusionQueue.Enqueue(conclusion);
        }

        public void NoOperation()
        {
        }

        public void StartConsume()
        {
            var subscription = new Subscription();
            subscription.Type = SubscriptionType.StockConclusion;
            subscription.Way = SubscriptionWay.Partial;
            subscription.Codes.Add("000020");
            ServiceClient.RequestSubscription(ClientId, subscription);
        }

        public void StopConsume()
        {
            ServiceClient.RequestUnsubscriptionAll(ClientId);
        }
    }
}
