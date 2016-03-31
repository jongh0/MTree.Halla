using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using MTree.DataStructure;
using MTree.Utility;
using System.Threading;
using MTree.Configuration;
using System.IO;
using System.Diagnostics;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;
using System.ComponentModel;

namespace MTree.RealTimeProvider
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public partial class RealTimeProvider : RealTimeBase, IRealTimePublisher, IRealTimeConsumer, INotifyPropertyChanged
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public DateTime MarketStartTime { get; set; }
        public DateTime MarketEndTime { get; set; }

        private System.Timers.Timer MarketEndTimer { get; set; }

        private Dictionary<string, CodeEntity> StockCodeList { get; set; } = new Dictionary<string, CodeEntity>();
        private Dictionary<string, CodeEntity> IndexCodeList { get; set; } = new Dictionary<string, CodeEntity>();

        private List<StockMastering> StockMasteringList { get; } = new List<StockMastering>();
        private List<IndexMastering> IndexMasteringList { get; } = new List<IndexMastering>();

        public RealTimeProvider()
        {
            TaskUtility.Run("RealTimeProvider.BiddingPriceQueue", QueueTaskCancelToken, ProcessBiddingPriceQueue);
            TaskUtility.Run("RealTimeProvider.CircuitBreakQueue", QueueTaskCancelToken, ProcessCircuitBreakQueue);
            TaskUtility.Run("RealTimeProvider.StockConclusionQueue", QueueTaskCancelToken, ProcessStockConclusionQueue);
            TaskUtility.Run("RealTimeProvider.IndexConclusionQueue", QueueTaskCancelToken, ProcessIndexConclusionQueue);
        }

        public void NotifyMessage(MessageTypes type, string message)
        {
            try
            {
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private void MarketEndTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            logger.Info("Market end timer elapsed");

            SaveDayChart();
            ExitProgram();
        }

        private void SaveDayChart()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            try
            {
                logger.Info("Save chart");

                var daishinContracts = DaishinContracts;
                var daishinContractCount = daishinContracts.Count;

                #region Save stock chart
                var stockCount = StockMasteringList.Count;

                for (int i = 0; i < stockCount; i++)
                {
                    var mastering = StockMasteringList[i];
                    var startDate = mastering.Stock.ListedDate;
                    var endDate = DateTime.Now;
                    var code = mastering.Stock.Code;
                    var fullCode = CodeEntity.ConvertToDaishinCode(StockCodeList[code]);

                    int startTick = Environment.TickCount;

                    var candleList = daishinContracts[i % daishinContractCount].Callback.GetChart(fullCode, startDate, endDate, CandleTypes.Day);
                    if (candleList == null || candleList.Count == 0)
                    {
                        logger.Info($"{code} stock chart not exists");
                        continue;
                    }

                    int publisherTick = Environment.TickCount - startTick;
                    startTick = Environment.TickCount;

                    foreach (var consumerContract in ChartContracts)
                    {
                        consumerContract.Value.Callback.ConsumeChart(candleList);
                    }

                    int consumerTick = Environment.TickCount - startTick;

                    logger.Info($"Save stock chart {i}/{stockCount}, {code}, {startDate.ToShortDateString()}, candle count: {candleList.Count}, publisher tick: {publisherTick}, consumer tick: {consumerTick}");

                    candleList.Clear();
                }
                #endregion

                #region Save index chart
                var indexCount = IndexMasteringList.Count;

                for (int i = 0; i < indexCount; i++)
                {
                    var mastering = IndexMasteringList[i];
                    var startDate = Config.General.DefaultStartDate;
                    var endDate = DateTime.Now;
                    var code = mastering.Index.Code;
                    var fullCode = CodeEntity.ConvertToDaishinCode(IndexCodeList[code]);

                    int startTick = Environment.TickCount;

                    var candleList = daishinContracts[i % daishinContractCount].Callback.GetChart(fullCode, startDate, endDate, CandleTypes.Day);
                    if (candleList == null || candleList.Count == 0)
                    {
                        logger.Info($"{code} index chart not exists");
                        continue;
                    }

                    int publisherTick = Environment.TickCount - startTick;
                    startTick = Environment.TickCount;

                    foreach (var consumerContract in ChartContracts)
                    {
                        consumerContract.Value.Callback.ConsumeChart(candleList);
                    }

                    int consumerTick = Environment.TickCount - startTick;

                    logger.Info($"Save index chart {i}/{indexCount}, {code}, {startDate.ToShortDateString()}, candle count: {candleList.Count}, publisher tick: {publisherTick}, consumer tick: {consumerTick}");

                    candleList.Clear();
                } 
                #endregion

                logger.Info("Chart saving done");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                sw.Stop();
                logger.Info($"Save chart Elapsed time: {sw.Elapsed.ToString()}");
            }
        }

        private void ExitProgram()
        {
            logger.Info("Exit program");

            #region Publisher 종료
            foreach (var contract in PublisherContracts)
            {
                try
                {
                    logger.Info($"Close publisher client, {contract.ToString()}");
                    contract.Value.Callback.NotifyMessage(MessageTypes.CloseClient, string.Empty);
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }
            }
            #endregion

            #region Consumer 종료
            foreach (var contract in ConsumerContracts)
            {
                try
                {
                    logger.Info($"Close consumer client, {contract.ToString()}");
                    contract.Value.Callback.NotifyMessage(MessageTypes.CloseClient, string.Empty);
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }
            } 
            #endregion

            Task.Run(() =>
            {
                // CybosStarter 종료
                ProcessUtility.Kill(ProcessTypes.CybosStarter);

                // 당일 수집된 로그를 Zip해서 Email로 전송함
                LogUtility.SendLogToEmail();

                // 20초후 프로그램 종료
                var msg = "RealTimeProvider will be closed after 20sec";
                logger.Info(msg);
                PushUtility.NotifyMessage(msg);

                Thread.Sleep(1000 * 20);

                // PopupStopper 종료
                ProcessUtility.Kill(ProcessTypes.PopupStopper);

                Environment.Exit(0);
            });
        }

        #region Command
        RelayCommand _SendLogCommand;
        public ICommand SendLogCommand
        {
            get
            {
                if (_SendLogCommand == null)
                    _SendLogCommand = new RelayCommand(() => ExecuteSendLog(), () => CanExecuteSendLog);

                return _SendLogCommand;
            }
        }

        public void ExecuteSendLog()
        {
            Task.Run(() =>
            {
                CanExecuteSendLog = false;
                LogUtility.SendLogToEmail();
                CanExecuteSendLog = true;
            });
        }

        private bool _CanExecuteSendLog = true;
        public bool CanExecuteSendLog
        {
            get { return _CanExecuteSendLog; }
            set
            {
                _CanExecuteSendLog = value;
                NotifyPropertyChanged(nameof(CanExecuteSendLog));
            }
        }
        #endregion

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
