using MTree.DataStructure;
using MTree.Utility;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MTree.RealTimeProvider
{
    public partial class RealTimeProvider
    {
        private ConcurrentDictionary<Guid, PublishContract> PublishContracts { get; set; } = new ConcurrentDictionary<Guid, PublishContract>();

        #region Contract Property
        private List<PublishContract> DaishinContract
        {
            get { return PublishContracts.Values.Where(c => c.Type == PublisherType.Daishin).ToList(); }
        }

        private List<PublishContract> EbestContract
        {
            get { return PublishContracts.Values.Where(c => c.Type == PublisherType.Ebest).ToList(); }
        }

        private List<PublishContract> KrxContract
        {
            get { return PublishContracts.Values.Where(c => c.Type == PublisherType.Krx).ToList(); }
        }

        private List<PublishContract> NaverContract
        {
            get { return PublishContracts.Values.Where(c => c.Type == PublisherType.Naver).ToList(); }
        }
        #endregion

        public void LaunchPublisher(PublisherType type)
        {
            try
            {
                var windowStyle = ProcessWindowStyle.Minimized;

                switch (type)
                {
                    case PublisherType.Daishin:
                        ProcessUtility.Start("MTree.DaishinPublisher.exe", type.ToString(), windowStyle);
                        break;
                    case PublisherType.Ebest:
                        ProcessUtility.Start("MTree.EbestPublisher.exe", type.ToString(), windowStyle);
                        break;
                    case PublisherType.Krx:
                        ProcessUtility.Start("MTree.KrxPublisher.exe", type.ToString(), windowStyle);
                        break;
                }
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
                    logger.Error($"{clientId} / {contract.Type} contract exist");
                }
                else
                {
                    if (contract.Type == PublisherType.None)
                    {
                        logger.Info($"{clientId} / {contract.Type} wrong contract type");
                    }
                    else
                    {
                        contract.Callback = OperationContext.Current.GetCallbackChannel<IRealTimePublisherCallback>();
                        PublishContracts.TryAdd(clientId, contract);

                        logger.Info($"{clientId} / {contract.Type} contract registered");
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

        public void PublishBiddingPrice(BiddingPrice biddingPrice)
        {
            BiddingPriceQueue.Enqueue(biddingPrice);
        }

        public void PublishCircuitBreak(CircuitBreak circuitBreak)
        {
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
