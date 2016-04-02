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

        private void ProcessBiddingPriceQueue()
        {
            try
            {
                BiddingPrice biddingPrice;
                if (BiddingPriceQueue.TryDequeue(out biddingPrice) == true)
                    DbAgent.Instance.Insert(biddingPrice);
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
                    DbAgent.Instance.Insert(circuitBreak);
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
                    DbAgent.Instance.Insert(conclusion);
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
                    DbAgent.Instance.Insert(conclusion);
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
            BiddingPriceCount++;
        }

        public override void ConsumeStockConclusion(StockConclusion conclusion)
        {
            StockConclusionQueue.Enqueue(conclusion);
            StockConclusionCount++;
        }

        public override void ConsumeIndexConclusion(IndexConclusion conclusion)
        {
            IndexConclusionQueue.Enqueue(conclusion);
            IndexConclusionCount++;
        }

        public override void ConsumeCircuitBreak(CircuitBreak circuitBreak)
        {
            CircuitBreakQueue.Enqueue(circuitBreak);
            CircuitBreakCount++;
        }

        public override void ConsumeStockMaster(List<StockMaster> stockMasters)
        {
            try
            {
                foreach (var stockMaster in stockMasters)
                {
                    DbAgent.Instance.Insert(stockMaster);
                    StockMasterCount++;
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
                    IndexMasterCount++;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public override void ConsumeChart(List<Candle> candles)
        {
            try
            {
                var code = candles[0].Code;
                DbAgent.Instance.ChartDb.DropCollection(code);

                var collection = DbAgent.Instance.ChartDb.GetCollection<Candle>(code);
                collection.InsertManyAsync(candles);
                ChartCount += candles.Count;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public override void NotifyMessage(MessageTypes type, string message)
        {
            logger.Info($"NotifyMessage, type: {type.ToString()}, message: {message}");

            try
            {
                if (type == MessageTypes.CloseClient)
                {
                    StopRefreshTimer();

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

                var fileName = $"MTree.{DateTime.Now.ToString(Config.General.DateFormat)}.HistorySaver.csv";
                var filePath = Path.Combine(Environment.CurrentDirectory, "Logs", fileName);

                using (var sw = new StreamWriter(new FileStream(filePath, FileMode.Create)))
                {
                    sw.WriteLine($"Chart, {ChartCount}");
                    sw.WriteLine($"CircuitBreak, {CircuitBreakCount}");
                    sw.WriteLine($"BiddingPrice, {BiddingPriceCount}");
                    sw.WriteLine($"StockMaster, {StockMasterCount}");
                    sw.WriteLine($"IndexMaster, {IndexMasterCount}");
                    sw.WriteLine($"StockConclusion, {StockConclusionCount}");
                    sw.WriteLine($"IndexConclusion, {IndexConclusionCount}");
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
