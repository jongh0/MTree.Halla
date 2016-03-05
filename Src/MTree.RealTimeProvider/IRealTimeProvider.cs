using System;
using System.ServiceModel;
using MTree.DataStructure;

namespace MTree.RealTimeProvider
{
    [ServiceContract(CallbackContract = typeof(IRealTimeProviderCallback))]
    public interface IRealTimeProvider
    {
        [OperationContract(IsOneWay = true)]
        void KeepConnection();

        [OperationContract(IsOneWay = true)]
        void NotifyBiddingPrice(BiddingPrice biddingPrice);

        [OperationContract(IsOneWay = true)]
        void NotifyCircuitBreak(CircuitBreak circuitBreak);

        [OperationContract(IsOneWay = true)]
        void NotifyConclusion(StockConclusion conclusion);

        [OperationContract(IsOneWay = true)]
        void NotifyConclusion(IndexConclusion conclusion);
    }

    public interface IRealTimeProviderCallback
    {
        [OperationContract(IsOneWay = true)]
        void BiddingPriceUpdated(BiddingPrice biddingPrice);

        [OperationContract(IsOneWay = true)]
        void CircuitBreakUpdated(CircuitBreak circuitBreak);

        [OperationContract(IsOneWay = true)]
        void ConclusionUpdated(StockConclusion conclusion);

        [OperationContract(IsOneWay = true)]
        void ConclusionUpdated(IndexConclusion conclusion);
    }
}
