using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Configuration;
using System.Threading;
using DbProvider;
using DataStructure;
using CommonLib;
using MongoDB.Driver;
using System.Diagnostics;
using MongoDB.Bson;
using System.Globalization;
using CommonLib.Utility;

namespace TestConsole
{
    class Program
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("en-US");

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Config.Initialize();

            //TestToString();
            //TestObjectId();
            //TestDataCounter();
            //TestLogging();
            //TestConfig();
            //TestCreateIndex();
            //TestDbAgent();
            //TestEmail();
            //TestDaishinInstanceLimit();
            //TestDbCollection();
            //TestDbInsert();

            Console.WriteLine("Press any key..");
            Console.ReadLine();
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            _logger.Error((Exception)e.ExceptionObject);
        }

        private static void TestDbInsert()
        {
            int startTick = Environment.TickCount;

            int count = 10000;

            var task1 = Task.Run(() =>
            {
                for (int i = 0; i < count; i++)
                {
                    var conclusion = new StockConclusion();
                    conclusion.Time = DateTime.Now;
                    conclusion.Code = "000020";
                    conclusion.Id = ObjectIdUtility.GenerateNewId(DateTime.Now);
                    conclusion.Amount = 100;
                    DbAgent.Instance.Insert(conclusion);
                    Thread.Sleep(1);
                }
            });

            var task2 = Task.Run(() =>
            {
                for (int i = 0; i < count; i++)
                {
                    var conclusion = new IndexConclusion();
                    conclusion.Time = DateTime.Now;
                    conclusion.Code = "000020";
                    conclusion.Id = ObjectIdUtility.GenerateNewId(DateTime.Now);
                    conclusion.Amount = 100;
                    DbAgent.Instance.Insert(conclusion);
                    Thread.Sleep(1);
                }
            });

            var task3 = Task.Run(() =>
            {
                for (int i = 0; i < count; i++)
                {
                    var bidding = new BiddingPrice();
                    bidding.Code = "000020";
                    bidding.Time = DateTime.Now;
                    bidding.Id = ObjectIdUtility.GenerateNewId(DateTime.Now);
                    bidding.Bids = new List<BiddingPriceEntity>();
                    bidding.Bids.Add(new BiddingPriceEntity());
                    bidding.Bids.Add(new BiddingPriceEntity());
                    bidding.Bids.Add(new BiddingPriceEntity());
                    bidding.Bids.Add(new BiddingPriceEntity());
                    bidding.Offers = new List<BiddingPriceEntity>();
                    bidding.Offers.Add(new BiddingPriceEntity());
                    bidding.Offers.Add(new BiddingPriceEntity());
                    bidding.Offers.Add(new BiddingPriceEntity());
                    bidding.Offers.Add(new BiddingPriceEntity());
                    DbAgent.Instance.Insert(bidding);
                    Thread.Sleep(1);
                }
            });

            Task.WaitAll(task1, task2, task3);

            Console.WriteLine($"Insert time: {Environment.TickCount - startTick}");
        }

        private static void TestToString()
        {
            var c = new StockConclusion();
            c.Id = ObjectIdUtility.GenerateNewId();
            c.Code = "000020";
            c.Amount = 200;
            c.ConclusionType = ConclusionTypes.Buy;
            c.Time = DateTime.Now;
            c.ReceivedTime = DateTime.Now;

            Console.WriteLine(c);
        }

        private static void TestObjectId()
        {
            var id = ObjectIdUtility.GenerateNewId();
            var id1 = ObjectIdUtility.GenerateNewId();
            var id2 = ObjectIdUtility.GenerateNewId();
            var id3 = ObjectIdUtility.GenerateNewId();

            Console.WriteLine(id);
            Console.WriteLine(id1);
            Console.WriteLine(id2);
            Console.WriteLine(id3);
        }

        private static void TestDataCounter()
        {
            var counter = new DataCounter(DataTypes.Database);
            counter.BiddingPriceCount = 10;
            counter.CircuitBreakCount = 30;
            DbAgent.Instance.Insert(counter);
        }

        private static void TestLogging()
        {
            _logger.Info("log test Info");
            _logger.Trace("log test Trace");
            _logger.Debug("log test Debug");
            _logger.Warn("log test Warn");
            _logger.Error("log test Error");
            _logger.Fatal("log test Fatal");
        }

        private static void TestConfig()
        {
            Console.WriteLine(Config.Database.ConnectionString);
        }

        private static void TestCreateIndex()
        {
            DbAgent.Instance.CreateIndex();
        }

        private static void TestDbCollection()
        {
            var collections = DbAgent.Instance.GetCollectionList(DbTypes.StockConclusion);
            Trace.WriteLine(string.Join(", ", collections));
        }

        private static void TestDbAgent()
        {
            var item = new StockMaster();
            item.Code = "000020b";
            item.Time = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            item.Asset = 10;

            int startTick = Environment.TickCount;
            item.Id = new MongoDB.Bson.ObjectId();
            DbAgent.Instance.Insert(item);
            Console.WriteLine($"db insert tick : {Environment.TickCount - startTick}");

            startTick = Environment.TickCount;
            item.Id = new MongoDB.Bson.ObjectId();
            DbAgent.Instance.Insert(item);
            Console.WriteLine($"db insert tick : {Environment.TickCount - startTick}");

            startTick = Environment.TickCount;
            item.Id = new MongoDB.Bson.ObjectId();
            item.Code = "000030b";
            DbAgent.Instance.Insert(item);
            Console.WriteLine($"db insert tick : {Environment.TickCount - startTick}");

            startTick = Environment.TickCount;
            item.Id = new MongoDB.Bson.ObjectId();
            DbAgent.Instance.Insert(item);
            Console.WriteLine($"db insert tick : {Environment.TickCount - startTick}");

            var filter = Builders<StockMaster>.Filter.Eq(i => i.Time, item.Time);
            DbAgent.Instance.Delete(item.Code, filter);

            var item2 = new Candle();
            item2.Code = "000020b";
            item2.Time = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            item2.Close = 100;

            startTick = Environment.TickCount;
            item2.Id = new MongoDB.Bson.ObjectId();
            DbAgent.Instance.Insert(item2);
            Console.WriteLine($"db insert tick : {Environment.TickCount - startTick}");

            startTick = Environment.TickCount;
            item2.Id = new MongoDB.Bson.ObjectId();
            DbAgent.Instance.Insert(item2);
            Console.WriteLine($"db insert tick : {Environment.TickCount - startTick}");
        }

        private static void TestEmail()
        {
            //EmailUtility.SendEmail("vvvv", "vvvv", @"D:\Documents\GitHub\Halla\Src\bin\Debug\Logs\All.2016-03-23.zip");
            EmailUtility.SendEmail("vvv", "vvvvv");
        }

        private static void TestDaishinInstanceLimit()
        {
            for (int i = 0; i < 41; i++)
                ProcessUtility.Start(ProcessTypes.DaishinPublisher, ProcessWindowStyle.Minimized);
        }
    }
}
