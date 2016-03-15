using System;
using System.ServiceModel;
using MongoDB.Driver;
using MTree.DbProvider;
using System.Threading;
using MTree.DataStructure;
using MTree.Configuration;
using MTree.Consumer;
using MTree.Utility;
using MTree.RealTimeProvider;

namespace MTree.HistorySaver
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class HistorySaver : ConsumerBase
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private IMongoCollection<BiddingPrice> BiddingPriceCollection { get; set; }
        private IMongoCollection<StockConclusion> StockConclusionCollection { get; set; }
        private IMongoCollection<IndexConclusion> IndexConclusionCollection { get; set; }
        private IMongoCollection<CircuitBreak> CircuitBreakCollection { get; set; }
        private IMongoCollection<StockMaster> StockMasterCollection { get; set; }

        public HistorySaver()
        {
            try
            {
                MongoDbProvider.Instance.Connect();
                BiddingPriceCollection = MongoDbProvider.Instance.GetDatabase(DbType.BiddingPrice).GetCollection<BiddingPrice>(Config.Database.TodayCollectionName);
                StockConclusionCollection = MongoDbProvider.Instance.GetDatabase(DbType.StockConclusion).GetCollection<StockConclusion>(Config.Database.TodayCollectionName);
                IndexConclusionCollection = MongoDbProvider.Instance.GetDatabase(DbType.IndexConclusion).GetCollection<IndexConclusion>(Config.Database.TodayCollectionName);
                CircuitBreakCollection = MongoDbProvider.Instance.GetDatabase(DbType.CircuitBreak).GetCollection<CircuitBreak>(nameof(CircuitBreak)); // TODO : circuit break collection name 정해야함

                CreateIndex();

                TaskUtility.Run("HistorySaver.BiddingPriceQueue", QueueTaskCancelToken, ProcessBiddingPriceQueue);
                TaskUtility.Run("HistorySaver.StockConclusionQueue", QueueTaskCancelToken, ProcessStockConclusionQueue);
                TaskUtility.Run("HistorySaver.IndexConclusionQueue", QueueTaskCancelToken, ProcessIndexConclusionQueue);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private void CreateIndex()
        {
            try
            {
                var biddingPriceKeys = Builders<BiddingPrice>.IndexKeys.Ascending(i => i.Code).Ascending(i => i.Time);
                BiddingPriceCollection.Indexes.CreateOneAsync(biddingPriceKeys);
                logger.Info("Bidding price collection index created");

                var stockConclusionKeys = Builders<StockConclusion>.IndexKeys.Ascending(i => i.Code).Ascending(i => i.Time);
                StockConclusionCollection.Indexes.CreateOneAsync(stockConclusionKeys);
                logger.Info("Stock conclusion collection index created");

                var indexConclusionKeys = Builders<IndexConclusion>.IndexKeys.Ascending(i => i.Code).Ascending(i => i.Time);
                IndexConclusionCollection.Indexes.CreateOneAsync(indexConclusionKeys);
                logger.Info("Index conclusion collection index created");

                var circuitBreakKeys = Builders<CircuitBreak>.IndexKeys.Ascending(i => i.Code).Ascending(i => i.Time);
                CircuitBreakCollection.Indexes.CreateOneAsync(circuitBreakKeys);
                logger.Info("Circuit break collection index created");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        protected override void ServiceClient_Opened(object sender, EventArgs e)
        {
            base.ServiceClient_Opened(sender, e);

            ServiceClient.RegisterContract(ClientId, new SubscribeContract(SubscribeType.StockMaster));
            ServiceClient.RegisterContract(ClientId, new SubscribeContract(SubscribeType.BiddingPrice, SubscribeScope.All));
            ServiceClient.RegisterContract(ClientId, new SubscribeContract(SubscribeType.StockConclusion, SubscribeScope.All));
            ServiceClient.RegisterContract(ClientId, new SubscribeContract(SubscribeType.IndexConclusion, SubscribeScope.All));
        }

        private void ProcessBiddingPriceQueue()
        {
            try
            {
                BiddingPrice biddingPrice;
                if (BiddingPriceQueue.TryDequeue(out biddingPrice) == true)
                    BiddingPriceCollection.InsertOne(biddingPrice);
                else
                    Thread.Sleep(10);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private void ProcessStockConclusionQueue()
        {
            try
            {
                StockConclusion conclusion;
                if (StockConclusionQueue.TryDequeue(out conclusion) == true)
                    StockConclusionCollection.InsertOne(conclusion);
                else
                    Thread.Sleep(10);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private void ProcessIndexConclusionQueue()
        {
            try
            {
                IndexConclusion conclusion;
                if (IndexConclusionQueue.TryDequeue(out conclusion) == true)
                    IndexConclusionCollection.InsertOne(conclusion);
                else
                    Thread.Sleep(10);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public override void ConsumeBiddingPrice(BiddingPrice biddingPrice)
        {
            base.ConsumeBiddingPrice(biddingPrice);
            BiddingPriceQueue.Enqueue(biddingPrice);
        }

        public override void ConsumeStockConclusion(StockConclusion conclusion)
        {
            base.ConsumeStockConclusion(conclusion);
            StockConclusionQueue.Enqueue(conclusion);
        }

        public override void ConsumeIndexConclusion(IndexConclusion conclusion)
        {
            base.ConsumeIndexConclusion(conclusion);
            IndexConclusionQueue.Enqueue(conclusion);
        }

        public override void ConsumeCircuitBreak(CircuitBreak circuitBreak)
        {
            base.ConsumeCircuitBreak(circuitBreak);
            CircuitBreakCollection.InsertOne(circuitBreak);
        }

        public override void ConsumeStockMaster(StockMaster stockMaster)
        {
            base.ConsumeStockMaster(stockMaster);

            try
            {
                if (stockMaster == null || string.IsNullOrEmpty(stockMaster.Code) == true)
                {
                    logger.Error("Stock master has a problem");
                    return;
                }

                if (StockMasterCollection == null)
                {
                    var db = MongoDbProvider.Instance.GetDatabase(DbType.StockMaster);
                    db.DropCollection(Config.Database.TodayCollectionName);
                    logger.Info($"Stock master collection droped, {Config.Database.TodayCollectionName}");

                    StockMasterCollection = db.GetCollection<StockMaster>(Config.Database.TodayCollectionName);

                    var keys = Builders<StockMaster>.IndexKeys.Ascending(i => i.Code).Ascending(i => i.Time);
                    StockMasterCollection.Indexes.CreateOneAsync(keys);
                    logger.Info($"Stock master collection index created, {Config.Database.TodayCollectionName}");
                }

                StockMasterCollection.InsertOne(stockMaster);
                logger.Trace($"Stock master saved, {stockMaster.Code}");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
    }
}
