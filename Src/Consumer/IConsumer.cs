using DataStructure;
using RealTimeProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Consumer
{
    public interface IConsumer
    {
        event Action<MessageTypes, string> MessageNotified;

        event Action<BiddingPrice> BiddingPriceConsumed;

        event Action<CircuitBreak> CircuitBreakConsumed;

        event Action<IndexConclusion> IndexConclusionConsumed;

        event Action<StockConclusion> StockConclusionConsumed;

        event Action<ETFConclusion> ETFConclusionConsumed;

        event Action<List<StockMaster>> StockMasterConsumed;

        event Action<List<IndexMaster>> IndexMasterConsumed;

        event Action<Dictionary<string, object>> CodeMapConsumed;

        event Action<List<Candle>> ChartConsumed;
    }
}
