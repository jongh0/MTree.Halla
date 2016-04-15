using System;
using System.ServiceModel;
using MTree.DataStructure;
using System.Collections.Generic;

namespace MTree.RealTimeProvider
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
        [OperationContract(IsOneWay = true)]
        void NotifyMessage(MessageTypes type, string message);

        [OperationContract]
        void ConsumeBiddingPrice(BiddingPrice biddingPrice);

        [OperationContract]
        void ConsumeCircuitBreak(CircuitBreak circuitBreak);

        [OperationContract]
        void ConsumeStockConclusion(StockConclusion conclusion);

        [OperationContract]
        void ConsumeIndexConclusion(IndexConclusion conclusion);

        [OperationContract(IsOneWay = true)]
        void ConsumeStockMaster(List<StockMaster> stockMaster);

        [OperationContract(IsOneWay = true)]
        void ConsumeIndexMaster(List<IndexMaster> indexMaster);

        [OperationContract(IsOneWay = true)]
        void ConsumeChart(List<Candle> candles);
    }
}
