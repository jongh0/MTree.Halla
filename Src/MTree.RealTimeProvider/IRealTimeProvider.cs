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
        void NotifyStockConclusion(StockConclusion conclusion);

        [OperationContract(IsOneWay = true)]
        void NotifyIndexConclusion(IndexConclusion conclusion);
    }

    public interface IRealTimeProviderCallback
    {
        [OperationContract(IsOneWay = true)]
        void BiddingPriceUpdated(BiddingPrice biddingPrice);

        [OperationContract(IsOneWay = true)]
        void StockConclusionUpdated(StockConclusion conclusion);

        [OperationContract(IsOneWay = true)]
        void IndexConclusionUpdated(IndexConclusion conclusion);
    }
}
