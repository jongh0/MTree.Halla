#define VERIFY_ORDERING

using System;
using System.ServiceModel;
using MTree.DbProvider;
using System.Threading;
using MTree.DataStructure;
using MTree.Consumer;
using MTree.Utility;
using MTree.RealTimeProvider;
using System.Threading.Tasks;
using System.Windows;
using System.ComponentModel;
using MTree.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Collections.Concurrent;
using MongoDB.Bson;

namespace MTree.HistorySaver
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class HistorySaver : ConsumerBase
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public DataCounter Counter { get; set; } = new DataCounter(DataTypes.HistorySaver);

        private System.Timers.Timer RefreshTimer { get; set; }

#if VERIFY_ORDERING
        private ConcurrentDictionary<string, ObjectId> VerifyList { get; set; } = new ConcurrentDictionary<string, ObjectId>();
#endif

        public HistorySaver()
        {
            try
            {
                TaskUtility.Run("HistorySaver.CircuitBreakQueue", QueueTaskCancelToken, ProcessCircuitBreakQueue);

                for (int i = 0; i < 5; i++)
                    TaskUtility.Run($"HistorySaver.StockConclusionQueue_{i + 1}", QueueTaskCancelToken, ProcessStockConclusionQueue);

                for (int i = 0; i < 2; i++)
                    TaskUtility.Run($"HistorySaver.IndexConclusionQueue_{i + 1}", QueueTaskCancelToken, ProcessIndexConclusionQueue);

                if (Config.General.SkipBiddingPrice == false)
                {
                    for (int i = 0; i < 10; i++)
                        TaskUtility.Run($"HistorySaver.BiddingPriceQueue_{i + 1}", QueueTaskCancelToken, ProcessBiddingPriceQueue);
                }

                StartRefreshTimer();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        protected override void ServiceClient_Opened(object sender, EventArgs e)
        {
            base.ServiceClient_Opened(sender, e);

            try
            {
                ServiceClient.RegisterContract(ClientId, new SubscribeContract(SubscribeTypes.Chart));
                ServiceClient.RegisterContract(ClientId, new SubscribeContract(SubscribeTypes.Mastering));
                ServiceClient.RegisterContract(ClientId, new SubscribeContract(SubscribeTypes.CircuitBreak));
                ServiceClient.RegisterContract(ClientId, new SubscribeContract(SubscribeTypes.StockConclusion));
                ServiceClient.RegisterContract(ClientId, new SubscribeContract(SubscribeTypes.IndexConclusion));
                if (Config.General.SkipBiddingPrice == false)
                    ServiceClient.RegisterContract(ClientId, new SubscribeContract(SubscribeTypes.BiddingPrice));
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private void ProcessBiddingPriceQueue()
        {
            try
            {
                BiddingPrice biddingPrice;
                if (BiddingPriceQueue.TryDequeue(out biddingPrice) == true)
                {
                    DbAgent.Instance.Insert(biddingPrice);
                    Counter.Increment(CounterTypes.BiddingPrice);
                }
                else
                    Thread.Sleep(10);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private void ProcessCircuitBreakQueue()
        {
            try
            {
                CircuitBreak circuitBreak;
                if (CircuitBreakQueue.TryDequeue(out circuitBreak) == true)
                {
                    DbAgent.Instance.Insert(circuitBreak);
                    Counter.Increment(CounterTypes.CircuitBreak);
                }
                else
                    Thread.Sleep(10);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private void ProcessStockConclusionQueue()
        {
            try
            {
                StockConclusion conclusion;
                if (StockConclusionQueue.TryDequeue(out conclusion) == true)
                {
                    DbAgent.Instance.Insert(conclusion);
                    Counter.Increment(CounterTypes.StockConclusion);
                }
                else
                    Thread.Sleep(10);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private void ProcessIndexConclusionQueue()
        {
            try
            {
                IndexConclusion conclusion;
                if (IndexConclusionQueue.TryDequeue(out conclusion) == true)
                {
                    DbAgent.Instance.Insert(conclusion);
                    Counter.Increment(CounterTypes.IndexConclusion);
                }
                else
                    Thread.Sleep(10);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public override void ConsumeBiddingPrice(BiddingPrice biddingPrice)
        {
            BiddingPriceQueue.Enqueue(biddingPrice);
        }

        public override void ConsumeStockConclusion(StockConclusion conclusion)
        {
            StockConclusionQueue.Enqueue(conclusion);

#if VERIFY_ORDERING
            try
            {
                var code = conclusion.Code;
                var newId = conclusion.Id;

                if (VerifyList.ContainsKey(code) == false)
                {
                    VerifyList.TryAdd(code, newId);
                }
                else
                {
                    var prevId = VerifyList[code];
                    if (prevId >= newId)
                        logger.Error($"Conclusion ordering fail, code: {code}, prevId: {prevId}, newId: {newId}");

                    prevId = newId;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
#endif
        }

        public override void ConsumeIndexConclusion(IndexConclusion conclusion)
        {
            IndexConclusionQueue.Enqueue(conclusion);
        }

        public override void ConsumeCircuitBreak(CircuitBreak circuitBreak)
        {
            CircuitBreakQueue.Enqueue(circuitBreak);
        }

        public override void ConsumeStockMaster(List<StockMaster> stockMasters)
        {
            try
            {
                foreach (var stockMaster in stockMasters)
                {
                    DbAgent.Instance.Insert(stockMaster);
                    Counter.Increment(CounterTypes.StockMaster);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public override void ConsumeIndexMaster(List<IndexMaster> indexMasters)
        {
            try
            {
                foreach (var indexMaster in indexMasters)
                {
                    DbAgent.Instance.Insert(indexMaster);
                    Counter.Increment(CounterTypes.IndexMaster);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        /// <summary>
        /// 기존에 저장된 Day Chart는 Collection Drop 후 새로 저장한다.
        /// </summary>
        /// <param name="candles"></param>
        public override void ConsumeChart(List<Candle> candles)
        {
            try
            {
                var code = candles[0].Code;
                DbAgent.Instance.ChartDb.DropCollection(code);

                var collection = DbAgent.Instance.ChartDb.GetCollection<Candle>(code);
                collection.InsertMany(candles);
                Counter.Add(CounterTypes.Chart, candles.Count);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public override void NotifyMessage(MessageTypes type, string message)
        {
            try
            {
                if (type == MessageTypes.CloseClient)
                {
                    // Count 업데이트 중지
                    StopRefreshTimer();

                    if (message.Equals(ExitProgramTypes.Normal.ToString()) == true)
                    {
                        // Queue Task가 모두 완료될 때 까지 대기
                        WaitQueueTask();
                    }

                    Task.Run(() =>
                    {
                        if (message.Equals(ExitProgramTypes.Normal.ToString()) == true)
                        {
                            Counter.SaveToFile();
                            DbAgent.Instance.Insert(Counter);
                            DbAgent.Instance.CreateIndex();
                            DbAgent.Instance.SaveStatisticLog();
                        }

                        logger.Info("Process will be closed");
                        Thread.Sleep(1000 * 5);

                        Environment.Exit(0);
                    });
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            base.NotifyMessage(type, message);
        }

        private void StartRefreshTimer()
        {
            if (RefreshTimer == null)
            {
                RefreshTimer = new System.Timers.Timer();
                RefreshTimer.AutoReset = true;
                RefreshTimer.Interval = 1000;
                RefreshTimer.Elapsed += RefreshTimer_Elapsed;
            }

            RefreshTimer.Start();
        }

        private void StopRefreshTimer()
        {
            RefreshTimer?.Stop();
        }

        private void RefreshTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Counter.NotifyPropertyAll();
        }
    }
}
