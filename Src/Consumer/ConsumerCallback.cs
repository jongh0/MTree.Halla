using DataStructure;
using RealTimeProvider;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Consumer
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class ConsumerCallback : SubscribingBase, IRealTimeConsumerCallback
    {
        public virtual void NotifyMessage(MessageTypes type, string message)
        {
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

        public virtual void ConsumeETFConclusion(ETFConclusion conclusion)
        {
            ETFConclusionQueue.Enqueue(conclusion);
        }

        public virtual void ConsumeStockMaster(List<StockMaster> stockMasters)
        {
        }
        
        public virtual void ConsumeIndexMaster(List<IndexMaster> indexMasters)
        {
        }

        public virtual void ConsumeCodeMap(Dictionary<string, object> codeMap)
        {
        }

        public virtual void ConsumeChart(List<Candle> candles)
        {
        }
    }
}
