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

namespace MTree.HistorySaver
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class HistorySaver : ConsumerBase, INotifyPropertyChanged
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        #region Counter property
        public int ChartCount { get; set; } = 0;
        public int BiddingPriceCount { get; set; } = 0;
        public int CircuitBreakCount { get; set; } = 0;
        public int StockMasterCount { get; set; } = 0;
        public int IndexMasterCount { get; set; } = 0;
        public int StockConclusionCount { get; set; } = 0;
        public int IndexConclusionCount { get; set; } = 0;
        public int TotalCount { get { return ChartCount + StockMasterCount + IndexMasterCount + BiddingPriceCount + CircuitBreakCount + StockConclusionCount + IndexConclusionCount; } }
        #endregion

        private System.Timers.Timer RefreshTimer { get; set; }

        public HistorySaver()
        {
            try
            {
                TaskUtility.Run("HistorySaver.BiddingPriceQueue", QueueTaskCancelToken, ProcessBiddingPriceQueue);
                TaskUtility.Run("HistorySaver.CircuitBreakQueue", QueueTaskCancelToken, ProcessCircuitBreakQueue);
                TaskUtility.Run("HistorySaver.StockConclusionQueue", QueueTaskCancelToken, ProcessStockConclusionQueue);
                TaskUtility.Run("HistorySaver.IndexConclusionQueue", QueueTaskCancelToken, ProcessIndexConclusionQueue);

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
                ServiceClient.RegisterContract(ClientId, new SubscribeContract(SubscribeTypes.BiddingPrice));
                ServiceClient.RegisterContract(ClientId, new SubscribeContract(SubscribeTypes.StockConclusion));
                ServiceClient.RegisterContract(ClientId, new SubscribeContract(SubscribeTypes.IndexConclusion));
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private async void ProcessBiddingPriceQueue()
        {
            try
            {
                BiddingPrice biddingPrice;
                if (BiddingPriceQueue.TryDequeue(out biddingPrice) == true)
                {
                    var collection = DbAgent.Instance.BiddingPriceDb.GetCollection<BiddingPrice>(biddingPrice.Code);
                    await collection.InsertOneAsync(biddingPrice);
                    BiddingPriceCount++;
                }
                else
                    Thread.Sleep(10);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private async void ProcessCircuitBreakQueue()
        {
            try
            {
                CircuitBreak circuitBreak;
                if (CircuitBreakQueue.TryDequeue(out circuitBreak) == true)
                {
                    var collection = DbAgent.Instance.CircuitBreakDb.GetCollection<CircuitBreak>(circuitBreak.Code);
                    await collection.InsertOneAsync(circuitBreak);
                    CircuitBreakCount++;
                }
                else
                    Thread.Sleep(10);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private async void ProcessStockConclusionQueue()
        {
            try
            {
                StockConclusion conclusion;
                if (StockConclusionQueue.TryDequeue(out conclusion) == true)
                {
                    var collection = DbAgent.Instance.StockConclusionDb.GetCollection<StockConclusion>(conclusion.Code);
                    await collection.InsertOneAsync(conclusion);
                    StockConclusionCount++;
                }
                else
                    Thread.Sleep(10);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private async void ProcessIndexConclusionQueue()
        {
            try
            {
                IndexConclusion conclusion;
                if (IndexConclusionQueue.TryDequeue(out conclusion) == true)
                {
                    var collection = DbAgent.Instance.IndexConclusionDb.GetCollection<IndexConclusion>(conclusion.Code);
                    await collection.InsertOneAsync(conclusion);
                    IndexConclusionCount++;
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
        }

        public override void ConsumeIndexConclusion(IndexConclusion conclusion)
        {
            IndexConclusionQueue.Enqueue(conclusion);
        }

        public override void ConsumeCircuitBreak(CircuitBreak circuitBreak)
        {
            CircuitBreakQueue.Enqueue(circuitBreak);
        }

        public override async void ConsumeStockMaster(List<StockMaster> stockMasters)
        {
            try
            {
                foreach (var stockMaster in stockMasters)
                {
                    var collection = DbAgent.Instance.StockMasterDb.GetCollection<StockMaster>(stockMaster.Code);
                    await collection.InsertOneAsync(stockMaster);
                    StockMasterCount++;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public override async void ConsumeIndexMaster(List<IndexMaster> indexMasters)
        {
            try
            {
                foreach (var indexMaster in indexMasters)
                {
                    var collection = DbAgent.Instance.IndexMasterDb.GetCollection<IndexMaster>(indexMaster.Code);
                    await collection.InsertOneAsync(indexMaster);
                    IndexMasterCount++;
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
        public override async void ConsumeChart(List<Candle> candles)
        {
            try
            {
                var code = candles[0].Code;
                DbAgent.Instance.ChartDb.DropCollection(code);

                var collection = DbAgent.Instance.ChartDb.GetCollection<Candle>(code);
                await collection.InsertManyAsync(candles);
                ChartCount += candles.Count;
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
                            SaveHistorySaver();
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
            NotifyPropertyChanged(nameof(ChartCount));
            NotifyPropertyChanged(nameof(BiddingPriceCount));
            NotifyPropertyChanged(nameof(CircuitBreakCount));
            NotifyPropertyChanged(nameof(StockMasterCount));
            NotifyPropertyChanged(nameof(IndexMasterCount));
            NotifyPropertyChanged(nameof(StockConclusionCount));
            NotifyPropertyChanged(nameof(IndexConclusionCount));
            NotifyPropertyChanged(nameof(TotalCount));
        }

        private void SaveHistorySaver()
        {
            try
            {
                logger.Info("Save HistorySaver");

                var fileName = $"MTree.{DateTime.Now.ToString(Config.General.DateFormat)}_HistorySaver.csv";
                var filePath = Path.Combine(Environment.CurrentDirectory, "Logs", fileName);

                using (var sw = new StreamWriter(new FileStream(filePath, FileMode.Create), Encoding.Default))
                {
                    sw.WriteLine($"Chart, {ChartCount}");
                    sw.WriteLine($"CircuitBreak, {CircuitBreakCount}");
                    sw.WriteLine($"BiddingPrice, {BiddingPriceCount}");
                    sw.WriteLine($"StockMaster, {StockMasterCount}");
                    sw.WriteLine($"IndexMaster, {IndexMasterCount}");
                    sw.WriteLine($"StockConclusion, {StockConclusionCount}");
                    sw.WriteLine($"IndexConclusion, {IndexConclusionCount}");
                    sw.WriteLine($"Total, {TotalCount}");
                }

                logger.Info($"Save HistorySaver done, {filePath}");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}
