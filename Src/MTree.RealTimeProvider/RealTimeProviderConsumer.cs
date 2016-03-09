using MTree.DataStructure;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MTree.RealTimeProvider
{
    public partial class RealTimeProvider
    {
        private ConcurrentDictionary<Guid, SubscribeContract> BiddingPriceContracts { get; set; } = new ConcurrentDictionary<Guid, SubscribeContract>();
        private ConcurrentDictionary<Guid, SubscribeContract> StockConclusionContracts { get; set; } = new ConcurrentDictionary<Guid, SubscribeContract>();
        private ConcurrentDictionary<Guid, SubscribeContract> IndexConclusionContracts { get; set; } = new ConcurrentDictionary<Guid, SubscribeContract>();
        
        public void RegisterSubscribeContract(Guid clientId, SubscribeContract contract)
        {
            try
            {
                var contractList = GetSubscriptionList(contract.Type);
                if (contractList == null) return;

                if (contractList.ContainsKey(clientId) == true)
                {
                    logger.Error($"{clientId} / {contract.Type} contract exist");
                }
                else
                {
                    contract.Callback = OperationContext.Current.GetCallbackChannel<IRealTimeConsumerCallback>();

                    if (contractList.TryAdd(clientId, contract) == true)
                        logger.Info($"{clientId} / {contract.Type} contract registered");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public void UnregisterSubscribeContractAll(Guid clientId)
        {
            UnregisterSubscribeContract(clientId, SubscribeType.BiddingPrice);
            UnregisterSubscribeContract(clientId, SubscribeType.StockConclusion);
            UnregisterSubscribeContract(clientId, SubscribeType.IndexConclusion);
        }

        public void UnregisterSubscribeContract(Guid clientId, SubscribeType type)
        {
            try
            {
                var contractList = GetSubscriptionList(type);
                if (contractList == null) return;

                if (contractList.ContainsKey(clientId) == true)
                {
                    SubscribeContract temp;
                    if (contractList.TryRemove(clientId, out temp) == true)
                    {
                        temp.Callback = null;
                        logger.Info($"{clientId} / {type} contract unregistered");
                    }
                }
                else
                {
                    logger.Info($"{clientId} / {type} contract not exist");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private ConcurrentDictionary<Guid, SubscribeContract> GetSubscriptionList(SubscribeType type)
        {
            switch (type)
            {
                case SubscribeType.BiddingPrice:
                    return BiddingPriceContracts;
                case SubscribeType.StockConclusion:
                    return StockConclusionContracts;
                case SubscribeType.IndexConclusion:
                    return IndexConclusionContracts;
                default:
                    return null;
            }
        }
    }
}
