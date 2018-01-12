using MongoDB.Bson;
using DataStructure;
using Publisher;
using RealTimeProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.ServiceModel;
using Configuration;

namespace TestPublisher
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    class Publisher : PublisherBase
    {
        private const int delay = 2;

        public void StartPublising()
        {
            Random rand = new Random();

            if (Config.General.SkipBiddingPrice == false)
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
                        subscribable.Price = rand.Next(100, 200);
                        subscribable.Time = DateTime.Now;
                        subscribable.ConclusionType = ConclusionTypes.Sell;

                        if (ServiceClient?.State == CommunicationState.Opened)
                            ServiceClient.PublishStockConclusion(subscribable);

                        Thread.Sleep(delay);
                    }
                }, QueueTaskCancelToken);
            }

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

                    if (ServiceClient?.State == CommunicationState.Opened)
                        ServiceClient.PublishIndexConclusion(subscribable);

                    Thread.Sleep(delay);
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

                    if (ServiceClient?.State == CommunicationState.Opened)
                        ServiceClient.PublishBiddingPrice(subscribable);

                    Thread.Sleep(delay);
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
