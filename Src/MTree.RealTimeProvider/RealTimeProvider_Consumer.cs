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
        #region Contracts
        private ConcurrentDictionary<Guid, SubscribeContract> ConsumerContracts { get; set; } = new ConcurrentDictionary<Guid, SubscribeContract>();
        private ConcurrentDictionary<Guid, SubscribeContract> StockMasterContracts { get; set; } = new ConcurrentDictionary<Guid, SubscribeContract>();
        private ConcurrentDictionary<Guid, SubscribeContract> BiddingPriceContracts { get; set; } = new ConcurrentDictionary<Guid, SubscribeContract>();
        private ConcurrentDictionary<Guid, SubscribeContract> CircuitBreakContracts { get; set; } = new ConcurrentDictionary<Guid, SubscribeContract>();
        private ConcurrentDictionary<Guid, SubscribeContract> StockConclusionContracts { get; set; } = new ConcurrentDictionary<Guid, SubscribeContract>();
        private ConcurrentDictionary<Guid, SubscribeContract> IndexConclusionContracts { get; set; } = new ConcurrentDictionary<Guid, SubscribeContract>();
        #endregion

        public void RegisterContract(Guid clientId, SubscribeContract contract)
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
                    logger.Error($"{contract.ToString()} contract exist / {clientId}");
                }
                else
                {
                    if (contractList.TryAdd(clientId, contract) == true)
                        logger.Info($"{contract.ToString()} contract registered / {clientId}");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public void UnregisterContractAll(Guid clientId)
        {
            UnregisterContract(clientId, SubscribeTypes.StockMaster);
            UnregisterContract(clientId, SubscribeTypes.BiddingPrice);
            UnregisterContract(clientId, SubscribeTypes.CircuitBreak);
            UnregisterContract(clientId, SubscribeTypes.StockConclusion);
            UnregisterContract(clientId, SubscribeTypes.IndexConclusion);
        }

        public void UnregisterContract(Guid clientId, SubscribeTypes type)
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

                if (StockMasterContracts.ContainsKey(clientId) == false &&
                    BiddingPriceContracts.ContainsKey(clientId) == false &&
                    CircuitBreakContracts.ContainsKey(clientId) == false &&
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

        private ConcurrentDictionary<Guid, SubscribeContract> GetSubscriptionList(SubscribeTypes type)
        {
            switch (type)
            {
                case SubscribeTypes.StockMaster:        return StockMasterContracts;
                case SubscribeTypes.BiddingPrice:       return BiddingPriceContracts;
                case SubscribeTypes.CircuitBreak:       return CircuitBreakContracts;
                case SubscribeTypes.StockConclusion:    return StockConclusionContracts;
                case SubscribeTypes.IndexConclusion:    return IndexConclusionContracts;
                default:                                return null;
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
                        if (contract.Value.Scope == SubscribeScopes.All ||
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

        private void ProcessCircuitBreakQueue()
        {
            try
            {
                CircuitBreak circuitBreak;
                if (CircuitBreakQueue.TryDequeue(out circuitBreak) == true)
                {
                    foreach (var contract in CircuitBreakContracts)
                    {
                        if (contract.Value.Scope == SubscribeScopes.All ||
                            contract.Value.ContainCode(circuitBreak.Code) == true)
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
                        if (contract.Value.Scope == SubscribeScopes.All ||
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
                        if (contract.Value.Scope == SubscribeScopes.All ||
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

        private void ProcessStockMaster(StockMaster stockMaster)
        {
            foreach (var contract in StockMasterContracts)
            {
                try
                {
                    contract.Value.Callback.ConsumeStockMaster(stockMaster);
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }
            }
        }
    }
}
