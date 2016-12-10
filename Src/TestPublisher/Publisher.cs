using MongoDB.Bson;
using MTree.DataStructure;
using MTree.Publisher;
using MTree.RealTimeProvider;
using MTree.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestPublisher
{
    class Publisher : RealTimePublisher
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private const int taskInterval = 5;

        public void StartPublising()
        {
            Random rand = new Random();
            Task.Run(() =>
            {
                while (true)
                {
                    if (QueueTaskCancelToken.IsCancellationRequested)
                        break;

                    var c = new StockConclusion();
                    c.Id = ObjectId.GenerateNewId();
                    c.Code = "000020";
                    c.Price = rand.Next(100, 200);
                    c.Time = DateTime.Now;
                    c.ConclusionType = ConclusionTypes.Buy;
                    c.Amount = 400;
                    c.MarketTimeType = MarketTimeTypes.BeforeExpect;
                    c.ReceivedTime = DateTime.Now.AddMinutes(1);

                    try
                    {
                        logger.Info($"StockConclusion >>> {c}");
                        ServiceClient.PublishStockConclusion(c);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                        break;
                    }

                    Thread.Sleep(taskInterval);
                }
            }, QueueTaskCancelToken);

            Task.Run(() =>
            {
                while (true)
                {
                    if (QueueTaskCancelToken.IsCancellationRequested)
                        break;

                    var c = new IndexConclusion();
                    c.Id = ObjectId.GenerateNewId();
                    c.Code = "000030";
                    c.Price = rand.Next(100, 200);
                    c.Time = DateTime.Now;
                    c.ReceivedTime = DateTime.Now.AddMinutes(1);
                    c.MarketCapitalization = 10000;
                    c.Amount = 500;
                    c.MarketTimeType = MarketTimeTypes.AfterExpect;

                    try
                    {
                        logger.Info($"IndexConclusion >>> {c}");
                        ServiceClient.PublishIndexConclusion(c);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                        break;
                    }

                    Thread.Sleep(taskInterval);
                }
            }, QueueTaskCancelToken);

            Task.Run(() =>
            {
                while (true)
                {
                    if (QueueTaskCancelToken.IsCancellationRequested)
                        break;

                    var b = new BiddingPrice();
                    b.Id = ObjectId.GenerateNewId();
                    b.Code = "000040";
                    b.Time = DateTime.Now;
                    b.ReceivedTime = DateTime.Now.AddMinutes(1);
                    b.Bids = new List<BiddingPriceEntity>();
                    b.Bids.Add(new BiddingPriceEntity(1, 2, 3));
                    b.Bids.Add(new BiddingPriceEntity(4, 5, 6));
                    b.Bids.Add(new BiddingPriceEntity(7, 8, 9));
                    b.Offers = new List<BiddingPriceEntity>();
                    b.Offers.Add(new BiddingPriceEntity(10, 20, 30));
                    b.Offers.Add(new BiddingPriceEntity(40, 50, 60));
                    b.Offers.Add(new BiddingPriceEntity(70, 80, 90));

                    try
                    {
                        logger.Info($"BiddingPrice >>> {b}");
                        ServiceClient.PublishBiddingPrice(b);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                        break;
                    }

                    Thread.Sleep(taskInterval);
                }
            }, QueueTaskCancelToken);
        }

        public void StopPublishing()
        {
            StopQueueTask();
            CloseChannel();
        }

        protected override void ServiceClient_Opened(object sender, EventArgs e)
        {
            base.ServiceClient_Opened(sender, e);

            Task.Run(() =>
            {
                // Contract 등록
                RegisterPublishContract(ProcessTypes.TestPublisher);
            });
        }
    }
}
