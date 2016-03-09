﻿using MTree.DataStructure;
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
    public class ConsumerBase : RealTimeBase, IRealTimeConsumerCallback
    {
        public ConsumerBase() : base()
        {
        }

        public virtual void NoOperation()
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
    }
}
