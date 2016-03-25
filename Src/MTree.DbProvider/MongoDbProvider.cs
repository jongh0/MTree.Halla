using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MTree.Configuration;
using MTree.DataStructure;
using System;

namespace MTree.DbProvider
{
    public enum DbTypes
    {
        Candle,
        BiddingPrice,
        StockMaster,
        StockConclusion,
        IndexConclusion,
        CircuitBreak,
        Test,
    }

    public class MongoDbProvider
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly string connectionString = Config.Instance.Database.ConnectionString;
        private readonly string candleDbString = "MTree_Candle";
        private readonly string biddingPriceDbString = "MTree_BiddingPrice";
        private readonly string stockMasterDbString = "MTree_StockMaster";
        private readonly string stockConclusionDbString = "MTree_StockConclusion";
        private readonly string indexConclusionDbString = "MTree_IndexConclusion";
        private readonly string circuitBreakDbString = "MTree_CircuitBreak";
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
                case DbTypes.Candle:            return Client.GetDatabase(candleDbString);
                case DbTypes.BiddingPrice:      return Client.GetDatabase(biddingPriceDbString);
                case DbTypes.StockMaster:       return Client.GetDatabase(stockMasterDbString);
                case DbTypes.StockConclusion:   return Client.GetDatabase(stockConclusionDbString);
                case DbTypes.IndexConclusion:   return Client.GetDatabase(indexConclusionDbString);
                case DbTypes.CircuitBreak:      return Client.GetDatabase(circuitBreakDbString);
                default:                        return Client.GetDatabase(testDbString);
            }
        }
    }
}
