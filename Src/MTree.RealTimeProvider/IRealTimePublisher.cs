using System;
using System.ServiceModel;
using MTree.DataStructure;
using System.Collections.Generic;

namespace MTree.RealTimeProvider
{
    [ServiceContract(CallbackContract = typeof(IRealTimePublisherCallback))]
    public interface IRealTimePublisher
    {
        [OperationContract(IsOneWay = true)]
        void NoOperation();

        [OperationContract]
        void RegisterPublishContract(Guid clientId, PublishContract contract);

        [OperationContract(IsOneWay = true)]
        void UnregisterPublishContract(Guid clientId);

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
        [OperationContract(IsOneWay = true)]
        void NoOperation();

        [OperationContract(IsOneWay = true)]
        void CloseClient();

        [OperationContract]
        Dictionary<string, string> GetStockCodeList();

        [OperationContract]
        StockMaster GetStockMaster(string code);

        [OperationContract]
        bool IsSubscribable();

        [OperationContract]
        bool SubscribeStock(string code);

        [OperationContract]
        bool UnsubscribeStock(string code);

        [OperationContract]
        bool SubscribeIndex(string code);

        [OperationContract]
        bool UnsubscribeIndex(string code);
    }
}
