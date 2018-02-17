#define ADD_DELAY_FOR_EACH_INSERT

using System;
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
using CommonLib.Utility;

namespace HistorySaver
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class HistorySaver_ : RealTimeConsumer, INotifyPropertyChanged
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private bool _consumeChartStarted = false;

        public DataCounter Counter { get; set; } = new DataCounter(DataTypes.HistorySaver);

        public HistorySaver_()
        {
            try
            {
                StartRefreshTimer();

                var instance = DbAgent.Instance;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        protected override void ServiceClient_Opened(object sender, EventArgs e)
        {
            base.ServiceClient_Opened(sender, e);

            Task.Run(() =>
            {
                try
                {
                    RegisterContract(new SubscribeContract(SubscribeTypes.Chart));
                    RegisterContract(new SubscribeContract(SubscribeTypes.Mastering));
                    RegisterContract(new SubscribeContract(SubscribeTypes.CircuitBreak));
                    RegisterContract(new SubscribeContract(SubscribeTypes.StockConclusion));
                    RegisterContract(new SubscribeContract(SubscribeTypes.IndexConclusion));

                    if (Config.General.SkipBiddingPrice == false)
                        RegisterContract(new SubscribeContract(SubscribeTypes.BiddingPrice));

                    if (Config.General.SkipETFConclusion == false)
                        RegisterContract(new SubscribeContract(SubscribeTypes.ETFConclusion));
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                }
            });
        }

        protected override void ProcessBiddingPriceQueue()
        {
            try
            {
                if (BiddingPriceQueue.TryDequeue(out var biddingPrice) == true)
                {
                    DbAgent.Instance.Insert(biddingPrice);
                    Counter.Increment(CounterTypes.BiddingPrice);
#if ADD_DELAY_FOR_EACH_INSERT
                    Thread.Sleep(1);
#endif
                }
                else
                    Thread.Sleep(10);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        protected override void ProcessCircuitBreakQueue()
        {
            try
            {
                if (CircuitBreakQueue.TryDequeue(out var circuitBreak) == true)
                {
                    DbAgent.Instance.Insert(circuitBreak);
                    Counter.Increment(CounterTypes.CircuitBreak);
#if ADD_DELAY_FOR_EACH_INSERT
                    Thread.Sleep(1);
#endif
                }
                else
                    Thread.Sleep(10);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        protected override void ProcessStockConclusionQueue()
        {
            try
            {
                if (StockConclusionQueue.TryDequeue(out var conclusion) == true)
                {
                    DbAgent.Instance.Insert(conclusion);
                    Counter.Increment(CounterTypes.StockConclusion);
#if ADD_DELAY_FOR_EACH_INSERT
                    Thread.Sleep(1);
#endif
                }
                else
                    Thread.Sleep(10); 
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        protected override void ProcessIndexConclusionQueue()
        {
            try
            {
                if (IndexConclusionQueue.TryDequeue(out var conclusion) == true)
                {
                    DbAgent.Instance.Insert(conclusion);
                    Counter.Increment(CounterTypes.IndexConclusion);
#if ADD_DELAY_FOR_EACH_INSERT
                    Thread.Sleep(1);
#endif
                }
                else
                    Thread.Sleep(10); 
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        protected override void ProcessETFConclusionQueue()
        {
            try
            {
                if (ETFConclusionQueue.TryDequeue(out var conclusion) == true)
                {
                    DbAgent.Instance.Insert(conclusion);
                    Counter.Increment(CounterTypes.ETFConclusion);
#if ADD_DELAY_FOR_EACH_INSERT
                    Thread.Sleep(1);
#endif
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
            if (stockMasters == null || stockMasters.Count == 0) return;

            try
            {
                foreach (var master in stockMasters)
                {
                    DbAgent.Instance.Insert(master);
                }

                Counter.Add(CounterTypes.StockMaster, stockMasters.Count);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        public override void ConsumeIndexMaster(List<IndexMaster> indexMasters)
        {
            if (indexMasters == null || indexMasters.Count == 0) return;

            try
            {
                foreach (var master in indexMasters)
                {
                    DbAgent.Instance.Insert(master);
                }

                Counter.Add(CounterTypes.IndexMaster, indexMasters.Count);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        public override void ConsumeCodeMap(Dictionary<string, object> jsonDic)
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
                // 매일 처음 Chart 저장 시 모든 Chart Collection Drop 시킨다.
                if (_consumeChartStarted == false)
                {
                    _consumeChartStarted = true;

                    _logger.Info("Drop all chart collection");

                    foreach (var collectionName in DbAgent.Instance.GetCollectionList(DbTypes.Chart))
                    {
                        DbAgent.Instance.ChartDb.DropCollection(collectionName);
                    }
                }

                foreach (var candle in candles)
                {
                    DbAgent.Instance.Insert(candle);
                }

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
