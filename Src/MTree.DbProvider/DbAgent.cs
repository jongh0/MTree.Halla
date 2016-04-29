using MongoDB.Driver;
using MTree.Configuration;
using MTree.DataStructure;
using MTree.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace MTree.DbProvider
{
    public class DbAgent
    {
        #region Static variable
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private static object lockObject = new object();

        private static volatile DbAgent _Instance;
        public static DbAgent Instance
        {
            get
            {
                if (_Instance == null)
                {
                    lock (lockObject)
                    {
                        if (_Instance == null)
                            _Instance = new DbAgent(Config.Database.ConnectionString);
                    }
                }

                return _Instance;
            }
        }

        private static volatile DbAgent _RemoteInstance;
        public static DbAgent RemoteInstance
        {
            get
            {
                if (_RemoteInstance == null)
                {
                    lock (lockObject)
                    {
                        if (_RemoteInstance == null)
                        {
                            if (Config.Database.RemoteConnectionString != string.Empty)
                                _RemoteInstance = new DbAgent(Config.Database.RemoteConnectionString);
                            else
                                logger.Error("Connection string for remote DB is empty");
                        }
                    }
                }

                return _RemoteInstance;
            }
        }
        #endregion

        public MongoDbProvider DbProvider { get; set; }

        public IMongoDatabase ChartDb { get; set; }
        public IMongoDatabase BiddingPriceDb { get; set; }
        public IMongoDatabase CircuitBreakDb { get; set; }
        public IMongoDatabase StockMasterDb { get; set; }
        public IMongoDatabase IndexMasterDb { get; set; }
        public IMongoDatabase StockConclusionDb { get; set; }
        public IMongoDatabase IndexConclusionDb { get; set; }
        public IMongoDatabase CommonDb { get; set; }

        public DbAgent(string connectionString = null)
        {
            DbProvider = new MongoDbProvider(connectionString);

            ChartDb = DbProvider.GetDatabase(DbTypes.Chart);
            BiddingPriceDb = DbProvider.GetDatabase(DbTypes.BiddingPrice);
            CircuitBreakDb = DbProvider.GetDatabase(DbTypes.CircuitBreak);
            StockMasterDb = DbProvider.GetDatabase(DbTypes.StockMaster);
            IndexMasterDb = DbProvider.GetDatabase(DbTypes.IndexMaster);
            StockConclusionDb = DbProvider.GetDatabase(DbTypes.StockConclusion);
            IndexConclusionDb = DbProvider.GetDatabase(DbTypes.IndexConclusion);
            CommonDb = DbProvider.GetDatabase(DbTypes.Common);
        }

        public List<string> GetCollectionList(DbTypes type)
        {
            List<string> collectionList = new List<string>();

            try
            {
                var db = DbProvider.GetDatabase(type);
                using (var cursor = db.ListCollections())
                {
                    foreach (var doc in cursor.ToList())
                    {
                        var collectionName = doc.GetElement("name").Value.AsString;
                        collectionList.Add(collectionName);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return collectionList;
        }

        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            try
            {
                if (typeof(T) == typeof(Candle))
                    return (IMongoCollection<T>)ChartDb.GetCollection<Candle>(collectionName);
                else if (typeof(T) == typeof(BiddingPrice))
                    return (IMongoCollection<T>)BiddingPriceDb.GetCollection<BiddingPrice>(collectionName);
                else if (typeof(T) == typeof(CircuitBreak))
                    return (IMongoCollection<T>)CircuitBreakDb.GetCollection<CircuitBreak>(collectionName);
                else if (typeof(T) == typeof(StockMaster))
                    return (IMongoCollection<T>)StockMasterDb.GetCollection<StockMaster>(collectionName);
                else if (typeof(T) == typeof(IndexMaster))
                    return (IMongoCollection<T>)IndexMasterDb.GetCollection<IndexMaster>(collectionName);
                else if (typeof(T) == typeof(StockConclusion))
                    return (IMongoCollection<T>)StockConclusionDb.GetCollection<StockConclusion>(collectionName);
                else if (typeof(T) == typeof(IndexConclusion))
                    return (IMongoCollection<T>)IndexConclusionDb.GetCollection<IndexConclusion>(collectionName);
                else if (typeof(T) == typeof(DataCounter))
                    return (IMongoCollection<T>)CommonDb.GetCollection<DataCounter>(collectionName);
                else
                    logger.Error($"Can not find collection, type: {typeof(T)}");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return null;
        }

        public IMongoCollection<T> GetCollection<T>(T item)
        {
            try
            {
                var collectionName = GetCollectionName(item);
                if (collectionName == null)
                {
                    logger.Error("Collection name is null");
                    return null;
                }

                if (item is Candle)
                    return (IMongoCollection<T>)ChartDb.GetCollection<Candle>(collectionName);
                else if (item is BiddingPrice)
                    return (IMongoCollection<T>)BiddingPriceDb.GetCollection<BiddingPrice>(collectionName);
                else if (item is CircuitBreak)
                    return (IMongoCollection<T>)CircuitBreakDb.GetCollection<CircuitBreak>(collectionName);
                else if (item is StockMaster)
                    return (IMongoCollection<T>)StockMasterDb.GetCollection<StockMaster>(collectionName);
                else if (item is IndexMaster)
                    return (IMongoCollection<T>)IndexMasterDb.GetCollection<IndexMaster>(collectionName);
                else if (item is StockConclusion)
                    return (IMongoCollection<T>)StockConclusionDb.GetCollection<StockConclusion>(collectionName);
                else if (item is IndexConclusion)
                    return (IMongoCollection<T>)IndexConclusionDb.GetCollection<IndexConclusion>(collectionName);
                else if (item is DataCounter)
                    return (IMongoCollection<T>)CommonDb.GetCollection<DataCounter>(collectionName);
                else
                    logger.Error($"Can not find collection, item: {item}");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return null;
        }

        public string GetCollectionName<T>(T item)
        {
            try
            {
                if (item is Subscribable)
                    return (item as Subscribable).Code;
                else if (item is DataCounter)
                    return nameof(DataCounter);
                else
                    logger.Error($"Can not find collection name, item: {item}");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return null;
        }

        /// <summary>
        /// Type과 Item에 맞는 Collection을 찾아서 Insert를 수행한다
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        public void Insert<T>(T item)
        {
            if (item == null)
            {
                logger.Error("item is null");
                return;
            }

            try
            {
                var collection = GetCollection(item);

                if (collection != null)
                    collection.InsertOne(item);
                else
                    logger.Error($"Insert error, {item}");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public void Delete<T>(string collectionName, FilterDefinition<T> filter)
        {
            if (string.IsNullOrEmpty(collectionName) == true || filter == null) return;

            try
            {
                var collection = GetCollection<T>(collectionName);

                if (collection != null)
                    collection.DeleteMany(filter);
                else
                    logger.Error($"Delete error, {collectionName}");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public IFindFluent<T, T> Find<T>(string collectionName, FilterDefinition<T> filter)
        {
            if (string.IsNullOrEmpty(collectionName) == true || filter == null) return null;

            try
            {
                var collection = GetCollection<T>(collectionName);

                if (collection != null)
                    return GetCollection<T>(collectionName).Find(filter);
                else
                    logger.Error($"Find error, {collectionName}");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return null;
        }

        /// <summary>
        /// Database에 Index를 생성한다.
        /// </summary>
        /// <param name="recreate">True: 기존 Index를 Drop 후 다시 만든다.</param>
        public void CreateIndex(bool recreate = false)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            try
            {
                logger.Info($"Create database index, recreate: {recreate}");

                foreach (var collectionName in GetCollectionList(DbTypes.Chart))
                {
                    var collection = GetCollection<Candle>(collectionName);
                    var keys = Builders<Candle>.IndexKeys.Ascending(i => i.CandleType).Ascending(i => i.ReceivedTime);

                    if (recreate == true)
                        collection.Indexes.DropAll();

                    collection.Indexes.CreateOne(keys);
                }

                foreach (var collectionName in GetCollectionList(DbTypes.BiddingPrice))
                {
                    var collection = GetCollection<BiddingPrice>(collectionName);
                    var keys = Builders<BiddingPrice>.IndexKeys.Ascending(i => i.ReceivedTime);

                    if (recreate == true)
                        collection.Indexes.DropAll();

                    collection.Indexes.CreateOne(keys);
                }

                foreach (var collectionName in GetCollectionList(DbTypes.CircuitBreak))
                {
                    var collection = GetCollection<CircuitBreak>(collectionName);
                    var keys = Builders<CircuitBreak>.IndexKeys.Ascending(i => i.ReceivedTime);

                    if (recreate == true)
                        collection.Indexes.DropAll();

                    collection.Indexes.CreateOne(keys);
                }

                foreach (var collectionName in GetCollectionList(DbTypes.StockMaster))
                {
                    var collection = GetCollection<StockMaster>(collectionName);
                    var keys = Builders<StockMaster>.IndexKeys.Ascending(i => i.ReceivedTime);

                    if (recreate == true)
                        collection.Indexes.DropAll();

                    collection.Indexes.CreateOne(keys);
                }

                foreach (var collectionName in GetCollectionList(DbTypes.IndexMaster))
                {
                    var collection = GetCollection<IndexMaster>(collectionName);
                    var keys = Builders<IndexMaster>.IndexKeys.Ascending(i => i.ReceivedTime);

                    if (recreate == true)
                        collection.Indexes.DropAll();

                    collection.Indexes.CreateOne(keys);
                }

                foreach (var collectionName in GetCollectionList(DbTypes.StockConclusion))
                {
                    var collection = GetCollection<StockConclusion>(collectionName);
                    var keys = Builders<StockConclusion>.IndexKeys.Ascending(i => i.ReceivedTime);

                    if (recreate == true)
                        collection.Indexes.DropAll();

                    collection.Indexes.CreateOne(keys);
                }

                foreach (var collectionName in GetCollectionList(DbTypes.IndexConclusion))
                {
                    var collection = GetCollection<IndexConclusion>(collectionName);
                    var keys = Builders<IndexConclusion>.IndexKeys.Ascending(i => i.ReceivedTime);

                    if (recreate == true)
                        collection.Indexes.DropAll();

                    collection.Indexes.CreateOne(keys);
                }

                var msg = "Database indexing done";
                logger.Info(msg);
                PushUtility.NotifyMessage(msg);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                sw.Stop();
                logger.Info($"Database indexing Elapsed time: {sw.Elapsed.ToString()}");
            }
        }

        /// <summary>
        /// 오늘 Database에 저장된 내용의 통계를 로그에 기록한다.
        /// </summary>
        public void SaveStatisticLog()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            try
            {
                logger.Info("Save database statictic");

                var startDate = DateTimeUtility.StartDateTime(DateTime.Now);
                var endDate = DateTimeUtility.EndDateTime(DateTime.Now);

                var Counter = new DataCounter(DataTypes.Database);

                foreach (var collectionName in GetCollectionList(DbTypes.Chart))
                {
                    var collection = GetCollection<Candle>(collectionName);
                    var builder = Builders<Candle>.Filter;
                    var filter = builder.Empty;
                    Counter.ChartCount += (int)collection.Find(filter).Count();
                }

                foreach (var collectionName in GetCollectionList(DbTypes.BiddingPrice))
                {
                    var collection = GetCollection<BiddingPrice>(collectionName);
                    var builder = Builders<BiddingPrice>.Filter;
                    var filter = builder.Gte(i => i.Time, startDate) & builder.Lte(i => i.Time, endDate);
                    Counter.BiddingPriceCount += (int)collection.Find(filter).Count();
                }

                foreach (var collectionName in GetCollectionList(DbTypes.CircuitBreak))
                {
                    var collection = GetCollection<CircuitBreak>(collectionName);
                    var builder = Builders<CircuitBreak>.Filter;
                    var filter = builder.Gte(i => i.Time, startDate) & builder.Lte(i => i.Time, endDate);
                    Counter.CircuitBreakCount += (int)collection.Find(filter).Count();
                }

                foreach (var collectionName in GetCollectionList(DbTypes.StockMaster))
                {
                    var collection = GetCollection<StockMaster>(collectionName);
                    var builder = Builders<StockMaster>.Filter;
                    var filter = builder.Gte(i => i.Time, startDate) & builder.Lte(i => i.Time, endDate);
                    Counter.StockMasterCount += (int)collection.Find(filter).Count();
                }

                foreach (var collectionName in GetCollectionList(DbTypes.IndexMaster))
                {
                    var collection = GetCollection<IndexMaster>(collectionName);
                    var builder = Builders<IndexMaster>.Filter;
                    var filter = builder.Gte(i => i.Time, startDate) & builder.Lte(i => i.Time, endDate);
                    Counter.IndexMasterCount += (int)collection.Find(filter).Count();
                }

                foreach (var collectionName in GetCollectionList(DbTypes.StockConclusion))
                {
                    var collection = GetCollection<StockConclusion>(collectionName);
                    var builder = Builders<StockConclusion>.Filter;
                    var filter = builder.Gte(i => i.Time, startDate) & builder.Lte(i => i.Time, endDate);
                    Counter.StockConclusionCount += (int)collection.Find(filter).Count();
                }

                foreach (var collectionName in GetCollectionList(DbTypes.IndexConclusion))
                {
                    var collection = GetCollection<IndexConclusion>(collectionName);
                    var builder = Builders<IndexConclusion>.Filter;
                    var filter = builder.Gte(i => i.Time, startDate) & builder.Lte(i => i.Time, endDate);
                    Counter.IndexConclusionCount += (int)collection.Find(filter).Count();
                }

                PushUtility.NotifyMessage("DB Statistics" + Environment.NewLine + Counter.ToString());

                // DB 통계를 파일로 저장
                Counter.SaveToFile();

                // DB 통계를 DB에 저장
                Insert(Counter);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                sw.Stop();
                logger.Info($"Database statistics Elapsed time: {sw.Elapsed.ToString()}");
            }
        }
    }
}
