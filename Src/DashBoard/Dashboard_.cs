using System;
using System.ServiceModel;
using System.Threading;
using DataStructure;
using Consumer;
using CommonLib;
using RealTimeProvider;
using System.Threading.Tasks;
using System.Windows;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Configuration;
using System.IO;
using System.Text;
using MongoDB.Bson;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;
using System.Diagnostics;
using System.ComponentModel;
using CommonLib.Utility;

namespace Dashboard
{
    public class Dashboard_
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public ObservableConcurrentDictionary<string, DashboardItem> StockItems { get; set; } = new ObservableConcurrentDictionary<string, DashboardItem>();
        public ObservableConcurrentDictionary<string, DashboardItem> IndexItems { get; set; } = new ObservableConcurrentDictionary<string, DashboardItem>();

        private ConcurrentDictionary<string, ObjectId> VerifyOrderingList { get; set; } = new ConcurrentDictionary<string, ObjectId>();

        public DataCounter Counter { get; set; } = null;
        public TrafficMonitor Monitor { get; set; } = null;

        private IConsumer Consumer { get; set; }

        public Dashboard_(IConsumer consumer)
        {
            try
            {
                Consumer = consumer ?? throw new ArgumentNullException(nameof(consumer));

                Consumer.StockMasterConsumed += OnStockMasterConsumed;
                Consumer.IndexMasterConsumed += OnIndexMasterConsumed;
                Consumer.CodeMapConsumed += OnCodeMapConsumed;
                Consumer.MessageNotified += OnMessageNotified;
                Consumer.StockConclusionConsumed += OnStockConclusionConsumed;
                consumer.IndexConclusionConsumed += OnIndexConclusionConsumed;
                consumer.BiddingPriceConsumed += OnBiddingPriceConsumed;
                consumer.CircuitBreakConsumed += OnCircuitBreakConsumed;

                if (consumer is RealTimeConsumer realTimeConsumer)
                    realTimeConsumer.ChannelOpened += RealTimeConsumer_ChannelOpened;

                if (Config.General.VerifyLatency == true)
                    Monitor = new TrafficMonitor();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private void RealTimeConsumer_ChannelOpened(RealTimeConsumer consumer)
        {
            if (consumer == null) return;

            consumer.RegisterContract(new SubscribeContract(SubscribeTypes.Mastering));
            consumer.RegisterContract(new SubscribeContract(SubscribeTypes.CircuitBreak));
            consumer.RegisterContract(new SubscribeContract(SubscribeTypes.StockConclusion));
            consumer.RegisterContract(new SubscribeContract(SubscribeTypes.IndexConclusion));

            if (Config.General.SkipBiddingPrice == false && Config.General.VerifyLatency == true)
                consumer.RegisterContract(new SubscribeContract(SubscribeTypes.BiddingPrice));
        }

        private void OnCircuitBreakConsumed(CircuitBreak circuitBreak)
        {
            try
            {
                if (Config.General.VerifyLatency == true)
                    Monitor.CheckLatency(circuitBreak);

                if (StockItems.ContainsKey(circuitBreak.Code) == true)
                    StockItems[circuitBreak.Code].CircuitBreakType = circuitBreak.CircuitBreakType;
                else
                    _logger.Warn($"Circuit break code not in mastering data: {circuitBreak.Code}");
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private void OnBiddingPriceConsumed(BiddingPrice biddingPrice)
        {
            try
            {
                if (Config.General.VerifyLatency == true)
                    Monitor.CheckLatency(biddingPrice);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private void OnIndexConclusionConsumed(IndexConclusion conclusion)
        {
            try
            {
                if (Config.General.VerifyLatency == true)
                    Monitor.CheckLatency(conclusion);

                if (IndexItems.ContainsKey(conclusion.Code) == false)
                {
                    IndexItems.Add(conclusion.Code, new DashboardItem(conclusion.Code));
                    _logger.Warn($"Index code not in mastering data: {conclusion.Code}");
                }

                var item = IndexItems[conclusion.Code];
                lock (item)
                {
                    item.Time = conclusion.Time;
                    item.Price = conclusion.Price;
                    item.Volume += conclusion.Amount;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private void OnStockConclusionConsumed(StockConclusion conclusion)
        {
            try
            {
                if (Config.General.VerifyLatency == true)
                    Monitor.CheckLatency(conclusion);

                if (StockItems.ContainsKey(conclusion.Code) == false)
                {
                    StockItems.Add(conclusion.Code, new DashboardItem(conclusion.Code));
                    _logger.Warn($"Stock code not in mastering data: {conclusion.Code}");
                }

                var item = StockItems[conclusion.Code];
                lock (item)
                {
                    item.Time = conclusion.Time;
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
                            _logger.Error($"Conclusion ordering fail, code: {code}, prevId: {prevId}, newId: {newId}");

                        prevId = newId;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private void OnStockMasterConsumed(List<StockMaster> stockMasters)
        {
            try
            {
                foreach (var stockMaster in stockMasters)
                {
                    if (StockItems.ContainsKey(stockMaster.Code) == false)
                    {
                        var item = new DashboardItem(stockMaster.Code);
                        StockItems.Add(item.Code, item);
                    }
                    StockItems[stockMaster.Code].Time = stockMaster.Time;
                    StockItems[stockMaster.Code].Name = stockMaster.Name;
                    StockItems[stockMaster.Code].Price = stockMaster.BasisPrice;
                    StockItems[stockMaster.Code].BasisPrice = stockMaster.BasisPrice;
                    StockItems[stockMaster.Code].Volume = 0;
                    StockItems[stockMaster.Code].PreviousVolume = stockMaster.PreviousVolume;
                    StockItems[stockMaster.Code].MarketType = stockMaster.MarketType;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private void OnIndexMasterConsumed(List<IndexMaster> indexMasters)
        {
            try
            {
                foreach (var indexMaster in indexMasters)
                {
                    if (IndexItems.ContainsKey(indexMaster.Code) == false)
                    {
                        var item = new DashboardItem(indexMaster.Code);
                        IndexItems.Add(item.Code, item);
                    }
                    IndexItems[indexMaster.Code].Time = indexMaster.Time;
                    IndexItems[indexMaster.Code].Name = indexMaster.Name;
                    IndexItems[indexMaster.Code].Price = indexMaster.BasisPrice;
                    IndexItems[indexMaster.Code].Volume = 0;
                    IndexItems[indexMaster.Code].BasisPrice = indexMaster.BasisPrice;
                    IndexItems[indexMaster.Code].MarketType = MarketTypes.INDEX;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private void OnCodeMapConsumed(Dictionary<string, object> codeMap)
        {
            //Dictionary<string, object> rebuilt = CodeMapBuilderUtil.RebuildNode(codeMap);
        }

        private void SaveDashboard()
        {
            try
            {
                var fileName = $"MTree.{Config.General.DateNow}_Dashboard.csv";
                var filePath = Path.Combine(Environment.CurrentDirectory, "Logs", Config.General.DateNow, fileName);

                using (var fs = new FileStream(filePath, FileMode.Create))
                using (var sw = new StreamWriter(fs, Encoding.Default))
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

                    sw.Flush();
                    fs.Flush(true);
                }

                _logger.Info($"Save Dashboard done, {fileName}");
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        public void OnMessageNotified(MessageTypes type, string message)
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
                            _logger.Info("Process will be closed");
                            Thread.Sleep(1000 * 5);

                            Environment.Exit(0);
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }
    }
}
