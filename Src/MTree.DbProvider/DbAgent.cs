using MongoDB.Driver;
using MTree.Configuration;
using MTree.DataStructure;
using MTree.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            int startTick = Environment.TickCount;

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
                else
                    logger.Error($"Can't find collection, type: {typeof(T)}");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                var duration = Environment.TickCount - startTick;
                if (duration > 2000)
                    logger.Warn($"Db get collection duration: {duration}, {collectionName}");
            }

            return null;
        }

        /// <summary>
        /// Type과 Item에 맞는 Collection을 찾아서 Async Insert를 수행한다
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        public void Insert<T>(T item)
        {
            if (item == null) return;

            int startTick = Environment.TickCount;

            try
            {
                var subscribable = item as Subscribable;
                var collection = GetCollection<T>(subscribable.Code);

                if (collection != null)
                    collection.InsertOne(item);
                else
                    logger.Error($"Insert error, {subscribable.Code}/{subscribable.Time.ToString(Config.General.DateTimeFormat)}");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                var duration = Environment.TickCount - startTick;
                if (duration > 2000)
                    logger.Warn($"Db insert duration: {duration}, {item.ToString()}");
            }
        }

        public void Insert<T>(T[] items)
        {
            if (items == null) return;

            foreach (var item in items)
                Insert(item);
        }

        public void Delete<T>(string collectionName, FilterDefinition<T> filter)
        {
            if (string.IsNullOrEmpty(collectionName) == true || filter == null) return;

            int startTick = Environment.TickCount;

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
            finally
            {
                var duration = Environment.TickCount - startTick;
                if (duration > 5000)
                    logger.Error($"Db delete duration: {duration}, {filter.ToString()}");
                else if (duration > 2000)
                    logger.Warn($"Db delete duration: {duration}, {filter.ToString()}");
            }
        }

        public IFindFluent<T, T> Find<T>(string collectionName, FilterDefinition<T> filter)
        {
            if (string.IsNullOrEmpty(collectionName) == true || filter == null) return null;

            int startTick = Environment.TickCount;

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
            finally
            {
                var duration = Environment.TickCount - startTick;
                if (duration > 5000)
                    logger.Error($"Db find duration: {duration}, {filter.ToString()}");
                else if (duration > 2000)
                    logger.Warn($"Db find duration: {duration}, {filter.ToString()}");
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
                    var keys = Builders<Candle>.IndexKeys.Ascending(i => i.CandleType).Ascending(i => i.Time);

                    if (recreate == true)
                        collection.Indexes.DropAll();

                    collection.Indexes.CreateOne(keys);
                }

                foreach (var collectionName in GetCollectionList(DbTypes.BiddingPrice))
                {
                    var collection = GetCollection<BiddingPrice>(collectionName);
                    var keys = Builders<BiddingPrice>.IndexKeys.Ascending(i => i.Time);

                    if (recreate == true)
                        collection.Indexes.DropAll();

                    collection.Indexes.CreateOne(keys);
                }

                foreach (var collectionName in GetCollectionList(DbTypes.CircuitBreak))
                {
                    var collection = GetCollection<CircuitBreak>(collectionName);
                    var keys = Builders<CircuitBreak>.IndexKeys.Ascending(i => i.Time);

                    if (recreate == true)
                        collection.Indexes.DropAll();

                    collection.Indexes.CreateOne(keys);
                }

                foreach (var collectionName in GetCollectionList(DbTypes.StockMaster))
                {
                    var collection = GetCollection<StockMaster>(collectionName);
                    var keys = Builders<StockMaster>.IndexKeys.Ascending(i => i.Time);

                    if (recreate == true)
                        collection.Indexes.DropAll();

                    collection.Indexes.CreateOne(keys);
                }

                foreach (var collectionName in GetCollectionList(DbTypes.IndexMaster))
                {
                    var collection = GetCollection<IndexMaster>(collectionName);
                    var keys = Builders<IndexMaster>.IndexKeys.Ascending(i => i.Time);

                    if (recreate == true)
                        collection.Indexes.DropAll();

                    collection.Indexes.CreateOne(keys);
                }

                foreach (var collectionName in GetCollectionList(DbTypes.StockConclusion))
                {
                    var collection = GetCollection<StockConclusion>(collectionName);
                    var keys = Builders<StockConclusion>.IndexKeys.Ascending(i => i.Time);

                    if (recreate == true)
                        collection.Indexes.DropAll();

                    collection.Indexes.CreateOne(keys);
                }

                foreach (var collectionName in GetCollectionList(DbTypes.IndexConclusion))
                {
                    var collection = GetCollection<IndexConclusion>(collectionName);
                    var keys = Builders<IndexConclusion>.IndexKeys.Ascending(i => i.Time);

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

                long chartCount = 0;
                long biddingCount = 0;
                long circuitBreakCount = 0;
                long stockMasterCount = 0;
                long indexMasterCount = 0;
                long stockConclusionCount = 0;
                long indexConclusionCount = 0;
                long totalCount = 0;

                foreach (var collectionName in GetCollectionList(DbTypes.Chart))
                {
                    var collection = GetCollection<Candle>(collectionName);
                    var builder = Builders<Candle>.Filter;
                    var filter = builder.Gte(i => i.Time, startDate) & builder.Lte(i => i.Time, endDate);
                    chartCount += collection.Find(filter).Count();
                }

                foreach (var collectionName in GetCollectionList(DbTypes.BiddingPrice))
                {
                    var collection = GetCollection<BiddingPrice>(collectionName);
                    var builder = Builders<BiddingPrice>.Filter;
                    var filter = builder.Gte(i => i.Time, startDate) & builder.Lte(i => i.Time, endDate);
                    biddingCount += collection.Find(filter).Count();
                }

                foreach (var collectionName in GetCollectionList(DbTypes.CircuitBreak))
                {
                    var collection = GetCollection<CircuitBreak>(collectionName);
                    var builder = Builders<CircuitBreak>.Filter;
                    var filter = builder.Gte(i => i.Time, startDate) & builder.Lte(i => i.Time, endDate);
                    circuitBreakCount += collection.Find(filter).Count();
                }

                foreach (var collectionName in GetCollectionList(DbTypes.StockMaster))
                {
                    var collection = GetCollection<StockMaster>(collectionName);
                    var builder = Builders<StockMaster>.Filter;
                    var filter = builder.Gte(i => i.Time, startDate) & builder.Lte(i => i.Time, endDate);
                    stockMasterCount += collection.Find(filter).Count();
                }

                foreach (var collectionName in GetCollectionList(DbTypes.IndexMaster))
                {
                    var collection = GetCollection<IndexMaster>(collectionName);
                    var builder = Builders<IndexMaster>.Filter;
                    var filter = builder.Gte(i => i.Time, startDate) & builder.Lte(i => i.Time, endDate);
                    indexMasterCount += collection.Find(filter).Count();
                }

                foreach (var collectionName in GetCollectionList(DbTypes.StockConclusion))
                {
                    var collection = GetCollection<StockConclusion>(collectionName);
                    var builder = Builders<StockConclusion>.Filter;
                    var filter = builder.Gte(i => i.Time, startDate) & builder.Lte(i => i.Time, endDate);
                    stockConclusionCount += collection.Find(filter).Count();
                }

                foreach (var collectionName in GetCollectionList(DbTypes.IndexConclusion))
                {
                    var collection = GetCollection<IndexConclusion>(collectionName);
                    var builder = Builders<IndexConclusion>.Filter;
                    var filter = builder.Gte(i => i.Time, startDate) & builder.Lte(i => i.Time, endDate);
                    indexConclusionCount += collection.Find(filter).Count();
                }

                totalCount = chartCount + biddingCount + circuitBreakCount + stockMasterCount + indexMasterCount + stockConclusionCount + indexConclusionCount;

                var sb = new StringBuilder();
                sb.AppendLine("DB Statistics");
                sb.AppendLine($"Chart: {chartCount.ToString(Config.General.CurrencyFormat)}");
                sb.AppendLine($"CircuitBreak: {circuitBreakCount.ToString(Config.General.CurrencyFormat)}");
                sb.AppendLine($"BiddingPrice: {biddingCount.ToString(Config.General.CurrencyFormat)}");
                sb.AppendLine($"StockMaster: {stockMasterCount.ToString(Config.General.CurrencyFormat)}");
                sb.AppendLine($"IndexMaster: {indexMasterCount.ToString(Config.General.CurrencyFormat)}");
                sb.AppendLine($"StockConclusion: {stockConclusionCount.ToString(Config.General.CurrencyFormat)}");
                sb.AppendLine($"IndexConclusion: {indexConclusionCount.ToString(Config.General.CurrencyFormat)}");
                sb.Append($"Total: {totalCount.ToString(Config.General.CurrencyFormat)}");

                logger.Info(sb.ToString());
                PushUtility.NotifyMessage(sb.ToString());
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
