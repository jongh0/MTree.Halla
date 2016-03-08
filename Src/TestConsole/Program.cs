﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTree.Configuration;
using System.Threading;
using MTree.DbProvider;
using MTree.DataStructure;

namespace TestConsole
{
    class Program
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            Console.WriteLine(Config.Ebest.UserId);
            Console.WriteLine(Config.Daishin.UserId);
            Console.WriteLine(Config.Database.ConnectionString);
            //TestDbSaving();
        }

        private static void TestDbSaving()
        {
            try
            {
                MongoDbProvider.Instance.Connect();

                // stock master
                var stockMaster = new StockMaster();
                var stockMasterCollection = MongoDbProvider.Instance.GetDatabase(DbType.StockMaster).GetCollection<StockMaster>(Config.Database.TodayCollectionName);
                stockMasterCollection.InsertOne(stockMaster);

                // stock conclusion
                var stockConclusion = new StockConclusion();
                var stockConclusionCollection = MongoDbProvider.Instance.GetDatabase(DbType.StockConclusion).GetCollection<StockConclusion>(Config.Database.TodayCollectionName);
                stockConclusionCollection.InsertOne(stockConclusion);

                // index conclusion
                var indexConclusion = new IndexConclusion();
                var indexConclusionCollection = MongoDbProvider.Instance.GetDatabase(DbType.IndexConclusion).GetCollection<IndexConclusion>(Config.Database.TodayCollectionName);
                indexConclusionCollection.InsertOne(indexConclusion);

                // bidding price
                var biddingPrice = new BiddingPrice();
                var biddingPriceCollection = MongoDbProvider.Instance.GetDatabase(DbType.BiddingPrice).GetCollection<BiddingPrice>(Config.Database.TodayCollectionName);
                biddingPriceCollection.InsertOne(biddingPrice);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
    }
}
