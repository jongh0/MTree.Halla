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

        private IMongoClient _client0;
        private IMongoClient _client1;
        private IMongoClient _client2;
        private IMongoClient _client3;

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

                _client0 = new MongoClient(connectionString);
                _client1 = new MongoClient(connectionString);
                _client2 = new MongoClient(connectionString);
                _client3 = new MongoClient(connectionString);

#if true // Write operations that use this write concern will return as soon as the message is written to the socket.
                _client0.Settings.WriteConcern = WriteConcern.Unacknowledged;
                _client1.Settings.WriteConcern = WriteConcern.Unacknowledged;
                _client2.Settings.WriteConcern = WriteConcern.Unacknowledged;
                _client3.Settings.WriteConcern = WriteConcern.Unacknowledged;
#endif

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
                var list = _client0.ListDatabases().ToList();
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
                case DbTypes.Chart:             return _client0.GetDatabase(ChartDbString);
                case DbTypes.BiddingPrice:      return _client1.GetDatabase(BiddingPriceDbString);
                case DbTypes.CircuitBreak:      return _client0.GetDatabase(CircuitBreakDbString);
                case DbTypes.StockMaster:       return _client0.GetDatabase(StockMasterDbString);
                case DbTypes.IndexMaster:       return _client0.GetDatabase(IndexMasterDbString);
                case DbTypes.StockConclusion:   return _client2.GetDatabase(StockConclusionDbString);
                case DbTypes.IndexConclusion:   return _client3.GetDatabase(IndexConclusionDbString);
                case DbTypes.ETFConclusion:     return _client3.GetDatabase(ETFConclusionDbString);
                case DbTypes.TradeConclusion:   return _client0.GetDatabase(TradeConclusionDbString);
                case DbTypes.Common:            return _client0.GetDatabase(CommonDbString);
                case DbTypes.Test:              return _client0.GetDatabase(TestDbString);
                default:                        return null;
            }
        }
    }
}
