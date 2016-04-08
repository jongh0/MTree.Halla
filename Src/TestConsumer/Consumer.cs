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
                Console.WriteLine($"{(DateTime.Now - conclusion.Time).ToString()}, {conclusion.Price}, {conclusion.Code} consumed");
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
            ServiceClient.RegisterContract(ClientId, new SubscribeContract(SubscribeTypes.StockConclusion));
        }

        public void StopConsume()
        {
            StopQueueTask();
            CloseChannel();
        }
    }
}
