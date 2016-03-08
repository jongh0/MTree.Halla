using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MTree.Configuration;
using MTree.DataStructure;
using System;

namespace MTree.DbProvider
{
    public enum DbType
    {
        Chart,
        BiddingPrice,
        StockMaster,
        StockConclusion,
        IndexConclusion,
        Test,
    }

    public class MongoDbProvider
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private static object lockObject = new object();

        private readonly string connectionString = Config.Database.ConnectionString;
        private readonly string chartDbString = "MTree_Chart";
        private readonly string biddingPriceDbString = "MTree_BiddingPrice";
        private readonly string stockMasterDbString = "MTree_StockMaster";
        private readonly string stockConclusionDbString = "MTree_StockConclusion";
        private readonly string indexConclusionDbString = "MTree_IndexConclusion";
        private readonly string testDbString = "MTree_Test";

        private IMongoClient Client { get; set; }
        private IMongoDatabase ChartDb { get; set; }
        private IMongoDatabase BiddingPriceDb { get; set; }
        private IMongoDatabase StockMasterDb { get; set; }
        private IMongoDatabase StockConclusionDb { get; set; }
        private IMongoDatabase IndexConclusionDb { get; set; }
        private IMongoDatabase TestDb { get; set; }

        private static volatile MongoDbProvider _intance;
        public static MongoDbProvider Instance
        {
            get
            {
                if (_intance == null)
                {
                    lock (lockObject)
                    {
                        if (_intance == null)
                            _intance = new MongoDbProvider();
                    }
                }

                return _intance;
            }
        }

        private void RegisterDbClass<T>()
        {
            try
            {
                if (BsonClassMap.IsClassMapRegistered(typeof(T)) == false)
                {
                    BsonClassMap.RegisterClassMap<T>();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Connect()
        {
            try
            {
                Client = new MongoClient(connectionString);
                ChartDb = Client.GetDatabase(chartDbString);
                BiddingPriceDb = Client.GetDatabase(biddingPriceDbString);
                StockMasterDb = Client.GetDatabase(stockMasterDbString);
                StockConclusionDb = Client.GetDatabase(stockConclusionDbString);
                IndexConclusionDb = Client.GetDatabase(indexConclusionDbString);
                TestDb = Client.GetDatabase(testDbString);

                logger.Info("MongoDb Connected");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public IMongoDatabase GetDatabase(DbType type)
        {
            switch (type)
            {
                case DbType.Chart:
                    return ChartDb;
                case DbType.BiddingPrice:
                    return BiddingPriceDb;
                case DbType.StockMaster:
                    return StockMasterDb;
                case DbType.StockConclusion:
                    return StockConclusionDb;
                case DbType.IndexConclusion:
                    return IndexConclusionDb;
                default:
                    return TestDb;
            }
        }
    }
}
