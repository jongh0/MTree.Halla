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
    public class Consumer : ConsumerBase
    {
        private void ProcessStockConclusionQueue()
        {
            StockConclusion conclusion;
            if (StockConclusionQueue.TryDequeue(out conclusion) == true)
                Console.WriteLine($"{conclusion.Time.ToLongTimeString()} consumed");
            else
                Thread.Sleep(10);
        }

        public override void ConsumeStockConclusion(StockConclusion conclusion)
        {
            StockConclusionQueue.Enqueue(conclusion);
        }

        public void StartConsume()
        {
            TaskUtility.Run("Consumer.StockConclusionQueue", QueueTaskCancelToken, ProcessStockConclusionQueue);

            var subscription = new SubscribeContract();
            subscription.Type = SubscribeTypes.StockConclusion;
            subscription.Scope = SubscribeScopes.Partial;
            subscription.Codes.Add("000020");
            ServiceClient.RegisterContract(ClientId, subscription);
        }

        public void StopConsume()
        {
            StopQueueTask();
            CloseChannel();
        }
    }
}
