﻿using System;
using System.ServiceModel;
using DataStructure;
using System.Collections.Generic;

namespace RealTimeProvider
{
    [ServiceContract(CallbackContract = typeof(IRealTimePublisherCallback))]
    public interface IRealTimePublisher
    {
        [OperationContract]
        void SendMessage(MessageTypes type, string message);

        [OperationContract]
        void RegisterPublisherContract(Guid clientId, PublisherContract contract);

        [OperationContract]
        void UnregisterPublisherContract(Guid clientId);

        [OperationContract]
        void PublishBiddingPrice(BiddingPrice biddingPrice);

        [OperationContract]
        void PublishCircuitBreak(CircuitBreak circuitBreak);

        [OperationContract]
        void PublishStockConclusion(StockConclusion conclusion);

        [OperationContract]
        void PublishIndexConclusion(IndexConclusion conclusion);

        [OperationContract]
        void PublishETFConclusion(ETFConclusion conclusion);
    }

    public interface IRealTimePublisherCallback
    {
        [OperationContract]
        void NotifyMessage(MessageTypes type, string message);

        [OperationContract]
        Dictionary<string, CodeEntity> GetCodeList();

        [OperationContract]
        Dictionary<string, object> GetCodeMap(CodeMapTypes codemapType);

        [OperationContract]
        StockMaster GetStockMaster(string code);

        [OperationContract]
        IndexMaster GetIndexMaster(string code);

        [OperationContract]
        List<Candle> GetChart(string code, DateTime startDate, DateTime endDate, CandleTypes candleType);

        [OperationContract]
        string GetMarketInfo(MarketInfoTypes type);

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

        [OperationContract]
        bool SubscribeBidding(string code);

        [OperationContract]
        bool UnsubscribeBidding(string code);

        [OperationContract]
        bool SubscribeCircuitBreak(string code);

        [OperationContract]
        bool UnsubscribeCircuitBreak(string code);

        [OperationContract]
        bool SubscribeETF(string code);

        [OperationContract]
        bool UnsubscribeETF(string code);
    }
}
