using System;
using System.ServiceModel;
using MTree.DataStructure;

namespace MTree.RealTimeProvider
{
    [ServiceContract(CallbackContract = typeof(IRealTimeConsumerCallback))]
    public interface IRealTimeConsumer
    {
        [OperationContract(IsOneWay = true)]
        void KeepConnection();
    }

    public interface IRealTimeConsumerCallback
    {
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
