using MTree.DataStructure;
using MTree.RealTimeProvider;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTree.Consumer
{
    public class ConsumerBase : SubscribingBase, IConsumerCallback
    {
        public event Action<List<StockMaster>> ConsumeStockMasterEvent;

        public event Action<List<IndexMaster>> ConsumeIndexMasterEvent;

        public event Action<string> ConsumeCodemapEvent;

        public event Action<MessageTypes, string> NotifyMessageEvent;
        
        public virtual void NotifyMessage(MessageTypes type, string message)
        {
            NotifyMessageEvent?.Invoke(type, message);
        }

        public virtual void ConsumeBiddingPrice(BiddingPrice biddingPrice)
        {
            BiddingPriceQueue.Enqueue(biddingPrice);
        }

        public virtual void ConsumeCircuitBreak(CircuitBreak circuitBreak)
        {
            CircuitBreakQueue.Enqueue(circuitBreak);
        }

        public virtual void ConsumeIndexConclusion(IndexConclusion conclusion)
        {
            IndexConclusionQueue.Enqueue(conclusion);
        }

        public virtual void ConsumeStockConclusion(StockConclusion conclusion)
        {
            StockConclusionQueue.Enqueue(conclusion);
        }

        public virtual void ConsumeStockMaster(List<StockMaster> stockMasters)
        {
            ConsumeStockMasterEvent?.Invoke(stockMasters);
        }
        
        public virtual void ConsumeIndexMaster(List<IndexMaster> indexMasters)
        {
            ConsumeIndexMasterEvent?.Invoke(indexMasters);
        }

        public virtual void ConsumeCodemap(string codeMap)
        {
            ConsumeCodemapEvent?.Invoke(codeMap);
        }

        public virtual void ConsumeChart(List<Candle> candles)
        {
        }
    }
}
