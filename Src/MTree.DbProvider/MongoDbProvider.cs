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

        private readonly string connectionString = Config.Default.MongoDbConnectionString;
        private readonly string chartDbString = "MTree_Chart";
        private readonly string biddingPriceDbString = "MTree_BiddingPrice";
        private readonly string stockMasterDbString = "MTree_StockMaster";
        private readonly string stockConclusionDbString = "MTree_StockConclusion";
        private readonly string indexConclusionDbString = "MTree_IndexConclusion";
        private readonly string testDbString = "MTree_Test";

        private IMongoClient client;
        private IMongoDatabase chartDb;
        private IMongoDatabase biddingPriceDb;
        private IMongoDatabase stockMasterDb;
        private IMongoDatabase stockConclusionDb;
        private IMongoDatabase indexConclusionDb;
        private IMongoDatabase testDb;

        private static object lockObject = new object();

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
                client = new MongoClient(connectionString);
                chartDb = client.GetDatabase(chartDbString);
                biddingPriceDb = client.GetDatabase(biddingPriceDbString);
                stockMasterDb = client.GetDatabase(stockMasterDbString);
                stockConclusionDb = client.GetDatabase(stockConclusionDbString);
                indexConclusionDb = client.GetDatabase(indexConclusionDbString);
                testDb = client.GetDatabase(testDbString);

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
                    return chartDb;
                case DbType.BiddingPrice:
                    return biddingPriceDb;
                case DbType.StockMaster:
                    return stockMasterDb;
                case DbType.StockConclusion:
                    return stockConclusionDb;
                case DbType.IndexConclusion:
                    return indexConclusionDb;
                default:
                    return testDb;
            }
        }
    }
}
