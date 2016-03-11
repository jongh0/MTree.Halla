using System;
using System.ServiceModel;
using MongoDB.Driver;
using MTree.DbProvider;
using System.Threading;
using MTree.DataStructure;
using MTree.Configuration;
using MTree.Consumer;
using MTree.Utility;

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

                GeneralTask.Run("HistorySaver.BiddingPriceQueue", QueueTaskCancelToken, ProcessBiddingPriceQueue);
                GeneralTask.Run("HistorySaver.StockConclusionQueue", QueueTaskCancelToken, ProcessStockConclusionQueue);
                GeneralTask.Run("HistorySaver.IndexConclusionQueue", QueueTaskCancelToken, ProcessIndexConclusionQueue);
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

                var stockConclusionKeys = Builders<StockConclusion>.IndexKeys.Ascending(i => i.Code).Ascending(i => i.Time);
                StockConclusionCollection.Indexes.CreateOneAsync(stockConclusionKeys);

                var indexConclusionKeys = Builders<IndexConclusion>.IndexKeys.Ascending(i => i.Code).Ascending(i => i.Time);
                IndexConclusionCollection.Indexes.CreateOneAsync(indexConclusionKeys);

                var circuitBreakKeys = Builders<CircuitBreak>.IndexKeys.Ascending(i => i.Code).Ascending(i => i.Time);
                CircuitBreakCollection.Indexes.CreateOneAsync(circuitBreakKeys);

                logger.Info("Index created");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
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
            BiddingPriceQueue.Enqueue(biddingPrice);
        }

        public override void ConsumeStockConclusion(StockConclusion conclusion)
        {
            StockConclusionQueue.Enqueue(conclusion);
        }

        public override void ConsumeIndexConclusion(IndexConclusion conclusion)
        {
            IndexConclusionQueue.Enqueue(conclusion);
        }

        public override void ConsumeCircuitBreak(CircuitBreak circuitBreak)
        {
            CircuitBreakCollection.InsertOneAsync(circuitBreak);
        }
    }
}
