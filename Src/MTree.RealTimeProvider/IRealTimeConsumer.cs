using System;
using System.ServiceModel;
using MTree.DataStructure;

namespace MTree.RealTimeProvider
{
    [ServiceContract(CallbackContract = typeof(IRealTimeConsumerCallback))]
    public interface IRealTimeConsumer
    {
        [OperationContract(IsOneWay = true)]
        void NotifyMessage(MessageTypes type, string message);

        [OperationContract(IsOneWay = true)]
        void RegisterContract(Guid clientId, SubscribeContract contract);

        [OperationContract(IsOneWay = true)]
        void UnregisterContractAll(Guid clientId);

        [OperationContract(IsOneWay = true)]
        void UnregisterContract(Guid clientId, SubscribeTypes type);
    }

    public interface IRealTimeConsumerCallback
    {
        [OperationContract(IsOneWay = true)]
        void NotifyMessage(MessageTypes type, string message);

        [OperationContract(IsOneWay = true)]
        void ConsumeBiddingPrice(BiddingPrice biddingPrice);

        [OperationContract(IsOneWay = true)]
        void ConsumeCircuitBreak(CircuitBreak circuitBreak);

        [OperationContract(IsOneWay = true)]
        void ConsumeStockConclusion(StockConclusion conclusion);

        [OperationContract(IsOneWay = true)]
        void ConsumeIndexConclusion(IndexConclusion conclusion);

        [OperationContract(IsOneWay = true)]
        void ConsumeStockMaster(StockMaster stockMaster);

        [OperationContract(IsOneWay = true)]
        void ConsumeIndexMaster(IndexMaster indexMaster);
    }
}
