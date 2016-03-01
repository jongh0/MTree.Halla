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

namespace MTree.HistorySaver
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class HistorySaverCallback : IRealTimeProviderCallback
    {
        public int count = 0;

        private CancellationToken cancelToken;
        public CancellationTokenSource cancelSource = new CancellationTokenSource();

        IMongoCollection<StockConclusion> collection;
        private ConcurrentQueue<StockConclusion> stockConclusionQueue = new ConcurrentQueue<StockConclusion>();

        public HistorySaverCallback()
        {
            cancelToken = cancelSource.Token;

            MongoDbProvider.Instance.Connect();
            //collection = MongoDbProvider.Instance.GetCollection("WCF_Test");

            Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        cancelToken.ThrowIfCancellationRequested();

                        StockConclusion conclusion;
                        if (stockConclusionQueue.TryDequeue(out conclusion) == true)
                        {
                            conclusion.Id = new ObjectId();
                            collection.InsertOne(conclusion);
                        }
                        else
                        {
                            Thread.Sleep(10);
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                }
            });
        }

        public void ConsumeConclusion(StockConclusion conclusion)
        {
            if (collection == null)
                return;
        }
    }
}
