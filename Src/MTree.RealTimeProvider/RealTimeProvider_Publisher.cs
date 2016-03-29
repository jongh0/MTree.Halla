using MTree.Configuration;
using MTree.DataStructure;
using MTree.Utility;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTree.RealTimeProvider
{
    public partial class RealTimeProvider
    {
        private ConcurrentDictionary<Guid, PublishContract> PublishContracts { get; set; } = new ConcurrentDictionary<Guid, PublishContract>();

        #region Contract Property
        #region Daishin
        private List<PublishContract> DaishinContracts
        {
            get { return PublishContracts.Values.Where(c => c.Type == ProcessTypes.Daishin).ToList(); }
        }

        private PublishContract DaishinContractForMastering
        {
            get { return PublishContracts.Values.FirstOrDefault(c => c.Type == ProcessTypes.Daishin && c.IsOperating == false); }
        }

        private PublishContract DaishinContractMaster
        {
            get { return PublishContracts.Values.FirstOrDefault(c => c.Type == ProcessTypes.DaishinMaster); }
        }
        #endregion

        #region Ebest
        private List<PublishContract> EbestContracts
        {
            get { return PublishContracts.Values.Where(c => c.Type == ProcessTypes.Ebest).ToList(); }
        }

        private PublishContract EbestContractForMastering
        {
            get { return PublishContracts.Values.FirstOrDefault(c => c.Type == ProcessTypes.Ebest && c.IsOperating == false); }
        }
        #endregion

        #region Kiwoom
        private List<PublishContract> KiwoomContracts
        {
            get { return PublishContracts.Values.Where(c => c.Type == ProcessTypes.Kiwoom).ToList(); }
        }

        private PublishContract KiwoomContractForMastering
        {
            get { return PublishContracts.Values.FirstOrDefault(c => c.Type == ProcessTypes.Kiwoom && c.IsOperating == false); }
        }
        #endregion

        #region Krx
        private List<PublishContract> KrxContracts
        {
            get { return PublishContracts.Values.Where(c => c.Type == ProcessTypes.Krx).ToList(); }
        }

        private PublishContract KrxContractForMastering
        {
            get { return PublishContracts.Values.FirstOrDefault(c => c.Type == ProcessTypes.Krx && c.IsOperating == false); }
        }
        #endregion
        #endregion

        private void LaunchClientProcess()
        {
            try
            {
                // HistorySaver
                ProcessUtility.Start(ProcessTypes.HistorySaver);

                // Dashboard
                ProcessUtility.Start(ProcessTypes.Dashboard);

                if (Config.General.SkipMastering == false)
                {
                    // Kiwoom
                    ProcessUtility.Start(ProcessTypes.Kiwoom, ProcessWindowStyle.Minimized);

                    // Daishin popup stopper
                    ProcessUtility.Start(ProcessTypes.DaishinPopupStopper, ProcessWindowStyle.Minimized);
                    ProcessUtility.Start(ProcessTypes.DaishinPopupStopper, ProcessWindowStyle.Minimized);
                }

                // Daishin
                int daishinProcessCount;
                if (Config.General.SkipBiddingPrice == true)
                    daishinProcessCount = (StockCodeList.Count + IndexCodeList.Count) / 400;
                else
                    daishinProcessCount = (StockCodeList.Count * 2 + IndexCodeList.Count) / 400;

                for (int i = 0; i < daishinProcessCount; i++)
                    ProcessUtility.Start(ProcessTypes.Daishin, ProcessWindowStyle.Minimized);

                // Ebest
                int ebestProcessCount = 3;
                for (int i = 0; i < ebestProcessCount; i++)
                    ProcessUtility.Start(ProcessTypes.Ebest, ProcessWindowStyle.Minimized);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public void RegisterContract(Guid clientId, PublishContract contract)
        {
            try
            {
                if (PublishContracts.ContainsKey(clientId) == true)
                {
                    logger.Error($"{contract.ToString()} contract exist / {clientId}");
                }
                else
                {
                    contract.Id = PublishContract.IdNumbering++;
                    contract.Callback = OperationContext.Current.GetCallbackChannel<IRealTimePublisherCallback>();

                    if (contract.Type == ProcessTypes.None)
                    {
                        PublishContracts.TryAdd(clientId, contract);
                        logger.Warn($"{contract.ToString()} contract type is not set / {clientId}");
                    }
                    else
                    {
                        bool isMasterContract = (contract.Type == ProcessTypes.DaishinMaster);
                        if (contract.Type == ProcessTypes.DaishinMaster) contract.Type = ProcessTypes.Daishin;

                        PublishContracts.TryAdd(clientId, contract);
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
        }

        /// <summary>
        /// DaishinMaster Client가 접속 시 순차적으로 Code 리스트 확인, Mastering, Code Distributing을 수행한다.
        /// 장이 열리지 않는 날이면 Program을 종료시킨다. (DEBUG Mode 제외)
        /// </summary>
        /// <param name="contract"></param>
        private void ProcessMasterContract(PublishContract contract)
        {
            try
            {
                logger.Info("Process master contrace");

                if (CheckMarketWorkDate(contract) == true)
                {
                    CheckMarketTime(contract);
                    CheckCodeList(contract);

                    LaunchClientProcess();

                    Task.Run(() =>
                    {
                        Thread.Sleep(1000 * 20);

                        if (Config.General.SkipMastering == true)
                        {
                            StartCodeDistributing();
                        }
                        else
                        {
                            StartStockMastering();
                            StartIndexMastering();
                            StartCodeDistributing();

                            ProcessUtility.Kill(ProcessTypes.DaishinPopupStopper);
                        }
                    });
                }
                else
                {
                    Task.Run(() =>
                    {
                        Thread.Sleep(1000 * 5);
                        ExitProgram();
                    });
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private void CheckCodeList(PublishContract contract)
        {
            try
            {
                logger.Info("Check code list");

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
        private bool CheckMarketWorkDate(PublishContract contract)
        {
            try
            {
                logger.Info("Check market work date");

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

        private void CheckMarketTime(PublishContract contract)
        {
            try
            {
                logger.Info("Check market time");

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
                    MarketEndTime = new DateTime(now.Year, now.Month, now.Day, endTime, 0, 0).AddHours(3); // 장외 3시간 추가
                }
                else
                {
                    logger.Error("Market end time parsing error");
                    MarketStartTime = new DateTime(now.Year, now.Month, now.Day, 15, 0, 0);
                }

                logger.Info($"Market end time: {MarketEndTime.ToString(Config.General.TimeFormat)}");

                if (MarketEndTimer != null)
                {
                    MarketEndTimer.Stop();
                    MarketEndTimer.Dispose();
                    MarketEndTimer = null;
                }

                if (MarketEndTime > now)
                {
                    TimeSpan interval = (MarketEndTime - now).Add(TimeSpan.FromHours(1)); // 장종료 1시간 후 프로그램 종료

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
                if (PublishContracts.ContainsKey(clientId) == true)
                {
                    PublishContract temp;
                    PublishContracts.TryRemove(clientId, out temp);

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
            DistributeStockConclusionSubscribingCode();
            DistributeIndexConclusionSubscribingCode();
            if (Config.General.SkipBiddingPrice == false)
                DistributeBiddingSubscribingCode();
            DistributeCircuitBreakSubscribingCode();
        }

        private void DistributeBiddingSubscribingCode()
        {
            logger.Info("Bidding code distribution, Start");

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
                logger.Info("Bidding code distribution, Done");
            else
                logger.Error("Bidding code distribution, Fail");
        }

        private void DistributeStockConclusionSubscribingCode()
        {
            logger.Info("Stock conclusion code distribution, Start");

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
                logger.Info("Stock code distribution, Done");
            else
                logger.Error("Stock code distribution, Fail");
        }

        private void DistributeIndexConclusionSubscribingCode()
        {
            logger.Info("Index conclusion code distribution, Start");
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
                logger.Info("Index code distribution, Done");
            else
                logger.Error("Index code distribution, Fail");
        }

        private void DistributeCircuitBreakSubscribingCode()
        {
            logger.Info("Circuite break code distribution, Start");
            int errorCnt = 0;
            for (int i = 0; i < StockCodeList.Count;i++)
            {
                var codeEntity = StockCodeList.Values.ElementAt(i);
                var code = codeEntity.Code;

                var contract = EbestContracts[i % EbestContracts.Count];
                if (contract.Type == ProcessTypes.Daishin)
                    code = CodeEntity.ConvertToDaishinCode(codeEntity);

                if (contract.Callback.SubscribeCircuitBreak(code) == false)
                {
                    logger.Error($"Circuite break code distribution fail. code:{code}");
                    i--;
                    errorCnt++;
                    if (errorCnt > 10)
                        break;
                }
            }
            if(errorCnt > 10)
                logger.Error($"Circuite break code distribution, Fail. error count:{errorCnt}");
            else
                logger.Info("Circuite break code distribution, Done");
            
        }

        public void PublishBiddingPrice(BiddingPrice biddingPrice)
        {
            BiddingPriceQueue.Enqueue(biddingPrice);
        }

        public void PublishCircuitBreak(CircuitBreak circuitBreak)
        {
            CircuitBreakQueue.Enqueue(circuitBreak);
        }

        public void PublishIndexConclusion(IndexConclusion conclusion)
        {
            IndexConclusionQueue.Enqueue(conclusion);
        }

        public void PublishStockConclusion(StockConclusion conclusion)
        {
            StockConclusionQueue.Enqueue(conclusion);
        }
    }
}
