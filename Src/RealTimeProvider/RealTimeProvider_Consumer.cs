using DataStructure;
using CommonLib;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RealTimeProvider
{
    public partial class RealTimeProvider_
    {
        #region Contracts
        public int ConsumerContractCount { get { return ConsumerContracts.Count; } }
        private ConcurrentDictionary<Guid, SubscribeContract> ConsumerContracts { get; set; } = new ConcurrentDictionary<Guid, SubscribeContract>();
        private ConcurrentDictionary<Guid, SubscribeContract> MasteringContracts { get; set; } = new ConcurrentDictionary<Guid, SubscribeContract>();
        private ConcurrentDictionary<Guid, SubscribeContract> ChartContracts { get; set; } = new ConcurrentDictionary<Guid, SubscribeContract>();
        private ConcurrentDictionary<Guid, SubscribeContract> BiddingPriceContracts { get; set; } = new ConcurrentDictionary<Guid, SubscribeContract>();
        private ConcurrentDictionary<Guid, SubscribeContract> CircuitBreakContracts { get; set; } = new ConcurrentDictionary<Guid, SubscribeContract>();
        private ConcurrentDictionary<Guid, SubscribeContract> StockConclusionContracts { get; set; } = new ConcurrentDictionary<Guid, SubscribeContract>();
        private ConcurrentDictionary<Guid, SubscribeContract> IndexConclusionContracts { get; set; } = new ConcurrentDictionary<Guid, SubscribeContract>();
        private ConcurrentDictionary<Guid, SubscribeContract> ETFConclusionContracts { get; set; } = new ConcurrentDictionary<Guid, SubscribeContract>();
        #endregion


        public List<Candle> GetChart(string code, DateTime startDate, DateTime endDate, CandleTypes candleType)
        {
            try
            {
                var contract = DaishinMasterContract;
                if (contract != null)
                    return contract.Callback.GetChart(code, startDate, endDate, candleType);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return null;
        }

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
                    _logger.Error($"{contract.ToString()} consumer contract exist / {clientId}");
                }
                else
                {
                    if (contractList.TryAdd(clientId, contract) == true)
                        _logger.Info($"{contract.ToString()} consumer contract registered / {clientId}");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            finally
            {
                NotifyPropertyChanged(nameof(ConsumerContractCount));
            }
        }

        public void UnregisterContractAll(Guid clientId)
        {
            foreach (SubscribeTypes value in Enum.GetValues(typeof(SubscribeTypes)))
            {
                UnregisterContract(clientId, value);
            }
        }

        public void UnregisterContract(Guid clientId, SubscribeTypes type)
        {
            try
            {
                var contractList = GetSubscriptionList(type);
                if (contractList == null) return;

                if (contractList.ContainsKey(clientId) == true)
                {
                    if (contractList.TryRemove(clientId, out var temp) == true)
                    {
                        temp.Callback = null;
                        _logger.Info($"{clientId} / {type} contract unregistered");
                    }
                }
                else
                {
                    _logger.Info($"{clientId} / {type} contract not exist");
                }

                if (MasteringContracts.ContainsKey(clientId) == false &&
                    ChartContracts.ContainsKey(clientId) == false &&
                    BiddingPriceContracts.ContainsKey(clientId) == false &&
                    CircuitBreakContracts.ContainsKey(clientId) == false &&
                    StockConclusionContracts.ContainsKey(clientId) == false &&
                    IndexConclusionContracts.ContainsKey(clientId) == false &&
                    ETFConclusionContracts.ContainsKey(clientId) == false)
                {
                    if (ConsumerContracts.TryRemove(clientId, out var temp) == true)
                    {
                        temp.Callback = null;
                        _logger.Info($"{clientId} / {type} contract unregistered");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            finally
            {
                NotifyPropertyChanged(nameof(ConsumerContractCount));
            }
        }

        private ConcurrentDictionary<Guid, SubscribeContract> GetSubscriptionList(SubscribeTypes type)
        {
            switch (type)
            {
                case SubscribeTypes.Mastering:          return MasteringContracts;
                case SubscribeTypes.Chart:              return ChartContracts;
                case SubscribeTypes.BiddingPrice:       return BiddingPriceContracts;
                case SubscribeTypes.CircuitBreak:       return CircuitBreakContracts;
                case SubscribeTypes.StockConclusion:    return StockConclusionContracts;
                case SubscribeTypes.IndexConclusion:    return IndexConclusionContracts;
                case SubscribeTypes.ETFConclusion:      return ETFConclusionContracts;
                default:                                return null;
            }
        }

        private void ProcessBiddingPriceQueue()
        {
            try
            {
                if (BiddingPriceQueue.TryDequeue(out var biddingPrice) == true)
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
                                _logger.Error(ex);
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
                _logger.Error(ex);
            }
        }

        private void ProcessCircuitBreakQueue()
        {
            try
            {
                if (CircuitBreakQueue.TryDequeue(out var circuitBreak) == true)
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
                                _logger.Error(ex);
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
                _logger.Error(ex);
            }
        }

        private void ProcessStockConclusionQueue()
        {
            try
            {
                if (StockConclusionQueue.TryDequeue(out var conclusion) == true)
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
                                _logger.Error(ex);
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
                _logger.Error(ex);
            }
        }

        private void ProcessIndexConclusionQueue()
        {
            try
            {
                if (IndexConclusionQueue.TryDequeue(out var conclusion) == true)
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
                                _logger.Error(ex);
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
                _logger.Error(ex);
            }
        }

        private void ProcessETFConclusionQueue()
        {
            try
            {
                if (ETFConclusionQueue.TryDequeue(out var conclusion) == true)
                {
                    foreach (var contract in ETFConclusionContracts)
                    {
                        if (contract.Value.Scope == SubscribeScopes.All ||
                            contract.Value.ContainCode(conclusion.Code) == true)
                        {
                            try
                            {
                                contract.Value.Callback.ConsumeETFConclusion(conclusion);
                            }
                            catch (Exception ex)
                            {
                                _logger.Error(ex);
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
                _logger.Error(ex);
            }
        }
    }
}
