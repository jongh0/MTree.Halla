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
        private PublishContract DaisinStockMasterContract
        {
            get { return PublishContracts.Values.FirstOrDefault(c => c.Type == PublishType.DaisinStockMaster); }
        }

        private PublishContract EbestStockMasterContract
        {
            get { return PublishContracts.Values.FirstOrDefault(c => c.Type == PublishType.EbestStockMaster); }
        }

        private PublishContract KrxStockMasterContract
        {
            get { return PublishContracts.Values.FirstOrDefault(c => c.Type == PublishType.KrxStockMaster); }
        }

        private PublishContract EbestIndexMasterContract
        {
            get { return PublishContracts.Values.FirstOrDefault(c => c.Type == PublishType.EbestIndexMaster); }
        }

        private List<PublishContract> DaishinStockConclusionContract
        {
            get { return PublishContracts.Values.Where(c => c.Type == PublishType.DaishinStockConclusion).ToList(); }
        }

        private List<PublishContract> EbestIndexConclusionContract
        {
            get { return PublishContracts.Values.Where(c => c.Type == PublishType.EbestIndexConclusion).ToList(); }
        } 
        #endregion

        public void LaunchPublisher(PublishType type)
        {
            try
            {
                switch (type)
                {
                    case PublishType.DaisinStockMaster:
                    case PublishType.DaishinStockConclusion:
                        ProcessUtility.Start("MTree.DaishinPublisher.exe", type.ToString());
                        break;

                    case PublishType.EbestStockMaster:
                    case PublishType.EbestIndexMaster:
                    case PublishType.EbestIndexConclusion:
                        ProcessUtility.Start("MTree.EbestPublisher.exe", type.ToString());
                        break;

                    case PublishType.KrxStockMaster:
                        ProcessUtility.Start("MTree.KrxPublisher.exe", type.ToString());
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
                    contract.Callback = OperationContext.Current.GetCallbackChannel<IRealTimePublisherCallback>();
                    PublishContracts.TryAdd(clientId, contract);

                    logger.Info($"{clientId} contract registered");
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

                    logger.Info($"{clientId} contract unregistered");
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
