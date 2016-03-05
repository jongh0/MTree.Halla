using MTree.DataStructure;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTree.Provider
{
    public class BrokerageFirmProvider : BaseProvider
    {
        protected AutoResetEvent waitQuoting;

        protected StockMaster quotingStockMaster;
        protected IndexMaster quotingIndexMaster;

        protected ConcurrentDictionary<string, IndexConclusion> prevIndexConclusions;

        protected ConcurrentQueue<BiddingPrice> biddingPriceQueue;
        protected ConcurrentQueue<StockConclusion> stockConclusionQueue;
        protected ConcurrentQueue<IndexConclusion> indexConclusionQueue;

        public BrokerageFirmProvider() : base()
        {
            waitQuoting = new AutoResetEvent(false);

            prevIndexConclusions = new ConcurrentDictionary<string, IndexConclusion>();

            biddingPriceQueue = new ConcurrentQueue<BiddingPrice>();
            stockConclusionQueue = new ConcurrentQueue<StockConclusion>();
            indexConclusionQueue = new ConcurrentQueue<IndexConclusion>();
        }
    }
}
