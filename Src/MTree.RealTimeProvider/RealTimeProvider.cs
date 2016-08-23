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
using MTree.DbProvider;
using System.Windows.Forms;

namespace MTree.RealTimeProvider
{
    public enum ExitProgramTypes
    {
        Normal,
        Force,
        Restart,
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false, ValidateMustUnderstand = false)]
    public partial class RealTimeProvider : SubscribingBase, IRealTimePublisher, IRealTimeConsumer, INotifyPropertyChanged
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public DateTime MarketStartTime { get; set; }
        public DateTime MarketEndTime { get; set; }

        private System.Timers.Timer MarketEndTimer { get; set; }

        private Dictionary<string, CodeEntity> StockCodeList { get; set; } = new Dictionary<string, CodeEntity>();
        private Dictionary<string, CodeEntity> IndexCodeList { get; set; } = new Dictionary<string, CodeEntity>();

        private List<StockMastering> StockMasteringList { get; } = new List<StockMastering>();
        private List<IndexMastering> IndexMasteringList { get; } = new List<IndexMastering>();

        public DataCounter Counter { get; set; } = new DataCounter(DataTypes.RealTimeProvider);

        private bool SkipMastering { get; set; } = false;
        private bool SkipCodeBuilding { get; set; } = false;
        private bool MasteringDone { get; set; } = false;

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
            string[] args = Environment.GetCommandLineArgs();
            
            if (args.Contains("SkipMastering") == true)
            {
                logger.Info("Command args, SkipMastering");
                SkipMastering = true;
            }
            else if (Config.General.SkipMastering == true)
            {
                SkipMastering = true;
            }

            if (args.Contains("SkipCodeBuilding") == true)
            {
                logger.Info("Command args, SkipCodeBuilding");
                SkipCodeBuilding = true;
            }
            else if (Config.General.SkipCodeBuilding == true)
            {
                SkipCodeBuilding = true;
            }

            TaskUtility.Run("RealTimeProvider.CircuitBreakQueue", QueueTaskCancelToken, ProcessCircuitBreakQueue);
            TaskUtility.Run("RealTimeProvider.StockConclusionQueue", QueueTaskCancelToken, ProcessStockConclusionQueue);
            TaskUtility.Run("RealTimeProvider.IndexConclusionQueue", QueueTaskCancelToken, ProcessIndexConclusionQueue);

            if (Config.General.SkipETFConclusion == false)
                TaskUtility.Run("RealTimeProvider.ETFConclusionQueue", QueueTaskCancelToken, ProcessETFConclusionQueue);

            if (Config.General.SkipBiddingPrice == false)
                TaskUtility.Run("RealTimeProvider.BiddingPriceQueue", QueueTaskCancelToken, ProcessBiddingPriceQueue);

            StartRefreshTimer();
        }

        #region NotifyMessage
        public void NotifyMessage(MessageTypes type, string message)
        {
            try
            {
                if (type == MessageTypes.DaishinSessionDisconnected)
                {
                    RealTimeState = "Daishin session disconnected";
                    logger.Info(RealTimeState);
                    PushUtility.NotifyMessage(RealTimeState);

                    ExitProgram(ExitProgramTypes.Restart);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private void NotifyMessageToConsumer(MessageTypes type, string message = "")
        {
            foreach (var contract in ConsumerContracts)
            {
                try
                {
                    contract.Value.Callback.NotifyMessage(type, message);
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }
            }
        }

        private void NotifyMessageToPubliser(MessageTypes type, string message = "")
        {
            foreach (var contract in PublisherContracts)
            {
                try
                {
                    contract.Value.Callback.NotifyMessage(type, message);
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }
            }
        } 
        #endregion

        private void MarketEndTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            logger.Info("Market end timer elapsed");

            // Popup stopper 실행
            ProcessUtility.Start(ProcessTypes.PopupStopper, ProcessWindowStyle.Minimized);

            SaveDayChart();

            // Data validator
            if (Config.Validator.LaunchValidatorAfterMarketEnd == true)
            {
                if (string.IsNullOrEmpty(Config.Database.RemoteConnectionString) == false &&
                    Config.Database.ConnectionString != Config.Database.RemoteConnectionString)
                {
                    logger.Info("Launch Data validator");

                    if (ProcessUtility.Start(ProcessTypes.DataValidatorRegularCheck, ProcessWindowStyle.Minimized).WaitForExit((int)TimeSpan.FromMinutes(60).TotalMilliseconds) == false)
                    {
                        logger.Error("Data validator time out");
                        ProcessUtility.Kill(ProcessTypes.DataValidatorRegularCheck);
                    }
                }
                else
                {
                    logger.Error("Remote connection string not valid");
                }
            }

            ExitProgram(ExitProgramTypes.Normal);
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

                    Counter.Add(CounterTypes.Chart, candleList.Count);

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

                    Counter.Add(CounterTypes.Chart, candleList.Count);

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

        private void ExitProgram(ExitProgramTypes exitType)
        {
            try
            {
                CanExitProgram = false;

                RealTimeState = $"Exit program, {exitType.ToString()}";
                logger.Info(RealTimeState);

                // Publisher 종료
                NotifyMessageToPubliser(MessageTypes.CloseClient, exitType.ToString());

                // Consumer 종료
                NotifyMessageToConsumer(MessageTypes.CloseClient, exitType.ToString());

                // Count 업데이트 중지
                StopRefreshTimer();

                Task.Run(() =>
                {
                    // CybosStarter 종료
                    ProcessUtility.Kill(ProcessTypes.CybosStarter);

                    if (exitType == ExitProgramTypes.Normal)
                    {
                        // Queue에 입력된 Count를 파일로 저장
                        Counter.SaveToFile();

                        // Queue에 입력된 Count를 DB에 저장
                        DbAgent.Instance.Insert(Counter);

                        // 당일 수집된 로그를 Zip해서 Email로 전송함
                        LogUtility.SendLog();

                        // 20초후 프로그램 종료
                        RealTimeState = "RealTimeProvider will be closed after 20sec";
                        logger.Info(RealTimeState);
                        PushUtility.NotifyMessage(RealTimeState);
                    }

                    Thread.Sleep(1000 * 20);

                    // PopupStopper 종료
                    ProcessUtility.Kill(ProcessTypes.PopupStopper);

                    if (exitType == ExitProgramTypes.Restart)
                    {
                        RealTimeState = "RealTimeProvider will be restarted";
                        logger.Info(RealTimeState);
                        PushUtility.NotifyMessage(RealTimeState);

                        //Application.Restart();
                        if (MasteringDone == true)
                            ProcessUtility.Start(ProcessTypes.RealTimeProvider, "SkipMastering SkipCodeBuilding");
                        else
                            ProcessUtility.Start(ProcessTypes.RealTimeProvider);
                    }

                    Environment.Exit(0);
                });
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
        
        public override void RefreshTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Counter.NotifyPropertyAll();
            NotifyPropertyChanged(nameof(BiddingPriceQueueCount));
            NotifyPropertyChanged(nameof(StockConclusionQueueCount));
            NotifyPropertyChanged(nameof(IndexConclusionQueueCount));
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
                ExitProgram(ExitProgramTypes.Restart);
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
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}
