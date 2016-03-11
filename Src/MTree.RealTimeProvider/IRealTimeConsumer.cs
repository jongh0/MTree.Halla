using System;
using System.ServiceModel;
using MTree.DataStructure;

namespace MTree.RealTimeProvider
{
    [ServiceContract(CallbackContract = typeof(IRealTimeConsumerCallback))]
    public interface IRealTimeConsumer
    {
        [OperationContract(IsOneWay = true)]
        void NoOperation();

        [OperationContract(IsOneWay = true)]
        void RegisterSubscribeContract(Guid clientId, SubscribeContract contract);

        [OperationContract(IsOneWay = true)]
        void UnregisterSubscribeContractAll(Guid clientId);

        [OperationContract(IsOneWay = true)]
        void UnregisterSubscribeContract(Guid clientId, SubscribeType type);
    }

    public interface IRealTimeConsumerCallback
    {
        [OperationContract(IsOneWay = true)]
        void NoOperation();

        [OperationContract(IsOneWay = true)]
        void CloseClient();

        [OperationContract(IsOneWay = true)]
        void ConsumeBiddingPrice(BiddingPrice biddingPrice);

        [OperationContract(IsOneWay = true)]
        void ConsumeCircuitBreak(CircuitBreak circuitBreak);

        [OperationContract(IsOneWay = true)]
        void ConsumeStockConclusion(StockConclusion conclusion);

        [OperationContract(IsOneWay = true)]
        void ConsumeIndexConclusion(IndexConclusion conclusion);
    }
}
