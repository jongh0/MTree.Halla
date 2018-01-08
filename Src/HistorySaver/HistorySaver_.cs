﻿using System;
using System.ServiceModel;
using DbProvider;
using System.Threading;
using DataStructure;
using Consumer;
using CommonLib;
using RealTimeProvider;
using System.Threading.Tasks;
using System.Windows;
using System.ComponentModel;
using Configuration;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Collections.Concurrent;
using MongoDB.Bson;

namespace HistorySaver
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class HistorySaver_ : RealTimeConsumer, INotifyPropertyChanged
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public DataCounter Counter { get; set; } = new DataCounter(DataTypes.HistorySaver);

        public HistorySaver_()
        {
            try
            {
                TaskUtility.Run("HistorySaver.CircuitBreakQueue", QueueTaskCancelToken, ProcessCircuitBreakQueue);
                TaskUtility.Run($"HistorySaver.StockConclusionQueue", QueueTaskCancelToken, ProcessStockConclusionQueue);
                TaskUtility.Run($"HistorySaver.IndexConclusionQueue", QueueTaskCancelToken, ProcessIndexConclusionQueue);

                if (Config.General.SkipETFConclusion == false)
                    TaskUtility.Run($"HistorySaver.ETFConclusionQueue", QueueTaskCancelToken, ProcessETFConclusionQueue);

                if (Config.General.SkipBiddingPrice == false)
                    TaskUtility.Run($"HistorySaver.BiddingPriceQueue", QueueTaskCancelToken, ProcessBiddingPriceQueue);

                StartRefreshTimer();

                var instance = DbAgent.Instance;
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
                if (BiddingPriceQueue.TryDequeue(out var biddingPrice) == true)
                {
                    DbAgent.Instance.Insert(biddingPrice);
                    Counter.Increment(CounterTypes.BiddingPrice);
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
                if (CircuitBreakQueue.TryDequeue(out var circuitBreak) == true)
                {
                    DbAgent.Instance.Insert(circuitBreak);
                    Counter.Increment(CounterTypes.CircuitBreak);
                }
                else
                    Thread.Sleep(10);
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
                if (StockConclusionQueue.TryDequeue(out var conclusion) == true)
                {
                    DbAgent.Instance.Insert(conclusion);
                    Counter.Increment(CounterTypes.StockConclusion);
                }
                else
                    Thread.Sleep(10);
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
                if (IndexConclusionQueue.TryDequeue(out var conclusion) == true)
                {
                    DbAgent.Instance.Insert(conclusion);
                    Counter.Increment(CounterTypes.IndexConclusion);
                }
                else
                    Thread.Sleep(10);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private void ProcessETFConclusionQueue()
        {
            try
            {
                if (ETFConclusionQueue.TryDequeue(out var conclusion) == true)
                {
                    DbAgent.Instance.Insert(conclusion);
                    Counter.Increment(CounterTypes.ETFConclusion);
                }
                else
                    Thread.Sleep(10);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
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
                _logger.Error(ex);
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
                _logger.Error(ex);
            }
        }

        public override void ConsumeCodemap(Dictionary<string, object> jsonDic)
        {
            ICodeMap codeMap = CodeMapConverter.DicToCodeMap(DateTime.Now.ToString(Config.General.DateFormat), jsonDic);
            
            CodeMapDbObject codemapDbObj = new CodeMapDbObject();
            codemapDbObj.Id = new ObjectId();
            codemapDbObj.Time = DateTimeUtility.DateOnly(DateTime.Now);
            codemapDbObj.ReceivedTime = DateTime.Now;
            codemapDbObj.Code = "CodeMap";
            codemapDbObj.CodeMap = CodeMapConverter.CodeMapToJsonString(codeMap);

            DbAgent.Instance.Insert(codemapDbObj);
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
                _logger.Error(ex);
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

                    Task.Run(() =>
                    {
                        if (message.Equals(ExitProgramTypes.Normal.ToString()) == true)
                        {
                            Counter.SaveToFile();
                            DbAgent.Instance.Insert(Counter);
                            DbAgent.Instance.CreateIndex();
                            DbAgent.Instance.SaveStatisticLog();
                        }

                        _logger.Info("Process will be closed");
                        Thread.Sleep(1000 * 5);

                        Environment.Exit(0);
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            base.NotifyMessage(type, message);
        }

        public override void RefreshTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Counter.NotifyPropertyAll();
            NotifyPropertyChanged(nameof(BiddingPriceQueueCount));
            NotifyPropertyChanged(nameof(StockConclusionQueueCount));
            NotifyPropertyChanged(nameof(IndexConclusionQueueCount));
            NotifyPropertyChanged(nameof(ETFConclusionQueueCount));
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}