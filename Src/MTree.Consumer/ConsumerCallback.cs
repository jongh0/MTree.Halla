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
        public virtual void SendMessage(MessageTypes type, string message)
        {
            LastWcfCommunicateTick = Environment.TickCount;
        }

        public virtual void ConsumeBiddingPrice(BiddingPrice biddingPrice)
        {
            LastWcfCommunicateTick = Environment.TickCount;
        }

        public virtual void ConsumeCircuitBreak(CircuitBreak circuitBreak)
        {
            LastWcfCommunicateTick = Environment.TickCount;
        }

        public virtual void ConsumeIndexConclusion(IndexConclusion conclusion)
        {
            LastWcfCommunicateTick = Environment.TickCount;
        }

        public virtual void ConsumeStockConclusion(StockConclusion conclusion)
        {
            LastWcfCommunicateTick = Environment.TickCount;
        }

        public virtual void ConsumeStockMaster(StockMaster stockMaster)
        {
            LastWcfCommunicateTick = Environment.TickCount;
        }
    }
}
