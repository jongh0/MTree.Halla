﻿using System;
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

        private ConsumerBase Consumer { get; set; }

        public Dashboard_(ConsumerBase consumer)
        {
            try
            {
                Consumer = consumer;
                
                Consumer.ConsumeStockMasterEvent += ConsumeStockMaster;
                Consumer.ConsumeIndexMasterEvent += ConsumeIndexMaster;
                Consumer.ConsumeCodemapEvent += ConsumeCodemap;
                Consumer.NotifyMessageEvent += NotifyMessage;

                TaskUtility.Run("Dashboard.CircuitBreakQueue", Consumer.QueueTaskCancelToken, ProcessCircuitBreakQueue);
                TaskUtility.Run("Dashboard.StockConclusionQueue", Consumer.QueueTaskCancelToken, ProcessStockConclusionQueue);
                TaskUtility.Run("Dashboard.IndexConclusionQueue", Consumer.QueueTaskCancelToken, ProcessIndexConclusionQueue);

                if (Config.General.SkipBiddingPrice == false)
                {
                    TaskUtility.Run("Dashboard.BiddingPriceQueue", Consumer.QueueTaskCancelToken, ProcessBiddingPriceQueue);
                }

                if (Config.General.VerifyLatency == true)
                {
                    Monitor = new TrafficMonitor();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }
       
        private void ProcessBiddingPriceQueue()
        {
            try
            {
                if (Consumer.BiddingPriceQueue.TryDequeue(out var biddingPrice) == true)
                {
                    if (Config.General.VerifyLatency == true)
                        Monitor.CheckLatency(biddingPrice);
                }
                else
                    Thread.Sleep(10);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private void ProcessCircuitBreakQueue()
        {
            try
            {
                if (Consumer.CircuitBreakQueue.TryDequeue(out var circuitBreak) == true)
                {
                    if (Config.General.VerifyLatency == true)
                        Monitor.CheckLatency(circuitBreak);

                    if (StockItems.ContainsKey(circuitBreak.Code) == true)
                        StockItems[circuitBreak.Code].CircuitBreakType = circuitBreak.CircuitBreakType;
                    else
                        _logger.Warn($"Circuit break code not in mastering data: {circuitBreak.Code}");
                }
                else
                {
                    Thread.Sleep(10);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private void ProcessStockConclusionQueue()
        {
            try
            {
                if (Consumer.StockConclusionQueue.TryDequeue(out var conclusion) == true)
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
                else
                {
                    Thread.Sleep(10);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private void ProcessIndexConclusionQueue()
        {
            try
            {
                if (Consumer.IndexConclusionQueue.TryDequeue(out var conclusion) == true)
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
                else
                {
                    Thread.Sleep(10);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        public void ConsumeStockMaster(List<StockMaster> stockMasters)
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

        public void ConsumeIndexMaster(List<IndexMaster> indexMasters)
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

        public void ConsumeCodemap(Dictionary<string, object> codeMap)
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

        public void NotifyMessage(MessageTypes type, string message)
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