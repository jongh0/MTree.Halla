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
            get { return PublishContracts.Values.Where(c => c.Type == ProcessType.Daishin).ToList(); }
        }

        private PublishContract DaishinContractForMastering
        {
            get { return PublishContracts.Values.FirstOrDefault(c => c.Type == ProcessType.Daishin && c.NowOperating == false); }
        }
        #endregion

        #region Ebest
        private List<PublishContract> EbestContracts
        {
            get { return PublishContracts.Values.Where(c => c.Type == ProcessType.Ebest).ToList(); }
        }

        private PublishContract EbestContractForMastering
        {
            get { return PublishContracts.Values.FirstOrDefault(c => c.Type == ProcessType.Ebest && c.NowOperating == false); }
        }
        #endregion

        #region Krx
        private List<PublishContract> KrxContracts
        {
            get { return PublishContracts.Values.Where(c => c.Type == ProcessType.Krx).ToList(); }
        }

        private PublishContract KrxContractForMastering
        {
            get { return PublishContracts.Values.FirstOrDefault(c => c.Type == ProcessType.Krx && c.NowOperating == false); }
        }
        #endregion

        #region Naver
        private List<PublishContract> NaverContracts
        {
            get { return PublishContracts.Values.Where(c => c.Type == ProcessType.Naver).ToList(); }
        }

        private PublishContract NaverContractForMastering
        {
            get { return PublishContracts.Values.FirstOrDefault(c => c.Type == ProcessType.Naver && c.NowOperating == false); }
        }
        #endregion
        #endregion

        private void LaunchPublisherAll()
        {
            try
            {
                // Daishin popup stopper
                ProcessUtility.Start(ProcessType.DaishinPopupStopper);

                // Daishin
                int daishinProcessCount = StockCodeList.Count / 200 + 1; // TODO : BiddingPrice 때문에 더 띄워야하나?
                //daishinProcessCount = 3;

                for (int i = 0; i < daishinProcessCount; i++)
                    ProcessUtility.Start(ProcessType.Daishin);

                // Ebest
                int ebestProcessCount = 3;

                for (int i = 0; i < ebestProcessCount; i++)
                    ProcessUtility.Start(ProcessType.Ebest);

                // Krx

                // Naver
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public void RegisterPublishContract(Guid clientId, PublishContract contract)
        {
            try
            {
                if (PublishContracts.ContainsKey(clientId) == true)
                {
                    logger.Error($"{contract.ToString()} contract exist / {clientId}");
                }
                else
                {
                    if (contract.Type == ProcessType.None)
                    {
                        logger.Info($"{contract.ToString()} wrong contract type / {clientId}");
                    }
                    else
                    {
                        bool isDaishinMaster = contract.Type == ProcessType.DaishinMaster;
                        if (contract.Type == ProcessType.DaishinMaster)
                            contract.Type = ProcessType.Daishin;

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

        public void UnregisterPublishContract(Guid clientId)
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

        private void DistributeSubscribeCode()
        {
            // TODO : index code 나눠주는 코드 추가해야함
            logger.Info("Subscribe code distribution, Start");

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
                            if (contract.Callback.SubscribeStock(StockCodeList.Keys.ElementAt(index)) == true)
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
                logger.Info("Subscribe code distribution, Done");
            else
                logger.Error("Subscribe code distribution, Fail");

            Debugger.Break(); // 테스트 용도
        }

        public void PublishBiddingPrice(BiddingPrice biddingPrice)
        {
            BiddingPriceQueue.Enqueue(biddingPrice);
        }

        public void PublishCircuitBreak(CircuitBreak circuitBreak)
        {
            logger.Info($"Circuit break!!!!!, {circuitBreak.ToString()}");
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
