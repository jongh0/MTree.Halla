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

                    var subscribable = new StockConclusion();
                    subscribable.Id = ObjectId.GenerateNewId();
                    subscribable.Code = "000020";
                    subscribable.Price = 100;
                    subscribable.Time = DateTime.Now;
                    
                    ServiceClient.PublishStockConclusion(subscribable);

                    Thread.Sleep(2);
                }
            }, QueueTaskCancelToken);

            Task.Run(() =>
            {
                while (true)
                {
                    if (QueueTaskCancelToken.IsCancellationRequested)
                        break;

                    var subscribable = new IndexConclusion();
                    subscribable.Id = ObjectId.GenerateNewId();
                    subscribable.Code = "000020";
                    subscribable.Price = 100;
                    subscribable.Time = DateTime.Now;

                    ServiceClient.PublishIndexConclusion(subscribable);

                    Thread.Sleep(2);
                }
            }, QueueTaskCancelToken);

            Task.Run(() =>
            {
                while (true)
                {
                    if (QueueTaskCancelToken.IsCancellationRequested)
                        break;

                    var subscribable = new BiddingPrice();
                    subscribable.Id = ObjectId.GenerateNewId();
                    subscribable.Code = "000020";
                    subscribable.Time = DateTime.Now;

                    ServiceClient.PublishBiddingPrice(subscribable);

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
