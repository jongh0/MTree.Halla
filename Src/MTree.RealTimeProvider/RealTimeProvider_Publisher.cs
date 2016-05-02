#define EVENLY_DISTRIBUTION

using MongoDB.Bson;
using MTree.Configuration;
using MTree.DataStructure;
using MTree.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTree.RealTimeProvider
{
    public partial class RealTimeProvider
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

        private void LaunchClientProcess()
        {
            try
            {
                RealTimeState = "Launch client process";
                logger.Info(RealTimeState);

                // HistorySaver
                if (Config.General.LaunchHistorySaver == true)
                    ProcessUtility.Start(ProcessTypes.HistorySaver);

                // Dashboard
                if (Config.General.LaunchDashboard == true)
                    ProcessUtility.Start(ProcessTypes.Dashboard);

                // Kiwoom
                if (Config.General.SkipMastering == false &&
                    Config.General.ExcludeKiwoom == false)
                    ProcessUtility.Start(ProcessTypes.KiwoomPublisher, ProcessWindowStyle.Minimized);

                // Daishin
                int daishinProcessCount;
#if false
                if (Config.General.SkipBiddingPrice == true)
                    daishinProcessCount = (StockCodeList.Count * 2 + IndexCodeList.Count) / 400;
                else
                    daishinProcessCount = (StockCodeList.Count * 3 + IndexCodeList.Count) / 400;
#if EVENLY_DISTRIBUTION
                daishinProcessCount += 6;
#endif
#else       
                daishinProcessCount = 39;
#endif

                for (int i = 0; i < daishinProcessCount; i++)
                    ProcessUtility.Start(ProcessTypes.DaishinPublisher, ProcessWindowStyle.Minimized);

                // Ebest
                if (Config.General.ExcludeEbest == false)
                {
                    int ebestProcessCount = 3;
                    for (int i = 0; i < ebestProcessCount; i++)
                        ProcessUtility.Start(ProcessTypes.EbestPublisher, ProcessWindowStyle.Minimized);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public void RegisterContract(Guid clientId, PublisherContract contract)
        {
            try
            {
                if (PublisherContracts.ContainsKey(clientId) == true)
                {
                    logger.Error($"{contract.ToString()} contract exist / {clientId}");
                }
                else
                {
                    contract.Id = PublisherContract.IdNumbering++;
                    contract.Callback = OperationContext.Current.GetCallbackChannel<IRealTimePublisherCallback>();

                    if (contract.Type == ProcessTypes.Unknown)
                    {
                        PublisherContracts.TryAdd(clientId, contract);
                        logger.Warn($"{contract.ToString()} contract type is not set / {clientId}");
                    }
                    else
                    {
                        bool isMasterContract = (contract.Type == ProcessTypes.DaishinPublisherMaster);
                        if (contract.Type == ProcessTypes.DaishinPublisherMaster) contract.Type = ProcessTypes.DaishinPublisher;

                        PublisherContracts.TryAdd(clientId, contract);
                        logger.Info($"{contract.ToString()} contract registered / {clientId}");

                        if (isMasterContract == true)
                            ProcessMasterContract(contract);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
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
                logger.Info(RealTimeState);

                if (CheckMarketWorkDate(contract) == true)
                {
                    CheckMarketTime(contract);
                    CheckCodeList(contract);

                    LaunchClientProcess();

                    Task.Run(() =>
                    {
                        RealTimeState = "Wait 20sec for client process launch";
                        logger.Info(RealTimeState);

                        Thread.Sleep(1000 * 20);

                        if (Config.General.SkipMastering == true)
                        {
                            StartCodeDistributing();
                        }
                        else
                        {
                            StartCodeDistributing();
                            StartStockMastering();
                            StartIndexMastering();
                        }

                        NotifyMessageToConsumer(MessageTypes.MasteringDone);

                        ProcessUtility.Kill(ProcessTypes.PopupStopper);
                    });
                }
                else
                {
                    Task.Run(() =>
                    {
                        logger.Info("RealTimeProvider will be closed after 5sec");

                        Thread.Sleep(1000 * 5);
                        ExitProgram(ExitProgramTypes.Force);
                    });
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
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
                logger.Info(RealTimeState);

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

                logger.Info($"Stock code: {StockCodeList.Count}, Index code: {IndexCodeList.Count}");

                #region Codemap Test
#if false
                Dictionary<string, object> codeMapHeader = new Dictionary<string, object>();

                Dictionary<string, object> marketCodeMapHeader = new Dictionary<string, object>();
                codeMapHeader.Add("MaketType", marketCodeMapHeader);

                foreach (KeyValuePair<string, CodeEntity> codeEntity in codeList)
                {
                    if (codeEntity.Value.MarketType != MarketTypes.INDEX &&
                        codeEntity.Value.MarketType != MarketTypes.ELW)
                    {
                        if (!marketCodeMapHeader.ContainsKey(codeEntity.Value.MarketType.ToString()))
                        {
                            marketCodeMapHeader.Add(codeEntity.Value.MarketType.ToString(), new List<string>());
                        }

                        List<string> marketHeader = (List<string>)marketCodeMapHeader[codeEntity.Value.MarketType.ToString()];
                        marketHeader.Add(codeEntity.Value.Code);
                    }
                }

                codeMapHeader.Add("DaishinTheme", contract.Callback.GetThemeList());

                using (StreamWriter stream = File.CreateText("CodeMap.json"))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.NullValueHandling = NullValueHandling.Ignore;
                    serializer.Formatting = Formatting.Indented;
                    serializer.Serialize(stream, codeMapHeader);
                }

                Dictionary<string, object>  deserialized = JsonConvert.DeserializeObject<Dictionary<string, object>>(File.ReadAllText("CodeMap.json"), new JsonSerializerSettings
                {
                    Error = (sender, args) =>
                    {
                        logger.Error($"Configuration deserialize error, {args.ErrorContext.Error.Message}");
                        args.ErrorContext.Handled = true;
                    }
                });
#endif
                #endregion
            }
            catch (Exception ex)
            {
                logger.Error(ex);
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
                logger.Info(RealTimeState);

                var workDate = contract.Callback.GetMarketInfo(MarketInfoTypes.WorkDate);
                logger.Info($"Market work date: {workDate}");

                int workDateTime = 0;
                if (int.TryParse(workDate, out workDateTime) == true)
                {
                    int year = workDateTime / 10000;
                    int month = workDateTime % 10000 / 100;
                    int day = workDateTime % 100;

                    var now = DateTime.Now;
                    if (now.Year == year && now.Month == month && now.Day == day)
                    {
                        logger.Info("Market opened");
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

#if DEBUG
            return true;
#else
            logger.Info("Market closed");
            return false;
#endif
        }

        private void CheckMarketTime(PublisherContract contract)
        {
            try
            {
                RealTimeState = "Check market time";
                logger.Info(RealTimeState);

                var now = DateTime.Now;

                // Market start time
                var startTimeStr = contract.Callback.GetMarketInfo(MarketInfoTypes.StartTime);
                int startTime = 0;

                if (int.TryParse(startTimeStr, out startTime) == true)
                {
                    MarketStartTime = new DateTime(now.Year, now.Month, now.Day, startTime, 0, 0);
                }
                else
                {
                    logger.Error("Market start time parsing error");
                    MarketStartTime = new DateTime(now.Year, now.Month, now.Day, 9, 0, 0);
                }

                logger.Info($"Market start time: {MarketStartTime.ToString(Config.General.TimeFormat)}");

                // Market end time
                var endTimeStr = contract.Callback.GetMarketInfo(MarketInfoTypes.EndTime);
                var endTime = 0;

                if (int.TryParse(endTimeStr, out endTime) == true)
                {
                    MarketEndTime = new DateTime(now.Year, now.Month, now.Day, endTime, 0, 0).AddHours(3); // 시간외 3시간 추가
                }
                else
                {
                    logger.Error("Market end time parsing error");
                    MarketEndTime = new DateTime(now.Year, now.Month, now.Day, 15, 0, 0).AddHours(3); // 시간외 3시간 추가
                }

                logger.Info($"Market end time: {MarketEndTime.ToString(Config.General.TimeFormat)}");

                if (MarketEndTime > now)
                {
                    TimeSpan interval = (MarketEndTime - now).Add(TimeSpan.FromMinutes(10)); // 장종료 10분 후 프로그램 종료

                    MarketEndTimer = new System.Timers.Timer();
                    MarketEndTimer.Interval = interval.TotalMilliseconds;
                    MarketEndTimer.Elapsed += MarketEndTimer_Elapsed;
                    MarketEndTimer.AutoReset = false;
                    MarketEndTimer.Start();

                    logger.Info($"Market end timer will be triggered after {interval.Hours}:{interval.Minutes}:{interval.Seconds}");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public void UnregisterContract(Guid clientId)
        {
            try
            {
                if (PublisherContracts.ContainsKey(clientId) == true)
                {
                    PublisherContract temp;
                    PublisherContracts.TryRemove(clientId, out temp);

                    logger.Info($"{clientId} / {temp.Type} contract unregistered");
                }
                else
                {
                    logger.Warn($"{clientId} contract not exist");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private void StartCodeDistributing()
        {
            RealTimeState = "Start code distributing";
            logger.Info(RealTimeState);


#if EVENLY_DISTRIBUTION
            DistributeConclusionAndBiddingSubscribingCode();
#else
            DistributeStockConclusionSubscribingCode();
            DistributeIndexConclusionSubscribingCode();
            if (Config.General.SkipBiddingPrice == false)
                DistributeBiddingSubscribingCode(); 
#endif
            DistributeCircuitBreakSubscribingCode();
        }

#if EVENLY_DISTRIBUTION
        private void DistributeConclusionAndBiddingSubscribingCode()
        {
            try
            {
                RealTimeState = "Conclusion and Bidding code distribution, Start";
                logger.Info(RealTimeState);

                for (int i = 0; i < StockCodeList.Count; i++)
                {
                    var codeEntiry = StockCodeList.Values.ElementAt(i);
                    var code = CodeEntity.ConvertToDaishinCode(codeEntiry);
                    var contract = DaishinContracts[i % DaishinContracts.Count];

                    if (contract.Callback.SubscribeStock(code) == false)
                        logger.Error($"Stock conclusioin code distribution fail. code: {code}");

                    if (Config.General.SkipBiddingPrice == false)
                    {
                        if (contract.Callback.SubscribeBidding(code) == false)
                            logger.Error($"Bidding code distribution fail. code: {code}");
                    }
                }

                for (int i = 0; i < IndexCodeList.Count; i++)
                {
                    var codeEntiry = IndexCodeList.Values.ElementAt(i);
                    var code = CodeEntity.ConvertToDaishinCode(codeEntiry);
                    var contract = DaishinContracts[i % DaishinContracts.Count];

                    if (contract.Callback.SubscribeIndex(code) == false)
                        logger.Error($"Index conclusioin code distribution fail. code: {code}");
                }

                RealTimeState = "Conclusion and Bidding code distribution, Done";
                logger.Info(RealTimeState);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
#else
        private void DistributeBiddingSubscribingCode()
        {
            RealTimeState = "Bidding code distribution, Start";
            logger.Info(RealTimeState);

            int index = 0;

            foreach (var contract in DaishinContracts)
            {
                while (true)
                {
                    try
                    {
                        if (StockCodeList.Count > index &&
                            contract.Callback.IsSubscribable() == true)
                        {
                            var codeEntity = StockCodeList.Values.ElementAt(index);
                            var code = codeEntity.Code;

                            if (contract.Type == ProcessTypes.Daishin)
                                code = CodeEntity.ConvertToDaishinCode(codeEntity);

                            if (contract.Callback.SubscribeBidding(code) == true)
                                index++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                        break;
                    }
                }
            }

            if (StockCodeList.Count == index)
            {
                RealTimeState = "Bidding code distribution, Done";
                logger.Info(RealTimeState);
            }
            else
            {
                RealTimeState = "Bidding code distribution, Fail";
                logger.Error(RealTimeState);
            }
        }

        private void DistributeStockConclusionSubscribingCode()
        {
            RealTimeState = "Stock conclusion code distribution, Start";
            logger.Info(RealTimeState);

            int index = 0;

            foreach (var contract in DaishinContracts)
            {
                while (true)
                {
                    try
                    {
                        if (StockCodeList.Count > index &&
                            contract.Callback.IsSubscribable() == true)
                        {
                            var codeEntity = StockCodeList.Values.ElementAt(index);
                            var code = codeEntity.Code;

                            if (contract.Type == ProcessTypes.Daishin)
                                code = CodeEntity.ConvertToDaishinCode(codeEntity);

                            if (contract.Callback.SubscribeStock(code) == true)
                                index++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                        break;
                    }
                }
            }

            if (StockCodeList.Count == index)
            {
                RealTimeState = "Stock code distribution, Done";
                logger.Info(RealTimeState);
            }
            else
            {
                RealTimeState = "Stock code distribution, Fail";
                logger.Error(RealTimeState);
            }
        }

        private void DistributeIndexConclusionSubscribingCode()
        {
            RealTimeState = "Index conclusion code distribution, Start";
            logger.Info(RealTimeState);

            int index = 0;

            foreach (var contract in DaishinContracts)
            {
                while (true)
                {
                    try
                    {
                        if (IndexCodeList.Count > index &&
                            contract.Callback.IsSubscribable() == true)
                        {
                            var codeEntity = IndexCodeList.Values.ElementAt(index);
                            var code = codeEntity.Code;

                            if (contract.Type == ProcessTypes.Daishin)
                                code = CodeEntity.ConvertToDaishinCode(codeEntity);

                            if (contract.Callback.SubscribeIndex(code) == true)
                                index++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                        break;
                    }
                }
            }

            if (IndexCodeList.Count == index)
            {
                RealTimeState = "Index code distribution, Done";
                logger.Info(RealTimeState);
            }
            else
            {
                RealTimeState = "Index code distribution, Fail";
                logger.Error(RealTimeState);
            }
        } 
#endif

        private void DistributeCircuitBreakSubscribingCode()
        {
            RealTimeState = "Circuite break code distribution, Start";
            logger.Info(RealTimeState);

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
                    logger.Error($"Circuite break code distribution fail. code: {code}");
                    i--;

                    errorCnt++;
                    if (errorCnt > 10)
                        break;
                }
            }

            if (errorCnt > 10)
            {
                RealTimeState = $"Circuite break code distribution, Fail. error count: {errorCnt}";
                logger.Error(RealTimeState);
            }
            else
            {
                RealTimeState = "Circuite break code distribution, Done";
                logger.Info(RealTimeState);
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
    }
}
