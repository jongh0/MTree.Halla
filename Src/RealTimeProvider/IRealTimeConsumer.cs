﻿using System;
using System.ServiceModel;
using DataStructure;
using System.Collections.Generic;

namespace RealTimeProvider
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

        [OperationContract]
        List<Candle> GetChart(string code, DateTime startDate, DateTime endDate, CandleTypes candleType);
    }

    public interface IRealTimeConsumerCallback
    {
        event Action<List<StockMaster>> ConsumeStockMasterEvent;

        event Action<List<IndexMaster>> ConsumeIndexMasterEvent;

        event Action<Dictionary<string, object>> ConsumeCodemapEvent;

        event Action<MessageTypes, string> NotifyMessageEvent;

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
        void ConsumeETFConclusion(ETFConclusion conclusion);

        [OperationContract]
        void ConsumeStockMaster(List<StockMaster> stockMaster);

        [OperationContract]
        void ConsumeIndexMaster(List<IndexMaster> indexMaster);

        [OperationContract]
        void ConsumeCodemap(Dictionary<string, object> codeMap);

        [OperationContract]
        void ConsumeChart(List<Candle> candles);
    }
}
