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
            get { return PublishContracts.Values.Where(c => c.Type == ProcessTypes.Kiwoon).ToList(); }
        }

        private PublishContract KiwoomContractForMastering
        {
            get { return PublishContracts.Values.FirstOrDefault(c => c.Type == ProcessTypes.Kiwoon && c.NowOperating == false); }
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
                // HistorySaver
                ProcessUtility.Start(ProcessTypes.HistorySaver);

                // Kiwoom
                ProcessUtility.Start(ProcessTypes.Kiwoon);

                // Daishin popup stopper
                ProcessUtility.Start(ProcessTypes.DaishinPopupStopper);

                // Daishin
                int daishinProcessCount = StockCodeList.Count * 2 / 200;
                for (int i = 0; i < daishinProcessCount; i++)
                    ProcessUtility.Start(ProcessTypes.Daishin);

                // Ebest
                int ebestProcessCount = 3;
                for (int i = 0; i < ebestProcessCount; i++)
                    ProcessUtility.Start(ProcessTypes.Ebest);

                // Krx

                // Naver
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
                    if (contract.Type == ProcessTypes.None)
                    {
                        logger.Info($"{contract.ToString()} wrong contract type / {clientId}");
                    }
                    else
                    {
                        bool isDaishinMaster = contract.Type == ProcessTypes.DaishinMaster;
                        if (contract.Type == ProcessTypes.DaishinMaster)
                            contract.Type = ProcessTypes.Daishin;

                        contract.Id = PublishContract.IdNumbering++;
                        contract.Callback = OperationContext.Current.GetCallbackChannel<IRealTimePublisherCallback>();
                        PublishContracts.TryAdd(clientId, contract);

                        logger.Info($"{contract.ToString()} contract registered / {clientId}");

                        if (isDaishinMaster)
                        {
                            StockCodeList = contract.Callback.GetStockCodeList();

                            if (StockCodeList != null)
                            {
                                logger.Info($"Stock code list count: {StockCodeList.Count}");
                                Task.Run(() => StartStockMastering());
                            }
                            else
                            {
                                logger.Error("Stock code list gathering failed");
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
            DistributeStockCode();
            DistributeIndexCode();
            DistributeBiddingCode();
        }

        private void DistributeBiddingCode()
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

        private void DistributeStockCode()
        {
            logger.Info("Stock code distribution, Start");

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

        private void DistributeIndexCode()
        {
            logger.Info("Index code distribution, Start");
        }

        public void PublishBiddingPrice(BiddingPrice biddingPrice)
        {
            BiddingPriceQueue.Enqueue(biddingPrice);
        }

        public void PublishCircuitBreak(CircuitBreak circuitBreak)
        {
            logger.Warn($"Circuit break!!!!!, {circuitBreak.ToString()}");
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
