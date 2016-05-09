using System;
using System.ServiceModel;
using System.Threading;
using MTree.DataStructure;
using MTree.Consumer;
using MTree.Utility;
using MTree.RealTimeProvider;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.Concurrent;
using System.Collections.Generic;
using MTree.Configuration;
using System.IO;
using System.Text;
using MongoDB.Bson;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;
using System.Diagnostics;
using System.ComponentModel;

namespace MTree.Dashboard
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false, ValidateMustUnderstand = false)]
    public class Dashboard : ConsumerBase, INotifyPropertyChanged
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public ObservableConcurrentDictionary<string, DashboardItem> StockItems { get; set; } = new ObservableConcurrentDictionary<string, DashboardItem>();
        public ObservableConcurrentDictionary<string, DashboardItem> IndexItems { get; set; } = new ObservableConcurrentDictionary<string, DashboardItem>();

        private ConcurrentDictionary<string, ObjectId> VerifyOrderingList { get; set; } = new ConcurrentDictionary<string, ObjectId>();

        public DataCounter Counter { get; set; } = null;
        public TrafficMonitor Monitor { get; set; } = null;

        public Dashboard()
        {
            try
            {
                TaskUtility.Run("Dashboard.CircuitBreakQueue", QueueTaskCancelToken, ProcessCircuitBreakQueue);

                if (Config.General.VerifyOrdering == true)
                {
                    TaskUtility.Run($"Dashboard.StockConclusionQueue", QueueTaskCancelToken, ProcessStockConclusionQueue);
                }
                else
                {
                    for (int i = 0; i < 5; i++)
                        TaskUtility.Run($"Dashboard.StockConclusionQueue_{i + 1}", QueueTaskCancelToken, ProcessStockConclusionQueue);
                }

                for (int i = 0; i < 2; i++)
                    TaskUtility.Run($"Dashboard.IndexConclusionQueue_{i + 1}", QueueTaskCancelToken, ProcessIndexConclusionQueue);

                if (Config.General.VerifyLatency == true)
                {
                    Monitor = new TrafficMonitor();

                    if (Config.General.SkipBiddingPrice == false)
                    {
                        for (int i = 0; i < 10; i++)
                            TaskUtility.Run($"Dashboard.BiddingPriceQueue_{i + 1}", QueueTaskCancelToken, ProcessBiddingPriceQueue);
                    }
                }
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
                ServiceClient.RegisterContract(ClientId, new SubscribeContract(SubscribeTypes.Mastering));
                ServiceClient.RegisterContract(ClientId, new SubscribeContract(SubscribeTypes.CircuitBreak));
                ServiceClient.RegisterContract(ClientId, new SubscribeContract(SubscribeTypes.StockConclusion));
                ServiceClient.RegisterContract(ClientId, new SubscribeContract(SubscribeTypes.IndexConclusion));

                if (Config.General.VerifyLatency == true && Config.General.SkipBiddingPrice == false)
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
                    if (Config.General.VerifyLatency == true)
                        Monitor.CheckLatency(biddingPrice);
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
                    if (Config.General.VerifyLatency == true)
                        Monitor.CheckLatency(circuitBreak);

                    if (StockItems.ContainsKey(circuitBreak.Code) == true)
                        StockItems[circuitBreak.Code].CircuitBreakType = circuitBreak.CircuitBreakType;
                    else
                        logger.Warn($"Circuit break code not in mastering data: {circuitBreak.Code}");
                }
                else
                {
                    Thread.Sleep(10);
                }
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
                    if (Config.General.VerifyLatency == true)
                        Monitor.CheckLatency(conclusion);

                    if (StockItems.ContainsKey(conclusion.Code) == false)
                    {
                        StockItems.Add(conclusion.Code, new DashboardItem(conclusion.Code));
                        logger.Warn($"Stock code not in mastering data: {conclusion.Code}");
                    }

                    var item = StockItems[conclusion.Code];
                    lock (item)
                    {
                        item.Price = conclusion.Price;
                        item.Volume += conclusion.Amount;
                    }

                    if (Config.General.VerifyOrdering == true)
                    {
                        var code = conclusion.Code;
                        var newId = conclusion.Id;

                        if (VerifyOrderingList.ContainsKey(code) == false)
                        {
                            VerifyOrderingList.TryAdd(code, newId);
                        }
                        else
                        {
                            var prevId = VerifyOrderingList[code];
                            if (prevId >= newId)
                                logger.Error($"Conclusion ordering fail, code: {code}, prevId: {prevId}, newId: {newId}");

                            prevId = newId;
                        }
                    }
                }
                else
                {
                    Thread.Sleep(10);
                }
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
                    if (Config.General.VerifyLatency == true)
                        Monitor.CheckLatency(conclusion);

                    if (IndexItems.ContainsKey(conclusion.Code) == false)
                    {
                        IndexItems.Add(conclusion.Code, new DashboardItem(conclusion.Code));
                        logger.Warn($"Index code not in mastering data: {conclusion.Code}");
                    }

                    var item = IndexItems[conclusion.Code];
                    lock (item)
                    {
                        item.Price = conclusion.Price;
                        item.Volume += conclusion.Amount;
                    }
                }
                else
                {
                    Thread.Sleep(10);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public override void ConsumeStockMaster(List<StockMaster> stockMasters)
        {
            try
            {
                foreach (var stockMaster in stockMasters)
                {
                    if (StockItems.ContainsKey(stockMaster.Code) == false)
                    {
                        var item = new DashboardItem(stockMaster.Code);
                        item.Name = stockMaster.Name;
                        item.Price = stockMaster.BasisPrice;
                        item.BasisPrice = stockMaster.BasisPrice;
                        item.PreviousVolume = stockMaster.PreviousVolume;
                        item.MarketType = stockMaster.MarketType;

                        StockItems.Add(item.Code, item);
                    }
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
                    if (IndexItems.ContainsKey(indexMaster.Code) == false)
                    {
                        var item = new DashboardItem(indexMaster.Code);
                        item.Name = indexMaster.Name;
                        item.Price = indexMaster.BasisPrice;
                        item.BasisPrice = indexMaster.BasisPrice;
                        item.MarketType = MarketTypes.INDEX;

                        IndexItems.Add(item.Code, item);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private void SaveDashboard()
        {
            try
            {
                var fileName = $"MTree.{Config.General.DateNow}_Dashboard.csv";
                var filePath = Path.Combine(Environment.CurrentDirectory, "Logs", Config.General.DateNow, fileName);

                using (var sw = new StreamWriter(new FileStream(filePath, FileMode.Create), Encoding.Default))
                {
                    sw.WriteLine("Code, Name, Price, PricePercent, BasisPrice, Volume, PreviousVolume, MarketType");

                    foreach (var item in StockItems.Values)
                    {
                        sw.WriteLine($"{item.Code}, {item.Name.Replace(',', ' ')}, {item.Price}, {item.PricePercent.ToString(Config.General.PercentFormat)}, {item.BasisPrice}, {item.Volume}, {item.PreviousVolume}, {item.MarketType.ToString()}");
                    }

                    foreach (var item in IndexItems.Values)
                    {
                        sw.WriteLine($"{item.Code}, {item.Name.Replace(',', ' ')}, {item.Price}, {item.PricePercent.ToString(Config.General.PercentFormat)}, {item.BasisPrice}, {item.Volume}, {item.PreviousVolume}, {item.MarketType.ToString()}");
                    }
                }

                logger.Info($"Save Dashboard done, {fileName}");
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
                    Task.Run(() =>
                    {
                        if (message.Equals(ExitProgramTypes.Normal.ToString()) == true)
                            SaveDashboard();

                        if (message.Equals(ExitProgramTypes.Normal.ToString()) == false ||
                            Config.General.KeepAliveDashboardAfterMarketEnd == false)
                        {
                            logger.Info("Process will be closed");
                            Thread.Sleep(1000 * 5);

                            Environment.Exit(0);
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            base.NotifyMessage(type, message);
        }

        #region Command
        private RelayCommand<DashboardItem> _DoubleClickCommand;
        public ICommand DoubleClickCommand
        {
            get
            {
                if (_DoubleClickCommand == null)
                    _DoubleClickCommand = new RelayCommand<DashboardItem>((param) => ExecuteDoubleClick(param));

                return _DoubleClickCommand;
            }
        }

        public void ExecuteDoubleClick(DashboardItem item)
        {
            Process.Start("https://search.naver.com/search.naver?where=nexearch&query=" + item.Name);
        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}
