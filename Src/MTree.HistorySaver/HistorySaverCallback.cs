using System;
using System.ServiceModel;
using MongoDB.Driver;
using MTree.DbProvider;
using MongoDB.Bson;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;
using MTree.DataStructure;
using MTree.RealTimeProvider;
using MTree.Configuration;

namespace MTree.HistorySaver
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class HistorySaverCallback : IRealTimeProviderCallback
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public CancellationTokenSource cancelSource = new CancellationTokenSource();
        private CancellationToken cancelToken;

        private IMongoCollection<BiddingPrice> biddingPriceCollection;
        private IMongoCollection<StockConclusion> stockConclusionCollection;
        private IMongoCollection<IndexConclusion> indexConclusionCollection;

        private ConcurrentQueue<BiddingPrice> biddingPriceQueue = new ConcurrentQueue<BiddingPrice>();
        private ConcurrentQueue<StockConclusion> stockConclusionQueue = new ConcurrentQueue<StockConclusion>();
        private ConcurrentQueue<IndexConclusion> indexConclusionQueue = new ConcurrentQueue<IndexConclusion>();

        public HistorySaverCallback()
        {
            cancelToken = cancelSource.Token;

            MongoDbProvider.Instance.Connect();
            biddingPriceCollection = MongoDbProvider.Instance.GetDatabase(DbType.BiddingPrice).GetCollection<BiddingPrice>(Config.Default.MongoDbDateCollectionName);
            stockConclusionCollection = MongoDbProvider.Instance.GetDatabase(DbType.StockConclusion).GetCollection<StockConclusion>(Config.Default.MongoDbDateCollectionName);
            indexConclusionCollection = MongoDbProvider.Instance.GetDatabase(DbType.IndexConclusion).GetCollection<IndexConclusion>(Config.Default.MongoDbDateCollectionName);

            StartBiddingPriceQueueTask();
            StartStockConclusionQueueTask();
            StartIndexConclusionQueueTask();
        }

        private void StartBiddingPriceQueueTask()
        {
            Task.Run(() =>
            {
                logger.Info("biddingPriceQueue task started");

                while (true)
                {
                    try
                    {
                        cancelToken.ThrowIfCancellationRequested();

                        BiddingPrice biddingPrice;
                        if (biddingPriceQueue.TryDequeue(out biddingPrice) == true)
                            biddingPriceCollection.InsertOne(biddingPrice);
                        else
                            Thread.Sleep(10);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                }

                logger.Info("biddingPriceQueue task stopped");
            });
        }

        private void StartStockConclusionQueueTask()
        {
            Task.Run(() =>
            {
                logger.Info("stockConclusionQueue task started");

                while (true)
                {
                    try
                    {
                        cancelToken.ThrowIfCancellationRequested();

                        StockConclusion conclusion;
                        if (stockConclusionQueue.TryDequeue(out conclusion) == true)
                            stockConclusionCollection.InsertOne(conclusion);
                        else
                            Thread.Sleep(10);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                }

                logger.Info("stockConclusionQueue task stopped");
            });
        }

        private void StartIndexConclusionQueueTask()
        {
            Task.Run(() =>
            {
                logger.Info("indexConclusionQueue task started");

                while (true)
                {
                    try
                    {
                        cancelToken.ThrowIfCancellationRequested();

                        IndexConclusion conclusion;
                        if (indexConclusionQueue.TryDequeue(out conclusion) == true)
                            indexConclusionCollection.InsertOne(conclusion);
                        else
                            Thread.Sleep(10);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                }

                logger.Info("indexConclusionQueue task stopped");
            });
        }

        public void BiddingPriceUpdated(BiddingPrice biddingPrice)
        {
            biddingPriceQueue.Enqueue(biddingPrice);
        }

        public void CircuitBreakUpdated(CircuitBreak circuitBreak)
        {
        }

        public void ConclusionUpdated(StockConclusion conclusion)
        {
            stockConclusionQueue.Enqueue(conclusion);
        }

        public void ConclusionUpdated(IndexConclusion conclusion)
        {
            indexConclusionQueue.Enqueue(conclusion);
        }
    }
}
