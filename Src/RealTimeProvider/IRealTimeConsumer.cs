using System;
using System.ServiceModel;
using DataStructure;
using System.Collections.Generic;

namespace RealTimeProvider
{
    [ServiceContract(CallbackContract = typeof(IRealTimeConsumerCallback))]
    public interface IRealTimeConsumer
    {
        [OperationContract]
        void SendMessage(MessageTypes type, string message);

        [OperationContract]
        void RegisterConsumerContract(Guid clientId, SubscribeContract contract);

        [OperationContract]
        void UnregisterConsumerContractAll(Guid clientId);

        [OperationContract]
        void UnregisterConsumerContract(Guid clientId, SubscribeTypes type);

        [OperationContract]
        List<Candle> GetChart(string code, DateTime startDate, DateTime endDate, CandleTypes candleType);
    }

    public interface IRealTimeConsumerCallback
    {
        [OperationContract]
        void NotifyMessage(MessageTypes type, string message);

        [OperationContract]
        void ConsumeBiddingPrice(BiddingPrice biddingPrice);

        [OperationContract]
        void ConsumeCircuitBreak(CircuitBreak circuitBreak);

        [OperationContract]
        void ConsumeStockConclusion(StockConclusion conclusion);

        [OperationContract]
        void ConsumeIndexConclusion(IndexConclusion conclusion);

        [OperationContract]
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
