using Configuration;
using DataStructure;
using CommonLib;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using CommonLib.Utility;

namespace RealTimeProvider
{
    public partial class RealTimeProvider_
    {
        public int PublisherContractCount { get { return PublisherContracts.Count; } }
        private ConcurrentDictionary<Guid, PublisherContract> PublisherContracts { get; set; } = new ConcurrentDictionary<Guid, PublisherContract>();


        #region Contract Property
        #region Daishin
        private List<PublisherContract> DaishinContracts
        {
            get { return PublisherContracts.Values.Where(c => c.Type == ProcessTypes.DaishinPublisher).ToList(); }
        }

        private PublisherContract DaishinContractForMastering
        {
            get { return PublisherContracts.Values.FirstOrDefault(c => c.Type == ProcessTypes.DaishinPublisher && c.IsOperating == false); }
        }

        private PublisherContract DaishinMasterContract
        {
            get { return PublisherContracts.Values.FirstOrDefault(c => c.Type == ProcessTypes.DaishinPublisherMaster); }
        }
        #endregion

        #region Ebest
        private List<PublisherContract> EbestContracts
        {
            get { return PublisherContracts.Values.Where(c => c.Type == ProcessTypes.EbestPublisher).ToList(); }
        }

        private PublisherContract EbestContractForMastering
        {
            get { return PublisherContracts.Values.FirstOrDefault(c => c.Type == ProcessTypes.EbestPublisher && c.IsOperating == false); }
        }
        #endregion

        #region Kiwoom
        private List<PublisherContract> KiwoomContracts
        {
            get { return PublisherContracts.Values.Where(c => c.Type == ProcessTypes.KiwoomPublisher).ToList(); }
        }

        private PublisherContract KiwoomContractForMastering
        {
            get { return PublisherContracts.Values.FirstOrDefault(c => c.Type == ProcessTypes.KiwoomPublisher && c.IsOperating == false); }
        }
        #endregion
        #endregion

        public void RegisterPublisherContract(Guid clientId, PublisherContract contract)
        {
            try
            {
                if (PublisherContracts.ContainsKey(clientId) == true)
                {
                    _logger.Error($"{contract.ToString()} publisher contract exist / {clientId}");
                }
                else
                {
                    contract.Id = PublisherContract.IdNumbering++;
                    contract.Callback = OperationContext.Current.GetCallbackChannel<IRealTimePublisherCallback>();

#if false // Channel 오류 시 Callback 사용 중지하기 위해서
                    if (contract.Callback is ICommunicationObject clientChannel)
                    {
                        clientChannel.Faulted += ClientChannel_Faulted;
                        clientChannel.Closed += ClientChannel_Closed;
                    } 
#endif

                    if (contract.Type == ProcessTypes.Unknown)
                    {
                        PublisherContracts.TryAdd(clientId, contract);
                        _logger.Warn($"{contract.ToString()} publisher contract type is not set / {clientId}");
                    }
                    else
                    {
                        bool isMasterContract = (contract.Type == ProcessTypes.DaishinPublisherMaster);
                        if (contract.Type == ProcessTypes.DaishinPublisherMaster) contract.Type = ProcessTypes.DaishinPublisher;

                        PublisherContracts.TryAdd(clientId, contract);
                        _logger.Info($"{contract.ToString()} publisher contract registered / {clientId}");

                        if (isMasterContract == true)
                        {
                            if (CheckMarketWorkDate(contract) == false)
                            {
                                // 휴장. Closing Process
                                var task = Task.Run(() =>
                                {
                                    _logger.Info("RealTimeProvider will be closed after 5sec");

                                    Thread.Sleep(1000 * 5);
                                    ExitProgram(ExitProgramTypes.Force);
                                });
                                task.Wait();
                            }

                            ProcessMasterContract(contract);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            finally
            {
                NotifyPropertyChanged(nameof(PublisherContractCount));
            }
        }

        /// <summary>
        /// DaishinMaster Client가 접속 시 순차적으로 Code 리스트 확인, Mastering, Code Distributing을 수행한다.
        /// 장이 열리지 않는 날이면 Program을 종료시킨다. (DEBUG Mode 제외)
        /// </summary>
        /// <param name="contract"></param>
        private void ProcessMasterContract(PublisherContract contract)
        {
            try
            {
                RealTimeState = "Process master contract";
                _logger.Info(RealTimeState);
                
                CheckMarketTime(contract);
                CheckCodeList(contract);

                LaunchClientProcess();

                Task.Run(() =>
                {
                    RealTimeState = "Wait 20sec for client process launch";
                    _logger.Info(RealTimeState);

                    Thread.Sleep(1000 * 20);
                    
                    StartCodeDistributing();

                    if (SkipMastering == false)
                    {
                        StartStockMastering();
                        StartIndexMastering();
                    }
                    
                    if (SkipCodeBuilding == false)
                        StartCodeMapBuilding(contract); // Should be after master finished

                    NotifyMessageToConsumer(MessageTypes.MasteringDone);

                    #region Cleanup Processes
                    ProcessUtility.Kill(ProcessTypes.PopupStopper);

                    foreach (var kiwoomContract in KiwoomContracts)
                    {
                        kiwoomContract.Callback.NotifyMessage(MessageTypes.CloseClient, string.Empty);
                    } 
                    #endregion

                });
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            finally
            {
                NotifyPropertyChanged(nameof(PublisherContractCount));
            }
        }

        private void CheckCodeList(PublisherContract contract)
        {
            try
            {
                RealTimeState = "Check code list";
                _logger.Info(RealTimeState);

                var codeList = contract.Callback.GetCodeList();

                foreach (KeyValuePair<string, CodeEntity> codeEntity in codeList)
                {
                    if (codeEntity.Value.MarketType == MarketTypes.INDEX ||
                        codeEntity.Value.MarketType == MarketTypes.ELW)
                    {
                        IndexCodeList.Add(codeEntity.Key, codeEntity.Value);
                    }
                    else
                    {
                        StockCodeList.Add(codeEntity.Key, codeEntity.Value);
                    }
                }

                _logger.Info($"Stock code list count: {StockCodeList.Count}");
                _logger.Info($"Index code list count: {IndexCodeList.Count}");
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        /// <summary>
        /// 오늘 장이 열리면 true, 아니면 false를 리턴한다.
        /// DEBUG Mode는 항상 true를 리턴한다.
        /// </summary>
        /// <param name="contract"></param>
        /// <returns></returns>
        private bool CheckMarketWorkDate(PublisherContract contract)
        {
            try
            {
                RealTimeState = "Check market work date";
                _logger.Info(RealTimeState);

                var workDate = contract.Callback.GetMarketInfo(MarketInfoTypes.WorkDate);
                _logger.Info($"Market work date: {workDate}");

                if (int.TryParse(workDate, out var workDateTime) == true)
                {
                    int year = workDateTime / 10000;
                    int month = workDateTime % 10000 / 100;
                    int day = workDateTime % 100;

                    var now = DateTime.Now;
                    if (now.Year == year && now.Month == month && now.Day == day)
                    {
                        _logger.Info("Market opened");
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

#if DEBUG
            return true;
#else
            _logger.Info("Market closed");
            return false;
#endif
        }

        private void CheckMarketTime(PublisherContract contract)
        {
            try
            {
                RealTimeState = "Check market time";
                _logger.Info(RealTimeState);

                var now = DateTime.Now;

                // Market start time
                var startTimeStr = contract.Callback.GetMarketInfo(MarketInfoTypes.StartTime);

                if (int.TryParse(startTimeStr, out var startTime) == true)
                {
                    MarketStartTime = new DateTime(now.Year, now.Month, now.Day, startTime / 100, startTime % 100, 0);
                }
                else
                {
                    _logger.Error("Market start time parsing error");
                    MarketStartTime = new DateTime(now.Year, now.Month, now.Day, 9, 0, 0);
                }

                _logger.Info($"Market start time: {MarketStartTime.ToString(Config.General.TimeFormat)}");

                // Market end time
                var endTimeStr = contract.Callback.GetMarketInfo(MarketInfoTypes.EndTime);

                if (int.TryParse(endTimeStr, out var endTime) == true)
                {
                    MarketEndTime = new DateTime(now.Year, now.Month, now.Day, endTime / 100, endTime % 100, 0).Add(new TimeSpan(2, 30, 00)); // 시간외 2시간 30분 추가
                }
                else
                {
                    _logger.Error("Market end time parsing error");
                    MarketEndTime = new DateTime(now.Year, now.Month, now.Day, 15, 30, 0).Add(new TimeSpan(2, 30, 00)); // 시간외 2시간 30분 추가
                }

                _logger.Info($"Market end time: {MarketEndTime.ToString(Config.General.TimeFormat)}");

                if (MarketEndTime > now)
                {
                    TimeSpan interval = (MarketEndTime - now).Add(TimeSpan.FromMinutes(10)); // 장종료 10분 후 프로그램 종료

                    MarketEndTimer = new System.Timers.Timer();
                    MarketEndTimer.Interval = interval.TotalMilliseconds;
                    MarketEndTimer.Elapsed += MarketEndTimer_Elapsed;
                    MarketEndTimer.AutoReset = false;
                    MarketEndTimer.Start();

                    _logger.Info($"Market end timer will be triggered after {interval.Hours}:{interval.Minutes}");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        public void UnregisterPublisherContract(Guid clientId)
        {
            if (clientId == Guid.Empty) return;

            try
            {
                if (PublisherContracts.ContainsKey(clientId) == true)
                {
                    PublisherContracts.TryRemove(clientId, out var temp);
                    _logger.Info($"{clientId} / {temp.Type} contract unregistered");
                }
                else
                {
                    _logger.Warn($"{clientId} contract not exist");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private void StartCodeDistributing()
        {
            RealTimeState = "Start code distributing";
            _logger.Info(RealTimeState);

            DistributeConclusionAndBiddingSubscribingCode();
            DistributeCircuitBreakSubscribingCode();
        }

        private void DistributeConclusionAndBiddingSubscribingCode()
        {
            try
            {
                RealTimeState = "Conclusion and Bidding code distribution, Start";
                _logger.Info(RealTimeState);

                for (int i = 0; i < StockCodeList.Count; i++)
                {
                    var codeEntiry = StockCodeList.Values.ElementAt(i);
                    var code = CodeEntity.ConvertToDaishinCode(codeEntiry);
                    var contract = DaishinContracts[i % DaishinContracts.Count];

                    if (contract.Callback.SubscribeStock(code) == false)
                        _logger.Error($"Stock conclusioin code distribution fail. code: {code}");

                    if (Config.General.SkipETFConclusion == false &&
                        codeEntiry.MarketType == MarketTypes.ETF)
                    {
                        if (contract.Callback.SubscribeETF(code) == false)
                            _logger.Error($"ETF conclusion code distribution fail. code: {code}");
                    }

                    if (Config.General.SkipBiddingPrice == false)
                    {
                        if (contract.Callback.SubscribeBidding(code) == false)
                            _logger.Error($"Bidding code distribution fail. code: {code}");
                    }
                }

                for (int i = 0; i < IndexCodeList.Count; i++)
                {
                    var codeEntiry = IndexCodeList.Values.ElementAt(i);
                    var code = CodeEntity.ConvertToDaishinCode(codeEntiry);
                    var contract = DaishinContracts[i % DaishinContracts.Count];

                    if (contract.Callback.SubscribeIndex(code) == false)
                        _logger.Error($"Index conclusioin code distribution fail. code: {code}");
                }

                RealTimeState = "Conclusion and Bidding code distribution, Done";
                _logger.Info(RealTimeState);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private void DistributeCircuitBreakSubscribingCode()
        {
            RealTimeState = "Circuite break code distribution, Start";
            _logger.Info(RealTimeState);

            int errorCnt = 0;

            for (int i = 0; i < StockCodeList.Count; i++)
            {
                var codeEntity = StockCodeList.Values.ElementAt(i);
                var code = codeEntity.Code;
                var contract = EbestContracts[i % EbestContracts.Count];

                if (contract.Type == ProcessTypes.DaishinPublisher)
                    code = CodeEntity.ConvertToDaishinCode(codeEntity);

                if (contract.Callback.SubscribeCircuitBreak(code) == false)
                {
                    _logger.Error($"Circuite break code distribution fail. code: {code}");
                    i--;

                    errorCnt++;
                    if (errorCnt > 10)
                        break;
                }
            }

            if (errorCnt > 10)
            {
                RealTimeState = $"Circuite break code distribution, Fail. error count: {errorCnt}";
                _logger.Error(RealTimeState);
            }
            else
            {
                RealTimeState = "Circuite break code distribution, Done";
                _logger.Info(RealTimeState);
            }
        }

        public void PublishBiddingPrice(BiddingPrice biddingPrice)
        {
            BiddingPriceQueue.Enqueue(biddingPrice);
            Counter.Increment(CounterTypes.BiddingPrice);
        }

        public void PublishCircuitBreak(CircuitBreak circuitBreak)
        {
            CircuitBreakQueue.Enqueue(circuitBreak);
            Counter.Increment(CounterTypes.CircuitBreak);
        }

        public void PublishIndexConclusion(IndexConclusion conclusion)
        {
            IndexConclusionQueue.Enqueue(conclusion);
            Counter.Increment(CounterTypes.IndexConclusion);
        }

        public void PublishStockConclusion(StockConclusion conclusion)
        {
            StockConclusionQueue.Enqueue(conclusion);
            Counter.Increment(CounterTypes.StockConclusion);
        }

        public void PublishETFConclusion(ETFConclusion conclusion)
        {
            ETFConclusionQueue.Enqueue(conclusion);
            Counter.Increment(CounterTypes.ETFConclusion);
        }
    }
}
