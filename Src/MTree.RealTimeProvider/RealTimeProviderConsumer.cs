using MTree.DataStructure;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTree.RealTimeProvider
{
    public partial class RealTimeProvider
    {
        private ConcurrentDictionary<Guid, SubscribeContract> ConsumerContracts { get; set; } = new ConcurrentDictionary<Guid, SubscribeContract>();
        private ConcurrentDictionary<Guid, SubscribeContract> BiddingPriceContracts { get; set; } = new ConcurrentDictionary<Guid, SubscribeContract>();
        private ConcurrentDictionary<Guid, SubscribeContract> StockConclusionContracts { get; set; } = new ConcurrentDictionary<Guid, SubscribeContract>();
        private ConcurrentDictionary<Guid, SubscribeContract> IndexConclusionContracts { get; set; } = new ConcurrentDictionary<Guid, SubscribeContract>();
        
        public void RegisterSubscribeContract(Guid clientId, SubscribeContract contract)
        {
            try
            {
                contract.Callback = OperationContext.Current.GetCallbackChannel<IRealTimeConsumerCallback>();

                // 모든 Contract 저장
                if (ConsumerContracts.ContainsKey(clientId) == false)
                    ConsumerContracts.TryAdd(clientId, contract);

                // 타입별 Contract 저장
                var contractList = GetSubscriptionList(contract.Type);
                if (contractList == null) return;

                if (contractList.ContainsKey(clientId) == true)
                {
                    logger.Error($"{contract.Type} contract exist / {clientId}");
                }
                else
                {
                    if (contractList.TryAdd(clientId, contract) == true)
                        logger.Info($"{contract.Type} contract registered / {clientId}");
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

                if (BiddingPriceContracts.ContainsKey(clientId) == false &&
                    StockConclusionContracts.ContainsKey(clientId) == false &&
                    IndexConclusionContracts.ContainsKey(clientId) == false)
                {
                    SubscribeContract temp;
                    if (ConsumerContracts.TryRemove(clientId, out temp) == true)
                    {
                        temp.Callback = null;
                        logger.Info($"{clientId} / {type} contract unregistered");
                    }
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

        private void ProcessBiddingPriceQueue()
        {
            try
            {
                BiddingPrice biddingPrice;
                if (BiddingPriceQueue.TryDequeue(out biddingPrice) == true)
                {
                    foreach (var contract in BiddingPriceContracts)
                    {
                        if (contract.Value.Scope == SubscribeScope.All ||
                            contract.Value.ContainCode(biddingPrice.Code) == true)
                        {
                            try
                            {
                                contract.Value.Callback.ConsumeBiddingPrice(biddingPrice);
                            }
                            catch (Exception ex)
                            {
                                logger.Error(ex);
                            }
                        }
                    }
                }
                else
                {
                    Thread.Sleep(10);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private void ProcessStockConclusionQueue()
        {
            try
            {
                StockConclusion conclusion;
                if (StockConclusionQueue.TryDequeue(out conclusion) == true)
                {
                    foreach (var contract in StockConclusionContracts)
                    {
                        if (contract.Value.Scope == SubscribeScope.All ||
                            contract.Value.ContainCode(conclusion.Code) == true)
                        {
                            try
                            {
                                contract.Value.Callback.ConsumeStockConclusion(conclusion);
                            }
                            catch (Exception ex)
                            {
                                logger.Error(ex);
                            }
                        }
                    }
                }
                else
                {
                    Thread.Sleep(10);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private void ProcessIndexConclusionQueue()
        {
            try
            {
                IndexConclusion conclusion;
                if (IndexConclusionQueue.TryDequeue(out conclusion) == true)
                {
                    foreach (var contract in IndexConclusionContracts)
                    {
                        if (contract.Value.Scope == SubscribeScope.All ||
                            contract.Value.ContainCode(conclusion.Code) == true)
                        {
                            try
                            {
                                contract.Value.Callback.ConsumeIndexConclusion(conclusion);
                            }
                            catch (Exception ex)
                            {
                                logger.Error(ex);
                            }
                        }
                    }
                }
                else
                {
                    Thread.Sleep(10);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private void ProcessCircuitBreak(CircuitBreak circuitBreak)
        {
            foreach (var contract in ConsumerContracts)
            {
                try
                {
                    contract.Value.Callback.ConsumeCircuitBreak(circuitBreak);
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }
            }
        }
    }
}
