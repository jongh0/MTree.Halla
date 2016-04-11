using MongoDB.Bson;
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
    class Publisher : PublisherBase
    {
        public void StartPublising()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    if (QueueTaskCancelToken.IsCancellationRequested)
                        break;

                    var conclusion = new StockConclusion();
                    conclusion.Id = ObjectId.GenerateNewId();
                    conclusion.Code = "000020";
                    conclusion.Price = 100;
                    conclusion.Time = DateTime.Now;
                    
                    ServiceClient.PublishStockConclusion(conclusion);
                    Console.WriteLine($"{conclusion.Time.ToLongTimeString()} published");

                    Thread.Sleep(2);
                }
            }, QueueTaskCancelToken);
        }

        public void StopPublishing()
        {
            StopQueueTask();
            CloseChannel();
        }
    }
}
