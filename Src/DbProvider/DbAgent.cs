using MongoDB.Bson;
using MongoDB.Driver;
using Configuration;
using DataStructure;
using CommonLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using CommonLib.Utility;
using System.Threading;

namespace DbProvider
{
    public class DbAgent
    {
        #region Static variable
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
        private static readonly object _lockObject = new object();

        private static volatile DbAgent _instance;
        public static DbAgent Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lockObject)
                    {
                        if (_instance == null)
                            _instance = new DbAgent(Config.Database.ConnectionString);
                    }
                }

                return _instance;
            }
        }

        private static volatile DbAgent _remoteInstance;
        public static DbAgent RemoteInstance
        {
            get
            {
                if (_remoteInstance == null)
                {
                    lock (_lockObject)
                    {
                        if (_remoteInstance == null)
                        {
                            if (string.IsNullOrEmpty(Config.Database.RemoteConnectionString) == false)
                                _remoteInstance = new DbAgent(Config.Database.RemoteConnectionString);
                            else
                                _logger.Error("Connection string for remote DB is empty");
                        }
                    }
                }

                return _remoteInstance;
            }
        }
        #endregion

        public string ConnectionString { get; private set; }

        public MongoDbProvider DbProvider { get; private set; }

        public IMongoDatabase ChartDb { get; private set; }
        public IMongoDatabase BiddingPriceDb { get; private set; }
        public IMongoDatabase CircuitBreakDb { get; private set; }
        public IMongoDatabase StockMasterDb { get; private set; }
        public IMongoDatabase IndexMasterDb { get; private set; }
        public IMongoDatabase StockConclusionDb { get; private set; }
        public IMongoDatabase IndexConclusionDb { get; private set; }
        public IMongoDatabase ETFConclusionDb { get; private set; }
        public IMongoDatabase TradeConclusionDb { get; private set; }
        public IMongoDatabase CommonDb { get; private set; }
        
        private DbAgent(string connectionString = null)
        {
            MongoDefaults.GuidRepresentation = GuidRepresentation.Standard;

            InitDatabase(connectionString);
        }

        private void InitDatabase(string connectionString)
        {
            ConnectionString = connectionString;

            DbProvider = new MongoDbProvider(connectionString);

            ChartDb = DbProvider.GetDatabase(DbTypes.Chart);
            BiddingPriceDb = DbProvider.GetDatabase(DbTypes.BiddingPrice);
            CircuitBreakDb = DbProvider.GetDatabase(DbTypes.CircuitBreak);
            StockMasterDb = DbProvider.GetDatabase(DbTypes.StockMaster);
            IndexMasterDb = DbProvider.GetDatabase(DbTypes.IndexMaster);
            StockConclusionDb = DbProvider.GetDatabase(DbTypes.StockConclusion);
            IndexConclusionDb = DbProvider.GetDatabase(DbTypes.IndexConclusion);
            ETFConclusionDb = DbProvider.GetDatabase(DbTypes.ETFConclusion);
            TradeConclusionDb = DbProvider.GetDatabase(DbTypes.TradeConclusion);
            CommonDb = DbProvider.GetDatabase(DbTypes.Common);
        }

        public void ChangeServer(string connectionString)
        {
            InitDatabase(connectionString);
        }

        public bool ConnectionTest()
        {
            return DbProvider.GetDatabaseList() != null;
        }

        public IEnumerable<string> GetCollectionList(DbTypes type)
        {
            List<string> collectionList = new List<string>();

            try
            {
                var db = DbProvider.GetDatabase(type);
                if (db == null) return collectionList;

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
                _logger.Error(ex);
            }

            return collectionList;
        }

        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            if (string.IsNullOrEmpty(collectionName) == true) return null;

            try
            {
                if (typeof(T) == typeof(BiddingPrice))
                    return (IMongoCollection<T>)BiddingPriceDb.GetCollection<BiddingPrice>(collectionName);
                else if (typeof(T) == typeof(StockConclusion))
                    return (IMongoCollection<T>)StockConclusionDb.GetCollection<StockConclusion>(collectionName);
                else if (typeof(T) == typeof(IndexConclusion))
                    return (IMongoCollection<T>)IndexConclusionDb.GetCollection<IndexConclusion>(collectionName);
                else if (typeof(T) == typeof(ETFConclusion))
                    return (IMongoCollection<T>)ETFConclusionDb.GetCollection<ETFConclusion>(collectionName);
                else if (typeof(T) == typeof(TradeConclusion))
                    return (IMongoCollection<T>)TradeConclusionDb.GetCollection<TradeConclusion>(collectionName);
                else if (typeof(T) == typeof(CircuitBreak))
                    return (IMongoCollection<T>)CircuitBreakDb.GetCollection<CircuitBreak>(collectionName);
                else if (typeof(T) == typeof(StockMaster))
                    return (IMongoCollection<T>)StockMasterDb.GetCollection<StockMaster>(collectionName);
                else if (typeof(T) == typeof(IndexMaster))
                    return (IMongoCollection<T>)IndexMasterDb.GetCollection<IndexMaster>(collectionName);
                else if (typeof(T) == typeof(Candle))
                    return (IMongoCollection<T>)ChartDb.GetCollection<Candle>(collectionName);
                else if (typeof(T) == typeof(DataCounter))
                    return (IMongoCollection<T>)CommonDb.GetCollection<DataCounter>(collectionName);
                else if (typeof(T) == typeof(CodeMapDbObject))
                    return (IMongoCollection<T>)CommonDb.GetCollection<CodeMapDbObject>(collectionName);
                else
                    _logger.Error($"Can not find collection, type: {typeof(T)}");
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return null;
        }

        public IMongoCollection<T> GetCollection<T>(T item)
        {
            return GetCollection<T>(GetCollectionName(item));
        }

        public string GetCollectionName(object item)
        {
            try
            {
                if (item is TradeConclusion tradeConclusion) // Subscribable 보다 먼저 있어야 한다.
                    return tradeConclusion.AccountNumber;
                else if (item is Subscribable subscribable)
                    return subscribable.Code;
                else if (item is DataCounter)
                    return nameof(DataCounter);
                else
                    _logger.Error($"Can not find collection name, item: {item}");
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return null;
        }

        long counter = 0;
        /// <summary>
        /// Type과 Item에 맞는 Collection을 찾아서 Insert를 수행한다
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        public void Insert<T>(T item)
        {
            if (item == null)
            {
                _logger.Error("item is null");
                return;
            }

            try
            {
                var collection = GetCollection(item);

                if (collection != null)
                    collection.InsertOne(item); 
                else
                    _logger.Error($"Insert error, {item}");
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
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
                    _logger.Error($"Delete error, {collectionName}");
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        public IFindFluent<T, T> Find<T>(string collectionName, FilterDefinition<T> filter, Expression<Func<T, object>> sortExpression = null)
        {
            if (string.IsNullOrEmpty(collectionName) == true || filter == null) return null;

            try
            {
                var collection = GetCollection<T>(collectionName);

                if (collection != null)
                {
                    if (sortExpression == null)
                        return GetCollection<T>(collectionName).Find(filter);
                    else
                        return GetCollection<T>(collectionName).Find(filter).SortBy(sortExpression);
                }
                else
                {
                    _logger.Error($"Find error, {collectionName}");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
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
                _logger.Info($"Create database index, recreate: {recreate}");

                #region Chart
                foreach (var collectionName in GetCollectionList(DbTypes.Chart))
                {
                    var collection = GetCollection<Candle>(collectionName);

                    if (recreate == true)
                        collection.Indexes.DropAll();

                    collection.Indexes.CreateOne(Builders<Candle>.IndexKeys.Ascending(i => i.CandleType).Ascending(i => i.Time));
                }
                #endregion

                #region BiddingPrice
                foreach (var collectionName in GetCollectionList(DbTypes.BiddingPrice))
                {
                    var collection = GetCollection<BiddingPrice>(collectionName);

                    if (recreate == true)
                        collection.Indexes.DropAll();

                    collection.Indexes.CreateOne(Builders<BiddingPrice>.IndexKeys.Ascending(i => i.Time));
                }
                #endregion

                #region CircuitBreak
                foreach (var collectionName in GetCollectionList(DbTypes.CircuitBreak))
                {
                    var collection = GetCollection<CircuitBreak>(collectionName);

                    if (recreate == true)
                        collection.Indexes.DropAll();

                    collection.Indexes.CreateOne(Builders<CircuitBreak>.IndexKeys.Ascending(i => i.Time));
                }
                #endregion

                #region StockMaster
                foreach (var collectionName in GetCollectionList(DbTypes.StockMaster))
                {
                    var collection = GetCollection<StockMaster>(collectionName);

                    if (recreate == true)
                        collection.Indexes.DropAll();

                    collection.Indexes.CreateOne(Builders<StockMaster>.IndexKeys.Ascending(i => i.Time));
                }
                #endregion

                #region IndexMaster
                foreach (var collectionName in GetCollectionList(DbTypes.IndexMaster))
                {
                    var collection = GetCollection<IndexMaster>(collectionName);

                    if (recreate == true)
                        collection.Indexes.DropAll();

                    collection.Indexes.CreateOne(Builders<IndexMaster>.IndexKeys.Ascending(i => i.Time));
                }
                #endregion

                #region StockConclusion
                foreach (var collectionName in GetCollectionList(DbTypes.StockConclusion))
                {
                    var collection = GetCollection<StockConclusion>(collectionName);

                    if (recreate == true)
                        collection.Indexes.DropAll();

                    collection.Indexes.CreateOne(Builders<StockConclusion>.IndexKeys.Ascending(i => i.Time));
                }
                #endregion

                #region IndexConclusion
                foreach (var collectionName in GetCollectionList(DbTypes.IndexConclusion))
                {
                    var collection = GetCollection<IndexConclusion>(collectionName);

                    if (recreate == true)
                        collection.Indexes.DropAll();

                    collection.Indexes.CreateOne(Builders<IndexConclusion>.IndexKeys.Ascending(i => i.Time));
                }
                #endregion

                #region ETFConclusion
                foreach (var collectionName in GetCollectionList(DbTypes.ETFConclusion))
                {
                    var collection = GetCollection<ETFConclusion>(collectionName);

                    if (recreate == true)
                        collection.Indexes.DropAll();

                    collection.Indexes.CreateOne(Builders<ETFConclusion>.IndexKeys.Ascending(i => i.Time));
                }
                #endregion

                #region TradeConclusion
                foreach (var collectionName in GetCollectionList(DbTypes.TradeConclusion))
                {
                    var collection = GetCollection<TradeConclusion>(collectionName);

                    if (recreate == true)
                        collection.Indexes.DropAll();

                    collection.Indexes.CreateOne(Builders<TradeConclusion>.IndexKeys.Ascending(i => i.Time));
                }
                #endregion

                _logger.Info("Database indexing done");
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            finally
            {
                sw.Stop();
                _logger.Info($"Database indexing Elapsed time: {sw.Elapsed.ToString()}");
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
                _logger.Info("Save database statictic");

                var startDate = DateTimeUtility.StartDateTime(DateTime.Now);
                var endDate = DateTimeUtility.EndDateTime(DateTime.Now);

                var Counter = new DataCounter(DataTypes.Database);

                #region Chart
                foreach (var collectionName in GetCollectionList(DbTypes.Chart))
                {
                    var collection = GetCollection<Candle>(collectionName);
                    var builder = Builders<Candle>.Filter;
                    var filter = builder.Empty;
                    Counter.ChartCount += (int)collection.Find(filter).Count();
                }
                #endregion

                #region BiddingPrice
                foreach (var collectionName in GetCollectionList(DbTypes.BiddingPrice))
                {
                    var collection = GetCollection<BiddingPrice>(collectionName);
                    var builder = Builders<BiddingPrice>.Filter;
                    var filter = builder.Gte(i => i.Time, startDate) & builder.Lte(i => i.Time, endDate);
                    Counter.BiddingPriceCount += (int)collection.Find(filter).Count();
                }
                #endregion

                #region CircuitBreak
                foreach (var collectionName in GetCollectionList(DbTypes.CircuitBreak))
                {
                    var collection = GetCollection<CircuitBreak>(collectionName);
                    var builder = Builders<CircuitBreak>.Filter;
                    var filter = builder.Gte(i => i.Time, startDate) & builder.Lte(i => i.Time, endDate);
                    Counter.CircuitBreakCount += (int)collection.Find(filter).Count();
                }
                #endregion

                #region StockMaster
                foreach (var collectionName in GetCollectionList(DbTypes.StockMaster))
                {
                    var collection = GetCollection<StockMaster>(collectionName);
                    var builder = Builders<StockMaster>.Filter;
                    var filter = builder.Gte(i => i.Time, startDate) & builder.Lte(i => i.Time, endDate);
                    Counter.StockMasterCount += (int)collection.Find(filter).Count();
                }
                #endregion

                #region IndexMaster
                foreach (var collectionName in GetCollectionList(DbTypes.IndexMaster))
                {
                    var collection = GetCollection<IndexMaster>(collectionName);
                    var builder = Builders<IndexMaster>.Filter;
                    var filter = builder.Gte(i => i.Time, startDate) & builder.Lte(i => i.Time, endDate);
                    Counter.IndexMasterCount += (int)collection.Find(filter).Count();
                }
                #endregion

                #region StockConclusion
                foreach (var collectionName in GetCollectionList(DbTypes.StockConclusion))
                {
                    var collection = GetCollection<StockConclusion>(collectionName);
                    var builder = Builders<StockConclusion>.Filter;
                    var filter = builder.Gte(i => i.Time, startDate) & builder.Lte(i => i.Time, endDate);
                    Counter.StockConclusionCount += (int)collection.Find(filter).Count();
                }
                #endregion

                #region IndexConclusion
                foreach (var collectionName in GetCollectionList(DbTypes.IndexConclusion))
                {
                    var collection = GetCollection<IndexConclusion>(collectionName);
                    var builder = Builders<IndexConclusion>.Filter;
                    var filter = builder.Gte(i => i.Time, startDate) & builder.Lte(i => i.Time, endDate);
                    Counter.IndexConclusionCount += (int)collection.Find(filter).Count();
                }
                #endregion

                #region ETFConclusion
                foreach (var collectionName in GetCollectionList(DbTypes.ETFConclusion))
                {
                    var collection = GetCollection<ETFConclusion>(collectionName);
                    var builder = Builders<ETFConclusion>.Filter;
                    var filter = builder.Gte(i => i.Time, startDate) & builder.Lte(i => i.Time, endDate);
#if false // Filter logging
                    var renderedFilter = filter.Render(collection.DocumentSerializer, collection.Settings.SerializerRegistry);
                    Trace.WriteLine(renderedFilter.ToJson()); 
#endif
                    Counter.ETFConclusionCount += (int)collection.Find(filter).Count();
                }
                #endregion

                #region TradeConclusion
                foreach (var collectionName in GetCollectionList(DbTypes.TradeConclusion))
                {
                    var collection = GetCollection<TradeConclusion>(collectionName);
                    var builder = Builders<TradeConclusion>.Filter;
                    var filter = builder.Gte(i => i.Time, startDate) & builder.Lte(i => i.Time, endDate);
                    Counter.TradeConclusionCount += (int)collection.Find(filter).Count();
                }
                #endregion

                // DB 통계를 파일로 저장
                Counter.SaveToFile();

                // DB 통계를 DB에 저장
                Insert(Counter);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            finally
            {
                sw.Stop();
                _logger.Info($"Database statistics Elapsed time: {sw.Elapsed.ToString()}");
            }
        }
    }
}
