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
            get { return PublishContracts.Values.Where(c => c.Type == PublisherType.Daishin).ToList(); }
        }

        private PublishContract DaishinContractForMastering
        {
            get { return PublishContracts.Values.FirstOrDefault(c => c.Type == PublisherType.Daishin && c.NowOperating == false); }
        }
        #endregion

        #region Ebest
        private List<PublishContract> EbestContracts
        {
            get { return PublishContracts.Values.Where(c => c.Type == PublisherType.Ebest).ToList(); }
        }

        private PublishContract EbestContractForMastering
        {
            get { return PublishContracts.Values.FirstOrDefault(c => c.Type == PublisherType.Ebest && c.NowOperating == false); }
        }
        #endregion

        #region Krx
        private List<PublishContract> KrxContracts
        {
            get { return PublishContracts.Values.Where(c => c.Type == PublisherType.Krx).ToList(); }
        }

        private PublishContract KrxContractForMastering
        {
            get { return PublishContracts.Values.FirstOrDefault(c => c.Type == PublisherType.Krx && c.NowOperating == false); }
        }
        #endregion

        #region Naver
        private List<PublishContract> NaverContracts
        {
            get { return PublishContracts.Values.Where(c => c.Type == PublisherType.Naver).ToList(); }
        }

        private PublishContract NaverContractForMastering
        {
            get { return PublishContracts.Values.FirstOrDefault(c => c.Type == PublisherType.Naver && c.NowOperating == false); }
        }
        #endregion
        #endregion

        public void LaunchPublisher(PublisherType type)
        {
            try
            {
                var windowStyle = ProcessWindowStyle.Minimized;

                switch (type)
                {
                    case PublisherType.Daishin:
                    case PublisherType.DaishinMaster:
                        ProcessUtility.Start("MTree.DaishinPublisher.exe", type.ToString(), windowStyle);
                        break;

                    case PublisherType.Ebest:
                        ProcessUtility.Start("MTree.EbestPublisher.exe", type.ToString(), windowStyle);
                        break;

                    case PublisherType.Krx:
                        ProcessUtility.Start("MTree.KrxPublisher.exe", type.ToString(), windowStyle);
                        break;

                    case PublisherType.Naver:
                        ProcessUtility.Start("MTree.NaverPublisher.exe", type.ToString(), windowStyle);
                        break;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private void LaunchPublisherAll()
        {
            try
            {
                // Daishin
                int daishinProcessCount = StockCodeList.Count / 200 + 1; // TODO : BiddingPrice 때문에 더 띄워야하나?
                daishinProcessCount = 3;

                for (int i = 0; i < daishinProcessCount; i++)
                    LaunchPublisher(PublisherType.Daishin);

                // Ebest
                int ebestProcessCount = 3;

                for (int i = 0; i < ebestProcessCount; i++)
                    LaunchPublisher(PublisherType.Ebest);

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
                    logger.Error($"{contract.Type} contract exist / {clientId}");
                }
                else
                {
                    if (contract.Type == PublisherType.None)
                    {
                        logger.Info($"{contract.Type} wrong contract type / {clientId}");
                    }
                    else
                    {
                        bool isDaishinMaster = contract.Type == PublisherType.DaishinMaster;
                        if (contract.Type == PublisherType.DaishinMaster)
                            contract.Type = PublisherType.Daishin;

                        contract.Callback = OperationContext.Current.GetCallbackChannel<IRealTimePublisherCallback>();
                        PublishContracts.TryAdd(clientId, contract);

                        logger.Info($"{contract.Type} contract registered / {clientId}");

                        if (isDaishinMaster)
                        {
                            StockCodeList = contract.Callback.GetStockCodeList();

                            if (StockCodeList != null)
                            {
                                logger.Info($"Stock code list count: {StockCodeList.Count}");
                                StartStockMastering();
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
            Task.Run(() =>
            {
                logger.Info("Subscribe code distribution, Start");

                int index = 0;

                foreach (var contract in DaishinContracts)
                {
                    try
                    {
                        if (contract.Callback.IsSubscribable() == true)
                        {
                            if (contract.Callback.SubscribeStock(StockCodeList[index]) == true)
                                index++;
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                    }
                }

                if (StockCodeList.Count == index)
                    logger.Info("Subscribe code distribution, Done");
                else
                    logger.Error("Subscribe code distribution, Fail");

                Debugger.Break(); // 테스트 용도
            });
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
