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
            get { return PublishContracts.Values.FirstOrDefault(c => c.Type == ProcessTypes.Daishin && c.NowOperating == false); }
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
            get { return PublishContracts.Values.FirstOrDefault(c => c.Type == ProcessTypes.Ebest && c.NowOperating == false); }
        }
        #endregion

        #region Kiwoom
        private List<PublishContract> KiwoomContracts
        {
            get { return PublishContracts.Values.Where(c => c.Type == ProcessTypes.Kiwoom).ToList(); }
        }

        private PublishContract KiwoomContractForMastering
        {
            get { return PublishContracts.Values.FirstOrDefault(c => c.Type == ProcessTypes.Kiwoom && c.NowOperating == false); }
        }
        #endregion

        #region Krx
        private List<PublishContract> KrxContracts
        {
            get { return PublishContracts.Values.Where(c => c.Type == ProcessTypes.Krx).ToList(); }
        }

        private PublishContract KrxContractForMastering
        {
            get { return PublishContracts.Values.FirstOrDefault(c => c.Type == ProcessTypes.Krx && c.NowOperating == false); }
        }
        #endregion

        #region Naver
        private List<PublishContract> NaverContracts
        {
            get { return PublishContracts.Values.Where(c => c.Type == ProcessTypes.Naver).ToList(); }
        }

        private PublishContract NaverContractForMastering
        {
            get { return PublishContracts.Values.FirstOrDefault(c => c.Type == ProcessTypes.Naver && c.NowOperating == false); }
        }
        #endregion
        #endregion

        private void LaunchClientProcess()
        {
            try
            {
#if true // HistorySaver
                ProcessUtility.Start(ProcessTypes.HistorySaver);
#endif


#if true // Kiwoom

                if (Config.Instance.General.SkipMastering == false)
                    ProcessUtility.Start(ProcessTypes.Kiwoom);
#endif


#if true // Daishin popup stopper
                ProcessUtility.Start(ProcessTypes.DaishinPopupStopper);
#endif


#if true // Daishin
                int daishinProcessCount;
                if (Config.Instance.General.SkipBiddingPrice == true)
                    daishinProcessCount = (StockCodeList.Count + IndexCodeList.Count) / 400;
                else
                    daishinProcessCount = (StockCodeList.Count * 2 + IndexCodeList.Count) / 400;

                for (int i = 0; i < daishinProcessCount; i++)
                    ProcessUtility.Start(ProcessTypes.Daishin);
#endif


#if true // Ebest
                int ebestProcessCount = 3;
                for (int i = 0; i < ebestProcessCount; i++)
                    ProcessUtility.Start(ProcessTypes.Ebest);
#endif
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
                        bool isMasterProcess = (contract.Type == ProcessTypes.DaishinMaster);
                        if (contract.Type == ProcessTypes.DaishinMaster) contract.Type = ProcessTypes.Daishin;

                        PublishContracts.TryAdd(clientId, contract);
                        logger.Info($"{contract.ToString()} contract registered / {clientId}");

                        if (isMasterProcess == true)
                        {
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

                            contract.Callback.NotifyMessage(MessageTypes.MarketStartTime, string.Empty);
                            contract.Callback.NotifyMessage(MessageTypes.MarketEndTime, string.Empty);

                            LaunchClientProcess();

                            if (Config.Instance.General.SkipMastering == true)
                            {
                                Task.Run(() =>
                                {
                                    Thread.Sleep(1000 * 60);
                                    StartCodeDistributing();
                                });
                            }
                            else
                            {
                                Task.Run(() => StartStockMastering());
                            }
                        }
                    }
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
            if (Config.Instance.General.SkipBiddingPrice == false)
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
            //logger.Warn($"Circuit break!!!!!, {circuitBreak.ToString()}");
            ProcessCircuitBreak(circuitBreak);
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
