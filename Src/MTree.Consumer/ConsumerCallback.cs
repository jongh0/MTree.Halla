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
    public class ConsumerCallback : RealTimeBase, IRealTimeConsumerCallback
    {
        public virtual void NotifyMessage(MessageTypes type, string message)
        {
        }

        public virtual void ConsumeBiddingPrice(BiddingPrice biddingPrice)
        {
        }

        public virtual void ConsumeCircuitBreak(CircuitBreak circuitBreak)
        {
        }

        public virtual void ConsumeIndexConclusion(IndexConclusion conclusion)
        {
        }

        public virtual void ConsumeStockConclusion(StockConclusion conclusion)
        {
        }

        public virtual void ConsumeStockMaster(List<StockMaster> stockMasters)
        {
        }

        public virtual void ConsumeIndexMaster(List<IndexMaster> indexMasters)
        {
        }

        public virtual void ConsumeChart(List<Candle> candles)
        {
        }
    }
}
