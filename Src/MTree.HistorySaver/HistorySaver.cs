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
using System.Collections.Concurrent;

namespace MTree.HistorySaver
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class HistorySaver : ConsumerBase
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        #region Lock
        private object ChartLock { get; } = new object();
        private object BiddingPriceLock { get; } = new object();
        private object StockMasterLock { get; } = new object();
        private object StockConclusionLock { get; } = new object();
        private object IndexConclusionLock { get; } = new object();
        #endregion

        #region Collections
        private ConcurrentDictionary<string, IMongoCollection<Candle>> ChartCollections { get; set; } = new ConcurrentDictionary<string, IMongoCollection<Candle>>();
        private ConcurrentDictionary<string, IMongoCollection<BiddingPrice>> BiddingPriceCollections { get; set; } = new ConcurrentDictionary<string, IMongoCollection<BiddingPrice>>();
        private ConcurrentDictionary<string, IMongoCollection<StockMaster>> StockMasterCollections { get; set; } = new ConcurrentDictionary<string, IMongoCollection<StockMaster>>();
        private ConcurrentDictionary<string, IMongoCollection<StockConclusion>> StockConclusionCollections { get; set; } = new ConcurrentDictionary<string, IMongoCollection<StockConclusion>>();
        private ConcurrentDictionary<string, IMongoCollection<IndexConclusion>> IndexConclusionCollections { get; set; } = new ConcurrentDictionary<string, IMongoCollection<IndexConclusion>>();
        private ConcurrentDictionary<string, IMongoCollection<CircuitBreak>> CircuitBreakCollections { get; set; } = new ConcurrentDictionary<string, IMongoCollection<CircuitBreak>>();
        #endregion

        public HistorySaver()
        {
            try
            {
                MongoDbProvider.Instance.Connect();

                TaskUtility.Run("HistorySaver.BiddingPriceQueue", QueueTaskCancelToken, ProcessBiddingPriceQueue);
                TaskUtility.Run("HistorySaver.StockConclusionQueue", QueueTaskCancelToken, ProcessStockConclusionQueue);
                TaskUtility.Run("HistorySaver.IndexConclusionQueue", QueueTaskCancelToken, ProcessIndexConclusionQueue);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        protected override void ServiceClient_Opened(object sender, EventArgs e)
        {
            base.ServiceClient_Opened(sender, e);

            ServiceClient.RegisterContract(ClientId, new SubscribeContract(SubscribeTypes.StockMaster));
            ServiceClient.RegisterContract(ClientId, new SubscribeContract(SubscribeTypes.BiddingPrice, SubscribeScopes.All));
            ServiceClient.RegisterContract(ClientId, new SubscribeContract(SubscribeTypes.StockConclusion, SubscribeScopes.All));
            ServiceClient.RegisterContract(ClientId, new SubscribeContract(SubscribeTypes.IndexConclusion, SubscribeScopes.All));
        }

        private void ProcessBiddingPriceQueue()
        {
            try
            {
                BiddingPrice item;
                if (BiddingPriceQueue.TryDequeue(out item) == true)
                {
                    IMongoCollection<BiddingPrice> collection = null;

                    if (BiddingPriceCollections.ContainsKey(item.Code) == false)
                    {
                        lock (BiddingPriceLock)
                        {
                            if (collection == null)
                            {
                                collection = MongoDbProvider.Instance.GetDatabase(DbTypes.BiddingPrice).GetCollection<BiddingPrice>(item.Code);
                                var keys = Builders<BiddingPrice>.IndexKeys.Ascending(i => i.Time);
                                collection.Indexes.CreateOneAsync(keys);

                                BiddingPriceCollections.TryAdd(item.Code, collection);
                            }
                        }
                    }
                    else
                    {
                        collection = BiddingPriceCollections[item.Code];
                    }

                    collection?.InsertOne(item);
                }
                else
                {
                    Thread.Sleep(10);
                }
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
                StockConclusion item;
                if (StockConclusionQueue.TryDequeue(out item) == true)
                {
                    IMongoCollection<StockConclusion> collection = null;

                    if (StockConclusionCollections.ContainsKey(item.Code) == false)
                    {
                        lock (StockConclusionLock)
                        {
                            if (collection == null)
                            {
                                collection = MongoDbProvider.Instance.GetDatabase(DbTypes.StockConclusion).GetCollection<StockConclusion>(item.Code);
                                var keys = Builders<StockConclusion>.IndexKeys.Ascending(i => i.Time);
                                collection.Indexes.CreateOneAsync(keys);

                                StockConclusionCollections.TryAdd(item.Code, collection);
                            }
                        }
                    }
                    else
                    {
                        collection = StockConclusionCollections[item.Code];
                    }

                    collection?.InsertOne(item);
                }
                else
                {
                    Thread.Sleep(10);
                }
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
                IndexConclusion item;
                if (IndexConclusionQueue.TryDequeue(out item) == true)
                {
                    IMongoCollection<IndexConclusion> collection = null;

                    if (IndexConclusionCollections.ContainsKey(item.Code) == false)
                    {
                        lock (IndexConclusionLock)
                        {
                            if (collection == null)
                            {
                                collection = MongoDbProvider.Instance.GetDatabase(DbTypes.IndexConclusion).GetCollection<IndexConclusion>(item.Code);
                                var keys = Builders<IndexConclusion>.IndexKeys.Ascending(i => i.Time);
                                collection.Indexes.CreateOneAsync(keys);

                                IndexConclusionCollections.TryAdd(item.Code, collection);
                            }
                        }
                    }
                    else
                    {
                        collection = IndexConclusionCollections[item.Code];
                    }

                    collection?.InsertOne(item);
                }
                else
                {
                    Thread.Sleep(10);
                }
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
            MongoDbProvider.Instance.GetDatabase(DbTypes.CircuitBreak).GetCollection<CircuitBreak>(nameof(CircuitBreak)).InsertOne(circuitBreak);
        }

        public override void ConsumeStockMaster(StockMaster stockMaster)
        {
            base.ConsumeStockMaster(stockMaster);

            int startTick = Environment.TickCount;

            try
            {
                IMongoCollection<StockMaster> collection = null;

                if (StockMasterCollections.ContainsKey(stockMaster.Code) == false)
                {
                    lock (StockMasterLock)
                    {
                        if (collection == null)
                        {
                            collection = MongoDbProvider.Instance.GetDatabase(DbTypes.StockMaster).GetCollection<StockMaster>(stockMaster.Code);
                            var keys = Builders<StockMaster>.IndexKeys.Ascending(i => i.Time);
                            collection.Indexes.CreateOneAsync(keys);
                        }
                    }
                }
                else
                {
                    collection = StockMasterCollections[stockMaster.Code];
                }

                var filter = Builders<StockMaster>.Filter.Eq(i => i.Time, stockMaster.Time);
                collection?.DeleteMany(filter);
                collection?.InsertOne(stockMaster);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                logger.Info($"{stockMaster.Code} stock master saved, Tick: {Environment.TickCount - startTick}");
            }
        }
    }
}
