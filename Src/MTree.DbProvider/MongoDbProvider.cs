using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MTree.Configuration;
using MTree.DataStructure;
using System;

namespace MTree.DbProvider
{
    public enum DbTypes
    {
        Chart,
        BiddingPrice,
        StockMaster,
        IndexMaster,
        StockConclusion,
        IndexConclusion,
        CircuitBreak,
        Test,
    }

    public class MongoDbProvider
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly string connectionString = Config.Database.ConnectionString;
        private readonly string chartDbString = "MTree_Chart";
        private readonly string biddingPriceDbString = "MTree_BiddingPrice";
        private readonly string circuitBreakDbString = "MTree_CircuitBreak";
        private readonly string stockMasterDbString = "MTree_StockMaster";
        private readonly string indexMasterDbString = "MTree_IndexMaster";
        private readonly string stockConclusionDbString = "MTree_StockConclusion";
        private readonly string indexConclusionDbString = "MTree_IndexConclusion";
        private readonly string testDbString = "MTree_Test";

        private IMongoClient Client { get; set; }

        public MongoDbProvider()
        {
            Connect();
        }

        private void Connect()
        {
            try
            {
                Client = new MongoClient(connectionString);
                logger.Info("MongoDb Connected");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public IMongoDatabase GetDatabase(DbTypes type)
        {
            switch (type)
            {
                case DbTypes.Chart:             return Client.GetDatabase(chartDbString);
                case DbTypes.BiddingPrice:      return Client.GetDatabase(biddingPriceDbString);
                case DbTypes.CircuitBreak:      return Client.GetDatabase(circuitBreakDbString);
                case DbTypes.StockMaster:       return Client.GetDatabase(stockMasterDbString);
                case DbTypes.IndexMaster:       return Client.GetDatabase(indexMasterDbString);
                case DbTypes.StockConclusion:   return Client.GetDatabase(stockConclusionDbString);
                case DbTypes.IndexConclusion:   return Client.GetDatabase(indexConclusionDbString);
                default:                        return Client.GetDatabase(testDbString);
            }
        }
    }
}
