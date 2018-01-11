using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using DataStructure;
using CommonLib;
using System.Threading;
using Configuration;
using System.IO;
using System.Diagnostics;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;
using System.ComponentModel;
using DbProvider;
using System.Windows.Forms;

namespace RealTimeProvider
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false, ValidateMustUnderstand = false)]
    public partial class RealTimeProvider_ : SubscribingBase, IRealTimePublisher, IRealTimeConsumer, INotifyPropertyChanged
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

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

        public RealTimeProvider_()
        {
            string[] args = Environment.GetCommandLineArgs();
            
            if (args.Contains("SkipMastering") == true)
            {
                _logger.Info("Command args, SkipMastering");
                SkipMastering = true;
            }
            else if (Config.General.SkipMastering == true)
            {
                SkipMastering = true;
            }

            if (args.Contains("SkipCodeBuilding") == true)
            {
                _logger.Info("Command args, SkipCodeBuilding");
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
                    _logger.Info(RealTimeState);

                    ExitProgram(ExitProgramTypes.Restart);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
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
                    _logger.Error(ex);
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
                    _logger.Error(ex);
                }
            }
        } 
        #endregion

        private void MarketEndTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            _logger.Info("Market end timer elapsed");

            // Popup stopper 실행
            ProcessUtility.Start(ProcessTypes.PopupStopper, ProcessWindowStyle.Minimized);

            SaveDayChart();

            // Data validator
            if (Config.Validator.LaunchValidatorAfterMarketEnd == true)
            {
                if (string.IsNullOrEmpty(Config.Database.RemoteConnectionString) == false &&
                    Config.Database.ConnectionString != Config.Database.RemoteConnectionString)
                {
                    _logger.Info("Launch Data validator");

                    if (ProcessUtility.Start(ProcessTypes.DataValidatorRegularCheck, ProcessWindowStyle.Minimized).WaitForExit((int)TimeSpan.FromMinutes(60).TotalMilliseconds) == false)
                    {
                        _logger.Error("Data validator time out");
                        ProcessUtility.Kill(ProcessTypes.DataValidatorRegularCheck);
                    }
                }
                else
                {
                    _logger.Error("Remote connection string not valid");
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
                _logger.Info(RealTimeState);

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
                        _logger.Info($"{msg}, chart not exists");
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

                    _logger.Info($"{msg}, candleCount: {candleList.Count}, publisherTick: {publisherTick}, consumerTick: {consumerTick}");

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
                        _logger.Info($"{msg}, chart not exists");
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

                    _logger.Info($"{msg}, candleCount: {candleList.Count}, publisherTick: {publisherTick}, consumerTick: {consumerTick}");

                    candleList.Clear();
                }
                #endregion

                RealTimeState = "Save chart done";
                _logger.Info(RealTimeState);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            finally
            {
                sw.Stop();
                _logger.Info($"Save chart Elapsed time: {sw.Elapsed.ToString()}");
            }
        }

        private void ExitProgram(ExitProgramTypes exitType)
        {
            try
            {
                CanExitProgram = false;

                RealTimeState = $"Exit program, {exitType.ToString()}";
                _logger.Info(RealTimeState);

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

                        // 오래된 로그 폴더/파일을 삭제한다
                        LogUtility.DeleteOldLog();

                        // 20초후 프로그램 종료
                        RealTimeState = "RealTimeProvider will be closed after 20sec";
                        _logger.Info(RealTimeState);
                    }

                    Thread.Sleep(1000 * 20);

                    // PopupStopper 종료
                    ProcessUtility.Kill(ProcessTypes.PopupStopper);

                    if (exitType == ExitProgramTypes.Restart)
                    {
                        RealTimeState = "RealTimeProvider will be restarted";
                        _logger.Info(RealTimeState);

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
                _logger.Error(ex);
            }
        }
        
        public override void RefreshTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Counter.NotifyPropertyAll();
            NotifyPropertyChanged(nameof(BiddingPriceQueueCount));
            NotifyPropertyChanged(nameof(StockConclusionQueueCount));
            NotifyPropertyChanged(nameof(IndexConclusionQueueCount));
            NotifyPropertyChanged(nameof(ETFConclusionQueueCount));
        }

        private void ClientChannel_Closed(object sender, EventArgs e)
        {
            _logger.Info("Client channel closed");

            if (sender is IRealTimeConsumerCallback consumerCallback)
            {
                var contract = ConsumerContracts.FirstOrDefault(c => c.Value.IsMatch(consumerCallback));
                UnregisterConsumerContractAll(contract.Key);
            }
            else if (sender is IRealTimePublisherCallback publisherCallback)
            {
                var contract = PublisherContracts.FirstOrDefault(c => c.Value.IsMatch(publisherCallback));
                UnregisterPublisherContract(contract.Key);
            }
        }

        private void ClientChannel_Faulted(object sender, EventArgs e)
        {
            _logger.Error("Client channel faulted");

            if (sender is IRealTimeConsumerCallback consumerCallback)
            {
                var contract = ConsumerContracts.FirstOrDefault(c => c.Value.IsMatch(consumerCallback));
                UnregisterConsumerContractAll(contract.Key);
            }
            else if (sender is IRealTimePublisherCallback publisherCallback)
            {
                var contract = PublisherContracts.FirstOrDefault(c => c.Value.IsMatch(publisherCallback));
                UnregisterPublisherContract(contract.Key);
            }
        }

        #region Command
        private RelayCommand _sendLogCommand;
        public ICommand SendLogCommand => _sendLogCommand ?? (_sendLogCommand = new RelayCommand(() => ExecuteSendLog(), () => CanSendLog));

        public void ExecuteSendLog()
        {
            RealTimeState = "Execute send log";
            _logger.Info(RealTimeState);

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

        private RelayCommand _exitProgramCommand;
        public ICommand ExitProgramCommand => _exitProgramCommand ?? (_exitProgramCommand = new RelayCommand(() => ExecuteExitProgram(), () => CanExitProgram));

        public void ExecuteExitProgram()
        {
            RealTimeState = "Execute exit program";
            _logger.Info(RealTimeState);

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
