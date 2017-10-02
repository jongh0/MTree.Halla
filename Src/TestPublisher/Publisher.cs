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
            Random rand = new Random();
            Task.Run(() =>
            {
                while (true)
                {
                    if (QueueTaskCancelToken.IsCancellationRequested)
                        break;

                    var subscribable = new StockConclusion();
                    subscribable.Id = ObjectId.GenerateNewId();
                    subscribable.Code = "000020";
                    subscribable.Price = rand.Next(100, 200);
                    subscribable.Time = DateTime.Now;
                    subscribable.ConclusionType = ConclusionTypes.Sell;

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
                    subscribable.Code = "000030";
                    subscribable.Price = rand.Next(100, 200);
                    subscribable.Time = DateTime.Now;
                    subscribable.MarketCapitalization = 1000;

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
                    subscribable.Code = "000040";
                    subscribable.Time = DateTime.Now;
                    subscribable.Bids = new List<BiddingPriceEntity>();
                    subscribable.Bids.Add(new BiddingPriceEntity(10, 10, 10));
                    subscribable.Bids.Add(new BiddingPriceEntity(10, 10, 10));
                    subscribable.Bids.Add(new BiddingPriceEntity(10, 10, 10));
                    subscribable.Offers = new List<BiddingPriceEntity>();
                    subscribable.Offers.Add(new BiddingPriceEntity(10, 10, 10));
                    subscribable.Offers.Add(new BiddingPriceEntity(10, 10, 10));
                    subscribable.Offers.Add(new BiddingPriceEntity(10, 10, 10));

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
