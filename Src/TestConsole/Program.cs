﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTree.Configuration;
using System.Threading;
using MTree.DbProvider;
using MTree.DataStructure;
using MTree.Utility;
using MongoDB.Driver;

namespace TestConsole
{
    class Program
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            Config.Initialize();

            TestDbAgent();
            //TestPushService();
            //TestEmail();

            Console.ReadLine();
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
            //EmailUtility.SendEmail("vvvv", "vvvv", @"D:\Documents\GitHub\MTree.Halla\Src\bin\Debug\Logs\MTree.All.2016-03-23.zip");
            EmailUtility.SendEmail("vvv", "vvvvv");
        }

        private static void TestPushService()
        {
            PushUtility.NotifyMessage("Hello MTree");
        }
    }
}
