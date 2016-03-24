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
using System.Text;

namespace MTree.HistorySaver
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class HistorySaver : ConsumerBase
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private int circuitBreakCount = 0;
        private int biddingPriceCount = 0;
        private int stockMasterCount = 0;
        private int stockConclusionCount = 0;
        private int indexConclusionCount = 0;

        #region Lock
        private object ChartLock { get; } = new object();
        private object BiddingPriceLock { get; } = new object();
        private object CircuitBreakLock { get; } = new object();
        private object StockMasterLock { get; } = new object();
        private object StockConclusionLock { get; } = new object();
        private object IndexConclusionLock { get; } = new object();
        #endregion

        #region Db & Collection
        private IMongoDatabase ChartDb { get; set; }
        private IMongoDatabase BiddingPriceDb { get; set; }
        private IMongoDatabase CircuitBreakDb { get; set; }
        private IMongoDatabase StockMasterDb { get; set; }
        private IMongoDatabase StockConclusionDb { get; set; }
        private IMongoDatabase IndexConclusionDb { get; set; }

        private ConcurrentDictionary<string, IMongoCollection<Candle>> ChartCollections { get; set; } = new ConcurrentDictionary<string, IMongoCollection<Candle>>();
        private ConcurrentDictionary<string, IMongoCollection<BiddingPrice>> BiddingPriceCollections { get; set; } = new ConcurrentDictionary<string, IMongoCollection<BiddingPrice>>();
        private ConcurrentDictionary<string, IMongoCollection<CircuitBreak>> CircuitBreakCollections { get; set; } = new ConcurrentDictionary<string, IMongoCollection<CircuitBreak>>();
        private ConcurrentDictionary<string, IMongoCollection<StockMaster>> StockMasterCollections { get; set; } = new ConcurrentDictionary<string, IMongoCollection<StockMaster>>();
        private ConcurrentDictionary<string, IMongoCollection<StockConclusion>> StockConclusionCollections { get; set; } = new ConcurrentDictionary<string, IMongoCollection<StockConclusion>>();
        private ConcurrentDictionary<string, IMongoCollection<IndexConclusion>> IndexConclusionCollections { get; set; } = new ConcurrentDictionary<string, IMongoCollection<IndexConclusion>>();
        #endregion

        public HistorySaver()
        {
            try
            {
                MongoDbProvider.Instance.Connect();
                ChartDb = MongoDbProvider.Instance.GetDatabase(DbTypes.Chart);
                BiddingPriceDb = MongoDbProvider.Instance.GetDatabase(DbTypes.BiddingPrice);
                CircuitBreakDb = MongoDbProvider.Instance.GetDatabase(DbTypes.CircuitBreak);
                StockMasterDb = MongoDbProvider.Instance.GetDatabase(DbTypes.StockMaster);
                StockConclusionDb = MongoDbProvider.Instance.GetDatabase(DbTypes.StockConclusion);
                IndexConclusionDb = MongoDbProvider.Instance.GetDatabase(DbTypes.IndexConclusion);

                TaskUtility.Run("HistorySaver.BiddingPriceQueue", QueueTaskCancelToken, ProcessBiddingPriceQueue);
                TaskUtility.Run("HistorySaver.CircuitBreakQueue", QueueTaskCancelToken, ProcessCircuitBreakQueue);
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
            ServiceClient.RegisterContract(ClientId, new SubscribeContract(SubscribeTypes.CircuitBreak));
            ServiceClient.RegisterContract(ClientId, new SubscribeContract(SubscribeTypes.BiddingPrice));
            ServiceClient.RegisterContract(ClientId, new SubscribeContract(SubscribeTypes.StockConclusion));
            ServiceClient.RegisterContract(ClientId, new SubscribeContract(SubscribeTypes.IndexConclusion));
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
                                collection = BiddingPriceDb.GetCollection<BiddingPrice>(item.Code);
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

                    collection.InsertOne(item);
                    biddingPriceCount++;
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

        private void ProcessCircuitBreakQueue()
        {
            try
            {
                CircuitBreak item;
                if (CircuitBreakQueue.TryDequeue(out item) == true)
                {
                    IMongoCollection<CircuitBreak> collection = null;

                    if (CircuitBreakCollections.ContainsKey(item.Code) == false)
                    {
                        lock (CircuitBreakLock)
                        {
                            if (collection == null)
                            {
                                collection = CircuitBreakDb.GetCollection<CircuitBreak>(item.Code);
                                var keys = Builders<CircuitBreak>.IndexKeys.Ascending(i => i.Time);
                                collection.Indexes.CreateOneAsync(keys);

                                CircuitBreakCollections.TryAdd(item.Code, collection);
                            }
                        }
                    }
                    else
                    {
                        collection = CircuitBreakCollections[item.Code];
                    }

                    collection.InsertOne(item);
                    circuitBreakCount++;
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
                                collection = StockConclusionDb.GetCollection<StockConclusion>(item.Code);
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

                    collection.InsertOne(item);
                    stockConclusionCount++;
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
                                collection = IndexConclusionDb.GetCollection<IndexConclusion>(item.Code);
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

                    collection.InsertOne(item);
                    indexConclusionCount++;
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
            BiddingPriceQueue.Enqueue(biddingPrice);
            base.ConsumeBiddingPrice(biddingPrice);
        }

        public override void ConsumeStockConclusion(StockConclusion conclusion)
        {
            StockConclusionQueue.Enqueue(conclusion);
            base.ConsumeStockConclusion(conclusion);
        }

        public override void ConsumeIndexConclusion(IndexConclusion conclusion)
        {
            IndexConclusionQueue.Enqueue(conclusion);
            base.ConsumeIndexConclusion(conclusion);
        }

        public override void ConsumeCircuitBreak(CircuitBreak circuitBreak)
        {
            CircuitBreakQueue.Enqueue(circuitBreak);
            base.ConsumeCircuitBreak(circuitBreak);
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
                            collection = StockMasterDb.GetCollection<StockMaster>(stockMaster.Code);
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
                collection.DeleteMany(filter);

                collection.InsertOne(stockMaster);
                stockMasterCount++;
                logger.Info($"{stockMaster.Code} stock master saved, Tick: {Environment.TickCount - startTick}");
            }
            catch (Exception ex)
            {
                logger.Error($"{stockMaster.Code} stock master saving fail.");
                logger.Error(ex);
            }
        }

        public override void NotifyMessage(MessageTypes type, string message)
        {
            try
            {
                if (type == MessageTypes.CloseClient)
                {
                    var sb = new StringBuilder();
                    sb.AppendLine("HistorySaver result");
                    sb.AppendLine($"StockMaster : {stockMasterCount}");
                    sb.AppendLine($"StockConclusion : {stockConclusionCount}");
                    sb.AppendLine($"IndexConclusion : {indexConclusionCount}");
                    sb.AppendLine($"BiddingPrice : {biddingPriceCount}");
                    sb.AppendLine($"CircuitBreak : {circuitBreakCount}");

                    logger.Info(sb.ToString());
                    PushUtility.NotifyMessage(sb.ToString());
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            base.NotifyMessage(type, message);
        }
    }
}
