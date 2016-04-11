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
    public class Consumer : ConsumerBase
    {
        int count = 0;
        double tick = 0;

        private void ProcessStockConclusionQueue()
        {
            StockConclusion conclusion;
            if (StockConclusionQueue.TryDequeue(out conclusion) == true)
            {
                var interval = (DateTime.Now - conclusion.Time);
                tick += interval.TotalMilliseconds;

                Console.WriteLine($"{interval.ToString()}, {conclusion.Price}, {conclusion.Code} consumed");

                if (count++ > 10000)
                    Debugger.Break();
            }
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
