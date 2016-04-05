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
    public enum ExitProgramTypes
    {
        Normal,
        Force,
    }

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

        #region Counter property
        public int BiddingPriceCount { get; set; } = 0;
        public int CircuitBreakCount { get; set; } = 0;
        public int StockConclusionCount { get; set; } = 0;
        public int IndexConclusionCount { get; set; } = 0;
        #endregion

        private System.Timers.Timer RefreshTimer { get; set; }

        private string _RealTimeState = string.Empty;
        public string RealTimeState
        {
            get { return _RealTimeState; }
            set
            {
                _RealTimeState = value;
                NotifyPropertyChanged(nameof(RealTimeState));
            }
        }

        public RealTimeProvider()
        {
            TaskUtility.Run("RealTimeProvider.BiddingPriceQueue", QueueTaskCancelToken, ProcessBiddingPriceQueue);
            TaskUtility.Run("RealTimeProvider.CircuitBreakQueue", QueueTaskCancelToken, ProcessCircuitBreakQueue);
            TaskUtility.Run("RealTimeProvider.StockConclusionQueue", QueueTaskCancelToken, ProcessStockConclusionQueue);
            TaskUtility.Run("RealTimeProvider.IndexConclusionQueue", QueueTaskCancelToken, ProcessIndexConclusionQueue);

            StartRefreshTimer();
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
                RealTimeState = "Save chart started";
                logger.Info(RealTimeState);

                var daishinContracts = DaishinContracts;
                var daishinContractCount = daishinContracts.Count;

                #region Save stock chart
                var stockCount = StockMasteringList.Count;

                for (int i = 0; i < stockCount; i++)
                {
                    var mastering = StockMasteringList[i];
                    var startDate = DateTime.Now.AddYears(-5); // 최근 5년치만 저장
                    var endDate = DateTime.Now;
                    var code = mastering.Stock.Code;
                    var fullCode = CodeEntity.ConvertToDaishinCode(StockCodeList[code]);

                    int startTick = Environment.TickCount;
                    var msg = $"Save stock chart ({i + 1}/{stockCount}), Code: {code}";

                    var candleList = daishinContracts[i % daishinContractCount].Callback.GetChart(fullCode, startDate, endDate, CandleTypes.Day);
                    if (candleList == null || candleList.Count == 0)
                    {
                        logger.Info($"{msg}, chart not exists");
                        continue;
                    }

                    int publisherTick = Environment.TickCount - startTick;
                    startTick = Environment.TickCount;

                    foreach (var consumerContract in ChartContracts)
                    {
                        consumerContract.Value.Callback.ConsumeChart(candleList);
                    }

                    int consumerTick = Environment.TickCount - startTick;

                    logger.Info($"{msg}, candleCount: {candleList.Count}, publisherTick: {publisherTick}, consumerTick: {consumerTick}");

                    candleList.Clear();
                }
                #endregion

                #region Save index chart
                var indexCount = IndexMasteringList.Count;

                for (int i = 0; i < indexCount; i++)
                {
                    var mastering = IndexMasteringList[i];
                    var startDate = DateTime.Now.AddYears(-5); // 최근 5년치만 저장
                    var endDate = DateTime.Now;
                    var code = mastering.Index.Code;
                    var fullCode = CodeEntity.ConvertToDaishinCode(IndexCodeList[code]);

                    int startTick = Environment.TickCount;
                    var msg = $"Save index chart ({i + 1}/{indexCount}), Code: {code}";

                    var candleList = daishinContracts[i % daishinContractCount].Callback.GetChart(fullCode, startDate, endDate, CandleTypes.Day);
                    if (candleList == null || candleList.Count == 0)
                    {
                        logger.Info($"{msg}, chart not exists");
                        continue;
                    }

                    int publisherTick = Environment.TickCount - startTick;
                    startTick = Environment.TickCount;

                    foreach (var consumerContract in ChartContracts)
                    {
                        consumerContract.Value.Callback.ConsumeChart(candleList);
                    }

                    int consumerTick = Environment.TickCount - startTick;

                    logger.Info($"{msg}, candleCount: {candleList.Count}, publisherTick: {publisherTick}, consumerTick: {consumerTick}");

                    candleList.Clear();
                }
                #endregion

                RealTimeState = "Save chart done";
                logger.Info(RealTimeState);
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

        private void ExitProgram(ExitProgramTypes type = ExitProgramTypes.Normal)
        {
            try
            {
                CanExitProgram = false;

                RealTimeState = $"Exit program, {type.ToString()}";
                logger.Info(RealTimeState);

                #region Publisher 종료
                foreach (var contract in PublisherContracts)
                {
                    try
                    {
                        logger.Info($"Close publisher client, {contract.ToString()}");
                        contract.Value.Callback.NotifyMessage(MessageTypes.CloseClient, type.ToString());
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
                        contract.Value.Callback.NotifyMessage(MessageTypes.CloseClient, type.ToString());
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                    }
                }
                #endregion

                // Count 업데이트 중지
                StopRefreshTimer();

                Task.Run(() =>
                {
                    // CybosStarter 종료
                    ProcessUtility.Kill(ProcessTypes.CybosStarter);

                    if (type == ExitProgramTypes.Normal)
                    {
                        // 당일 수집된 로그를 Zip해서 Email로 전송함
                        LogUtility.SendLog();

                        // Queue에 입력된 Count를 파일로 저장
                        SaveRealTimeProvider();

                        // 20초후 프로그램 종료
                        RealTimeState = "RealTimeProvider will be closed after 20sec";
                        logger.Info(RealTimeState);
                        PushUtility.NotifyMessage(RealTimeState);
                    }

                    Thread.Sleep(1000 * 20);

                    // PopupStopper 종료
                    ProcessUtility.Kill(ProcessTypes.PopupStopper);

                    Environment.Exit(0);
                });
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
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
            NotifyPropertyChanged(nameof(BiddingPriceCount));
            NotifyPropertyChanged(nameof(CircuitBreakCount));
            NotifyPropertyChanged(nameof(StockConclusionCount));
            NotifyPropertyChanged(nameof(IndexConclusionCount));
        }

        private void SaveRealTimeProvider()
        {
            try
            {
                logger.Info("Save RealTimeProvider");

                var fileName = $"MTree.{DateTime.Now.ToString(Config.General.DateFormat)}_RealTimeProvider.csv";
                var filePath = Path.Combine(Environment.CurrentDirectory, "Logs", fileName);

                using (var sw = new StreamWriter(new FileStream(filePath, FileMode.Create), Encoding.Default))
                {
                    sw.WriteLine($"CircuitBreak, {CircuitBreakCount}");
                    sw.WriteLine($"BiddingPrice, {BiddingPriceCount}");
                    sw.WriteLine($"StockConclusion, {StockConclusionCount}");
                    sw.WriteLine($"IndexConclusion, {IndexConclusionCount}");
                }

                logger.Info($"Save RealTimeProvider done, {filePath}");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        #region Command
        private RelayCommand _SendLogCommand;
        public ICommand SendLogCommand
        {
            get
            {
                if (_SendLogCommand == null)
                    _SendLogCommand = new RelayCommand(() => ExecuteSendLog(), () => CanSendLog);

                return _SendLogCommand;
            }
        }

        public void ExecuteSendLog()
        {
            RealTimeState = "Execute send log";
            logger.Info(RealTimeState);

            Task.Run(() =>
            {
                CanSendLog = false;
                LogUtility.SendLog();
                CanSendLog = true;
            });
        }

        private bool _CanSendLog = true;
        public bool CanSendLog
        {
            get { return _CanSendLog; }
            set
            {
                _CanSendLog = value;
                NotifyPropertyChanged(nameof(CanSendLog));
            }
        }

        private RelayCommand _ExitProgramCommand;
        public ICommand ExitProgramCommand
        {
            get
            {
                if (_ExitProgramCommand == null)
                    _ExitProgramCommand = new RelayCommand(() => ExecuteExitProgram(), () => CanExitProgram);

                return _ExitProgramCommand;
            }
        }

        public void ExecuteExitProgram()
        {
            RealTimeState = "Execute exit program";
            logger.Info(RealTimeState);

            Task.Run(() =>
            {
#if false // Test code
                SaveDayChart();
                ExitProgram(ExitProgramTypes.Normal);
#else
                ExitProgram(ExitProgramTypes.Force); 
#endif
            });
        }

        private bool _CanExitProgram = true;
        public bool CanExitProgram
        {
            get { return _CanExitProgram; }
            set
            {
                _CanExitProgram = value;
                NotifyPropertyChanged(nameof(CanExitProgram));
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
