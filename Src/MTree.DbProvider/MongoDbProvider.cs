using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MTree.Configuration;
using MTree.DataStructure;
using System;

namespace MTree.DbProvider
{
    public class MongoDbProvider
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly string connectionString = Config.Default.MongoDbConnectionString;
        private readonly string masterDbString = "MTree.Master";
        private readonly string chartDbString = "MTree.Chart";
        private readonly string indexConclusionDbString = "MTree.StockConclusion";
        private readonly string stockConclusionDbString = "MTree.IndexConclusion";

        private IMongoClient client;
        private IMongoDatabase masterDb;
        private IMongoDatabase chartDb;
        private IMongoDatabase stockConclusionDb;
        private IMongoDatabase indexConclusionDb;

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
                //RegisterDbClass<Conclusion>(); // TODO : DataStructure에 있는 것들 모두 등록해야하나?

                client = new MongoClient(connectionString);
                masterDb = client.GetDatabase(masterDbString);
                chartDb = client.GetDatabase(chartDbString);
                stockConclusionDb = client.GetDatabase(stockConclusionDbString);
                indexConclusionDb = client.GetDatabase(indexConclusionDbString);

                logger.Info("MongoDb Connected");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
    }
}
