﻿using MongoDB.Driver;
using MTree.Configuration;
using MTree.DataStructure;
using MTree.DbProvider;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.DbProvider
{
    public class DbAgent
    {
        #region Static variable
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        private static object lockObject = new object();

        private static volatile DbAgent _intance;
        public static DbAgent Instance
        {
            get
            {
                if (_intance == null)
                {
                    lock (lockObject)
                    {
                        if (_intance == null)
                            _intance = new DbAgent();
                    }
                }

                return _intance;
            }
        }
        #endregion

        public MongoDbProvider DbProvider { get; set; } = new MongoDbProvider();

        public IMongoDatabase CandleDb { get; set; }
        public IMongoDatabase BiddingPriceDb { get; set; }
        public IMongoDatabase CircuitBreakDb { get; set; }
        public IMongoDatabase StockMasterDb { get; set; }
        public IMongoDatabase StockConclusionDb { get; set; }
        public IMongoDatabase IndexConclusionDb { get; set; }

        public DbAgent()
        {
            CandleDb = DbProvider.GetDatabase(DbTypes.Candle);
            BiddingPriceDb = DbProvider.GetDatabase(DbTypes.BiddingPrice);
            CircuitBreakDb = DbProvider.GetDatabase(DbTypes.CircuitBreak);
            StockMasterDb = DbProvider.GetDatabase(DbTypes.StockMaster);
            StockConclusionDb = DbProvider.GetDatabase(DbTypes.StockConclusion);
            IndexConclusionDb = DbProvider.GetDatabase(DbTypes.IndexConclusion);
        }

        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            try
            {
                if (typeof(T) == typeof(Candle))
                    return (IMongoCollection<T>)CandleDb.GetCollection<Candle>(collectionName);
                else if (typeof(T) == typeof(StockMaster))
                    return (IMongoCollection<T>)StockMasterDb.GetCollection<StockMaster>(collectionName);
                else if (typeof(T) == typeof(BiddingPrice))
                    return (IMongoCollection<T>)BiddingPriceDb.GetCollection<BiddingPrice>(collectionName);
                else if (typeof(T) == typeof(CircuitBreak))
                    return (IMongoCollection<T>)CircuitBreakDb.GetCollection<CircuitBreak>(collectionName);
                else if (typeof(T) == typeof(StockConclusion))
                    return (IMongoCollection<T>)StockConclusionDb.GetCollection<StockConclusion>(collectionName);
                else if (typeof(T) == typeof(IndexConclusion))
                    return (IMongoCollection<T>)IndexConclusionDb.GetCollection<IndexConclusion>(collectionName);
                else
                    logger.Error($"Can't find collection, type: {typeof(T)}");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return null;
        }

        public void Insert<T>(T item)
        {
            if (item == null) return;

            try
            {
                var subscribable = item as Subscribable;
                var collection = GetCollection<T>(subscribable.Code);

                if (collection != null)
                    collection.InsertOne(item);
                else
                    logger.Error($"Insert error, {subscribable.Code}/{subscribable.Time.ToString(Config.Instance.General.DateTimeFormat)}");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public void Insert<T>(T[] items)
        {
            if (items == null) return;

            foreach (var item in items)
                Insert(item);
        }

        public void Delete<T>(string collectionName, FilterDefinition<T> filter)
        {
            if (string.IsNullOrEmpty(collectionName) == true || filter == null) return;

            try
            {
                var collection = GetCollection<T>(collectionName);

                if (collection != null)
                    collection.DeleteMany(filter);
                else
                    logger.Error($"Delete error, {collectionName}");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public IFindFluent<T, T> Find<T>(string collectionName, FilterDefinition<T> filter)
        {
            if (string.IsNullOrEmpty(collectionName) == true || filter == null) return null;

            try
            {
                var collection = GetCollection<T>(collectionName);

                if (collection != null)
                    return GetCollection<T>(collectionName).Find(filter);
                else
                    logger.Error($"Find error, {collectionName}");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return null;
        }

        public void CreateIndex(bool recreate = false)
        {
            try
            {
                using (var cursor = CandleDb.ListCollections())
                {
                    foreach (var doc in cursor.ToList())
                    {
                        var collectionName = doc.GetElement("name").Value.ToString();
                        var collection = GetCollection<Candle>(collectionName);
                        var keys = Builders<Candle>.IndexKeys.Ascending(i => i.CandleType).Ascending(i => i.Time);

                        if (recreate == true)
                            collection.Indexes.DropAll();

                        collection.Indexes.CreateOne(keys);
                    }
                }

                using (var cursor = BiddingPriceDb.ListCollections())
                {
                    foreach (var doc in cursor.ToList())
                    {
                        var collectionName = doc.GetElement("name").Value.ToString();
                        var collection = GetCollection<BiddingPrice>(collectionName);
                        var keys = Builders<BiddingPrice>.IndexKeys.Ascending(i => i.Time);

                        if (recreate == true)
                            collection.Indexes.DropAll();

                        collection.Indexes.CreateOne(keys);
                    }
                }

                using (var cursor = CircuitBreakDb.ListCollections())
                {
                    foreach (var doc in cursor.ToList())
                    {
                        var collectionName = doc.GetElement("name").Value.ToString();
                        var collection = GetCollection<CircuitBreak>(collectionName);
                        var keys = Builders<CircuitBreak>.IndexKeys.Ascending(i => i.Time);

                        if (recreate == true)
                            collection.Indexes.DropAll();

                        collection.Indexes.CreateOne(keys);
                    }
                }

                using (var cursor = StockMasterDb.ListCollections())
                {
                    foreach (var doc in cursor.ToList())
                    {
                        var collectionName = doc.GetElement("name").Value.ToString();
                        var collection = GetCollection<StockMaster>(collectionName);
                        var keys = Builders<StockMaster>.IndexKeys.Ascending(i => i.Time);

                        if (recreate == true)
                            collection.Indexes.DropAll();

                        collection.Indexes.CreateOne(keys);
                    }
                }

                using (var cursor = StockConclusionDb.ListCollections())
                {
                    foreach (var doc in cursor.ToList())
                    {
                        var collectionName = doc.GetElement("name").Value.ToString();
                        var collection = GetCollection<StockConclusion>(collectionName);
                        var keys = Builders<StockConclusion>.IndexKeys.Ascending(i => i.Time);

                        if (recreate == true)
                            collection.Indexes.DropAll();

                        collection.Indexes.CreateOne(keys);
                    }
                }

                using (var cursor = IndexConclusionDb.ListCollections())
                {
                    foreach (var doc in cursor.ToList())
                    {
                        var collectionName = doc.GetElement("name").Value.ToString();
                        var collection = GetCollection<IndexConclusion>(collectionName);
                        var keys = Builders<IndexConclusion>.IndexKeys.Ascending(i => i.Time);

                        if (recreate == true)
                            collection.Indexes.DropAll();

                        collection.Indexes.CreateOne(keys);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
    }
}
