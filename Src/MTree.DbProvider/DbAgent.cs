using MongoDB.Driver;
using MTree.Configuration;
using MTree.DataStructure;
using MTree.DbProvider;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.DbProvider
{
    public class DbAgent
    {
        #region Static variable
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private static object lockObject = new object();

        private static volatile DbAgent _intance;
        public static DbAgent Instance
        {
            get
            {
                if (_intance == null)
                {
                    lock (lockObject)
                    {
                        if (_intance == null)
                            _intance = new DbAgent();
                    }
                }

                return _intance;
            }
        }
        #endregion

        #region Db & Collection
        public MongoDbProvider DbProvider { get; set; } = new MongoDbProvider();

        public int CandleCount { get; private set; } = 0;
        public int CircuitBreakCount { get; private set; } = 0;
        public int BiddingPriceCount { get; private set; } = 0;
        public int StockMasterCount { get; private set; } = 0;
        public int StockConclusionCount { get; private set; } = 0;
        public int IndexConclusionCount { get; private set; } = 0;

        private IMongoDatabase CandleDb { get; set; }
        private IMongoDatabase BiddingPriceDb { get; set; }
        private IMongoDatabase CircuitBreakDb { get; set; }
        private IMongoDatabase StockMasterDb { get; set; }
        private IMongoDatabase StockConclusionDb { get; set; }
        private IMongoDatabase IndexConclusionDb { get; set; }

        private ConcurrentDictionary<string, IMongoCollection<Candle>> CandleCollections { get; set; } = new ConcurrentDictionary<string, IMongoCollection<Candle>>();
        private ConcurrentDictionary<string, IMongoCollection<BiddingPrice>> BiddingPriceCollections { get; set; } = new ConcurrentDictionary<string, IMongoCollection<BiddingPrice>>();
        private ConcurrentDictionary<string, IMongoCollection<CircuitBreak>> CircuitBreakCollections { get; set; } = new ConcurrentDictionary<string, IMongoCollection<CircuitBreak>>();
        private ConcurrentDictionary<string, IMongoCollection<StockMaster>> StockMasterCollections { get; set; } = new ConcurrentDictionary<string, IMongoCollection<StockMaster>>();
        private ConcurrentDictionary<string, IMongoCollection<StockConclusion>> StockConclusionCollections { get; set; } = new ConcurrentDictionary<string, IMongoCollection<StockConclusion>>();
        private ConcurrentDictionary<string, IMongoCollection<IndexConclusion>> IndexConclusionCollections { get; set; } = new ConcurrentDictionary<string, IMongoCollection<IndexConclusion>>();
        #endregion

        public DbAgent()
        {
            CandleDb = DbProvider.GetDatabase(DbTypes.Candle);
            BiddingPriceDb = DbProvider.GetDatabase(DbTypes.BiddingPrice);
            CircuitBreakDb = DbProvider.GetDatabase(DbTypes.CircuitBreak);
            StockMasterDb = DbProvider.GetDatabase(DbTypes.StockMaster);
            StockConclusionDb = DbProvider.GetDatabase(DbTypes.StockConclusion);
            IndexConclusionDb = DbProvider.GetDatabase(DbTypes.IndexConclusion);
        }

        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            try
            {
                #region Candle
                if (typeof(T) == typeof(Candle))
                {
                    IMongoCollection<Candle> collection = null;

                    if (CandleCollections.ContainsKey(collectionName) == false)
                    {
                        lock (CandleCollections)
                        {
                            if (CandleCollections.ContainsKey(collectionName) == false)
                            {
                                collection = CandleDb.GetCollection<Candle>(collectionName);
                                var keys = Builders<Candle>.IndexKeys.Ascending(i => i.CandleType).Ascending(i => i.Time);
                                collection.Indexes.CreateOneAsync(keys);

                                CandleCollections.TryAdd(collectionName, collection);
                            }
                            else
                            {
                                collection = CandleCollections[collectionName];
                            }
                        }
                    }
                    else
                    {
                        collection = CandleCollections[collectionName];
                    }

                    return (IMongoCollection<T>)collection;
                }
                #endregion
                #region StockMaster
                else if (typeof(T) == typeof(StockMaster))
                {
                    IMongoCollection<StockMaster> collection = null;

                    if (StockMasterCollections.ContainsKey(collectionName) == false)
                    {
                        lock (StockMasterCollections)
                        {
                            if (StockMasterCollections.ContainsKey(collectionName) == false)
                            {
                                collection = StockMasterDb.GetCollection<StockMaster>(collectionName);
                                var keys = Builders<StockMaster>.IndexKeys.Ascending(i => i.Time);
                                collection.Indexes.CreateOneAsync(keys);

                                StockMasterCollections.TryAdd(collectionName, collection);
                            }
                            else
                            {
                                collection = StockMasterCollections[collectionName];
                            }
                        }
                    }
                    else
                    {
                        collection = StockMasterCollections[collectionName];
                    }

                    return (IMongoCollection<T>)collection;
                }
                #endregion
                #region BiddingPrice
                else if (typeof(T) == typeof(BiddingPrice))
                {
                    IMongoCollection<BiddingPrice> collection = null;

                    if (BiddingPriceCollections.ContainsKey(collectionName) == false)
                    {
                        lock (BiddingPriceCollections)
                        {
                            if (BiddingPriceCollections.ContainsKey(collectionName) == false)
                            {
                                collection = BiddingPriceDb.GetCollection<BiddingPrice>(collectionName);
                                var keys = Builders<BiddingPrice>.IndexKeys.Ascending(i => i.Time);
                                collection.Indexes.CreateOneAsync(keys);

                                BiddingPriceCollections.TryAdd(collectionName, collection);
                            }
                            else
                            {
                                collection = BiddingPriceCollections[collectionName];
                            }
                        }
                    }
                    else
                    {
                        collection = BiddingPriceCollections[collectionName];
                    }

                    return (IMongoCollection<T>)collection;
                }
                #endregion
                #region StockConclusion
                else if (typeof(T) == typeof(StockConclusion))
                {
                    IMongoCollection<StockConclusion> collection = null;

                    if (StockConclusionCollections.ContainsKey(collectionName) == false)
                    {
                        lock (StockConclusionCollections)
                        {
                            if (StockConclusionCollections.ContainsKey(collectionName) == false)
                            {
                                collection = StockConclusionDb.GetCollection<StockConclusion>(collectionName);
                                var keys = Builders<StockConclusion>.IndexKeys.Ascending(i => i.Time);
                                collection.Indexes.CreateOneAsync(keys);

                                StockConclusionCollections.TryAdd(collectionName, collection);
                            }
                            else
                            {
                                collection = StockConclusionCollections[collectionName];
                            }
                        }
                    }
                    else
                    {
                        collection = StockConclusionCollections[collectionName];
                    }

                    return (IMongoCollection<T>)collection;
                }
                #endregion
                #region IndexConclusion
                else if (typeof(T) == typeof(IndexConclusion))
                {
                    IMongoCollection<IndexConclusion> collection = null;

                    if (IndexConclusionCollections.ContainsKey(collectionName) == false)
                    {
                        lock (IndexConclusionCollections)
                        {
                            if (IndexConclusionCollections.ContainsKey(collectionName) == false)
                            {
                                collection = IndexConclusionDb.GetCollection<IndexConclusion>(collectionName);
                                var keys = Builders<IndexConclusion>.IndexKeys.Ascending(i => i.Time);
                                collection.Indexes.CreateOneAsync(keys);

                                IndexConclusionCollections.TryAdd(collectionName, collection);
                            }
                            else
                            {
                                collection = IndexConclusionCollections[collectionName];
                            }
                        }
                    }
                    else
                    {
                        collection = IndexConclusionCollections[collectionName];
                    }

                    return (IMongoCollection<T>)collection;
                } 
                #endregion
                else
                {
                    logger.Error($"Can't find collection, type: {typeof(T)}");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return null;
        }

        #region Insert
        public void InsertItem(Candle item)
        {
            try
            {
                var collection = GetCollection<Candle>(item.Code);

                if (collection != null)
                {
                    collection.InsertOne(item);
                    CandleCount++;
                }
                else
                {
                    logger.Error($"Insert error, {item.Code}/{item.Time.ToString(Config.Instance.General.DateTimeFormat)}");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public void InsertItem(StockMaster item)
        {
            try
            {
                var collection = GetCollection<StockMaster>(item.Code);

                if (collection != null)
                {
                    collection.InsertOne(item);
                    StockMasterCount++;
                }
                else
                {
                    logger.Error($"Insert error, {item.Code}/{item.Time.ToString(Config.Instance.General.DateTimeFormat)}");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public void InsertItem(BiddingPrice item)
        {
            try
            {
                var collection = GetCollection<BiddingPrice>(item.Code);

                if (collection != null)
                {
                    collection.InsertOne(item);
                    BiddingPriceCount++;
                }
                else
                {
                    logger.Error($"Insert error, {item.Code}/{item.Time.ToString(Config.Instance.General.DateTimeFormat)}");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public void InsertItem(CircuitBreak item)
        {
            try
            {
                var collection = GetCollection<CircuitBreak>(item.Code);

                if (collection != null)
                {
                    collection.InsertOne(item);
                    CircuitBreakCount++;
                }
                else
                {
                    logger.Error($"Insert error, {item.Code}/{item.Time.ToString(Config.Instance.General.DateTimeFormat)}");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public void InsertItem(StockConclusion item)
        {
            try
            {
                var collection = GetCollection<StockConclusion>(item.Code);

                if (collection != null)
                {
                    collection.InsertOne(item);
                    StockConclusionCount++;
                }
                else
                {
                    logger.Error($"Insert error, {item.Code}/{item.Time.ToString(Config.Instance.General.DateTimeFormat)}");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public void InsertItem(IndexConclusion item)
        {
            try
            {
                var collection = GetCollection<IndexConclusion>(item.Code);

                if (collection != null)
                {
                    collection.InsertOne(item);
                    IndexConclusionCount++;
                }
                else
                {
                    logger.Error($"Insert error, {item.Code}/{item.Time.ToString(Config.Instance.General.DateTimeFormat)}");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        } 
        #endregion
    }
}
