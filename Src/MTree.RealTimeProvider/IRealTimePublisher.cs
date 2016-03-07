using System;
using System.ServiceModel;
using MTree.DataStructure;

namespace MTree.RealTimeProvider
{
    [ServiceContract(CallbackContract = typeof(IRealTimePublisherCallback))]
    public interface IRealTimePublisher
    {
        [OperationContract(IsOneWay = true)]
        void KeepConnection();

        [OperationContract(IsOneWay = true)]
        void PublishBiddingPrice(BiddingPrice biddingPrice);

        [OperationContract(IsOneWay = true)]
        void PublishCircuitBreak(CircuitBreak circuitBreak);

        [OperationContract(IsOneWay = true)]
        void PublishStockConclusion(StockConclusion conclusion);

        [OperationContract(IsOneWay = true)]
        void PublishIndexConclusion(IndexConclusion conclusion);
    }

    public interface IRealTimePublisherCallback
    {
    }
}
