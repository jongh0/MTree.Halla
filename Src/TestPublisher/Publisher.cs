using MTree.DataStructure;
using MTree.Publisher;
using MTree.RealTimeProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestPublisher
{
    class Publisher : PublisherImplement, IRealTimePublisherCallback
    {
        protected override void Initialize()
        {
            base.Initialize();
            ProcessName = nameof(Publisher);
        }

        public void StartPublising()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    if (QueueTaskCancelToken.IsCancellationRequested)
                        break;

                    var conclusion = new StockConclusion();
                    conclusion.Code = "000020";
                    conclusion.Time = DateTime.Now;
                    
                    ServiceClient.PublishStockConclusion(conclusion);
                    Console.WriteLine($"{conclusion.Time.ToLongTimeString()} published");

                    Thread.Sleep(10);
                }
            }, QueueTaskCancelToken);
        }

        public void StopPublishing()
        {
            QueueTaskCancelSource.Cancel();
        }
    }
}
