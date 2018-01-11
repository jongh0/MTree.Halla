using System;
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
        void RegisterConsumerContract(Guid clientId, SubscribeContract contract);

        [OperationContract(IsOneWay = true)]
        void UnregisterConsumerContractAll(Guid clientId);

        [OperationContract(IsOneWay = true)]
        void UnregisterConsumerContract(Guid clientId, SubscribeTypes type);

        [OperationContract]
        List<Candle> GetChart(string code, DateTime startDate, DateTime endDate, CandleTypes candleType);
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
        void ConsumeETFConclusion(ETFConclusion conclusion);

        [OperationContract]
        void ConsumeStockMaster(List<StockMaster> stockMaster);

        [OperationContract]
        void ConsumeIndexMaster(List<IndexMaster> indexMaster);

        [OperationContract]
        void ConsumeCodeMap(Dictionary<string, object> codeMap);

        [OperationContract]
        void ConsumeChart(List<Candle> candles);
    }
}
