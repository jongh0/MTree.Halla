using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Configuration;
using DataStructure;
using CommonLib;
using System;
using System.Collections.Generic;
using CommonLib.Utility;

namespace DbProvider
{
    public class MongoDbProvider
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly string chartDbString = "MTree_Chart";
        private readonly string biddingPriceDbString = "MTree_BiddingPrice";
        private readonly string circuitBreakDbString = "MTree_CircuitBreak";
        private readonly string stockMasterDbString = "MTree_StockMaster";
        private readonly string indexMasterDbString = "MTree_IndexMaster";
        private readonly string stockConclusionDbString = "MTree_StockConclusion";
        private readonly string indexConclusionDbString = "MTree_IndexConclusion";
        private readonly string etfConclusionDbString = "MTree_ETFConclusion";
        private readonly string tradeConclusionDbString = "MTree_TradeConclusion";
        private readonly string commonDbString = "MTree_Common";
        private readonly string testDbString = "MTree_Test";

        private IMongoClient Client { get; set; }

        public MongoDbProvider(string connectionString)
        {
            Connect(connectionString);
        }

        private void Connect(string connectionString)
        {
            try
            {
                if (ServiceUtility.IsServiceRunning("MongoDb") == false)
                    ServiceUtility.StartService("MongoDb");

                Client = new MongoClient(connectionString);
                // Write operations that use this write concern will return as soon as the message is written to the socket.
                Client.Settings.WriteConcern = WriteConcern.Unacknowledged;

                _logger.Info($"MongoDb Connected to {connectionString}");
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        public List<BsonDocument> GetDatabaseList()
        {
            try
            {
                var list = Client.ListDatabases().ToList();
                return list;
            }
            catch (TimeoutException)
            {
                _logger.Warn($"MongoDb Server is not accessable");
            }
            return null;
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
                case DbTypes.ETFConclusion:     return Client.GetDatabase(etfConclusionDbString);
                case DbTypes.TradeConclusion:   return Client.GetDatabase(tradeConclusionDbString);
                case DbTypes.Common:            return Client.GetDatabase(commonDbString);
                case DbTypes.Test:              return Client.GetDatabase(testDbString);
                default:                        return null;
            }
        }
    }
}
