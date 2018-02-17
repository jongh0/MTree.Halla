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

        private const string ChartDbString = "MTree_Chart";
        private const string BiddingPriceDbString = "MTree_BiddingPrice";
        private const string CircuitBreakDbString = "MTree_CircuitBreak";
        private const string StockMasterDbString = "MTree_StockMaster";
        private const string IndexMasterDbString = "MTree_IndexMaster";
        private const string StockConclusionDbString = "MTree_StockConclusion";
        private const string IndexConclusionDbString = "MTree_IndexConclusion";
        private const string ETFConclusionDbString = "MTree_ETFConclusion";
        private const string TradeConclusionDbString = "MTree_TradeConclusion";
        private const string CommonDbString = "MTree_Common";
        private const string TestDbString = "MTree_Test";

        private IMongoClient _client;

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

                _client = new MongoClient(connectionString);
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
                return _client.ListDatabases()?.ToList() ?? null;
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
                case DbTypes.Chart:             return _client.GetDatabase(ChartDbString);
                case DbTypes.BiddingPrice:      return _client.GetDatabase(BiddingPriceDbString);
                case DbTypes.CircuitBreak:      return _client.GetDatabase(CircuitBreakDbString);
                case DbTypes.StockMaster:       return _client.GetDatabase(StockMasterDbString);
                case DbTypes.IndexMaster:       return _client.GetDatabase(IndexMasterDbString);
                case DbTypes.StockConclusion:   return _client.GetDatabase(StockConclusionDbString);
                case DbTypes.IndexConclusion:   return _client.GetDatabase(IndexConclusionDbString);
                case DbTypes.ETFConclusion:     return _client.GetDatabase(ETFConclusionDbString);
                case DbTypes.TradeConclusion:   return _client.GetDatabase(TradeConclusionDbString);
                case DbTypes.Common:            return _client.GetDatabase(CommonDbString);
                case DbTypes.Test:              return _client.GetDatabase(TestDbString);
                default:                        return null;
            }
        }
    }
}
