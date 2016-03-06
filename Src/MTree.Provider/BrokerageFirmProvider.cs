using MTree.DataStructure;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTree.Provider
{
    public class BrokerageFirmProvider : ClientProvider
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        protected LoginInfo loginInfo;
        protected AutoResetEvent waitQuoting;

        protected StockMaster quotingStockMaster;
        protected IndexMaster quotingIndexMaster;

        protected ConcurrentDictionary<string, IndexConclusion> prevIndexConclusions;

        protected ConcurrentQueue<BiddingPrice> biddingPriceQueue;
        protected ConcurrentQueue<StockConclusion> stockConclusionQueue;
        protected ConcurrentQueue<IndexConclusion> indexConclusionQueue;

        protected CancellationTokenSource queueTaskCancelSource = new CancellationTokenSource();
        private CancellationToken queueTaskCancelToken;

        protected string Server { get; set; }
        protected int Port { get; set; }

        public BrokerageFirmProvider() : base()
        {
            loginInfo = new LoginInfo();

            waitQuoting = new AutoResetEvent(false);

            prevIndexConclusions = new ConcurrentDictionary<string, IndexConclusion>();

            biddingPriceQueue = new ConcurrentQueue<BiddingPrice>();
            stockConclusionQueue = new ConcurrentQueue<StockConclusion>();
            indexConclusionQueue = new ConcurrentQueue<IndexConclusion>();

            queueTaskCancelToken = queueTaskCancelSource.Token;
        }

        protected void StartBiddingPriceQueueTask()
        {
            Task.Run(() =>
            {
                logger.Info("biddingPriceQueue task started");

                while (true)
                {
                    try
                    {
                        queueTaskCancelToken.ThrowIfCancellationRequested();

                        BiddingPrice biddingPrice;
                        if (client.State == CommunicationState.Opened && 
                            biddingPriceQueue.TryDequeue(out biddingPrice) == true)
                            client.NotifyBiddingPrice(biddingPrice);
                        else
                            Thread.Sleep(10);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                    }
                }

                logger.Info("biddingPriceQueue task stopped");
            });
        }

        protected void StartStockConclusionQueueTask()
        {
            Task.Run(() =>
            {
                logger.Info("stockConclusionQueue task started");

                while (true)
                {
                    try
                    {
                        queueTaskCancelToken.ThrowIfCancellationRequested();

                        StockConclusion conclusion;
                        if (client.State == CommunicationState.Opened && 
                            stockConclusionQueue.TryDequeue(out conclusion) == true)
                            client.NotifyConclusion(conclusion);
                        else
                            Thread.Sleep(10);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                    }
                }

                logger.Info("stockConclusionQueue task stopped");
            });
        }

        protected void StartIndexConclusionQueueTask()
        {
            Task.Run(() =>
            {
                logger.Info("indexConclusionQueue task started");

                while (true)
                {
                    try
                    {
                        queueTaskCancelToken.ThrowIfCancellationRequested();

                        IndexConclusion conclusion;
                        if (client.State == CommunicationState.Opened && 
                            indexConclusionQueue.TryDequeue(out conclusion) == true)
                            client.NotifyConclusion(conclusion);
                        else
                            Thread.Sleep(10);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                    }
                }

                logger.Info("indexConclusionQueue task stopped");
            });
        }
    }
}
