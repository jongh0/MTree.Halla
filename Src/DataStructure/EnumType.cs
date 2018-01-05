using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStructure
{
    public enum CandleTypes
    {
        Tick,
        Min,
        Day,
        Week,
        Month,
        Year,
    }

    public enum CircuitBreakTypes
    {
        Clear,
        StaticInvoke,
        DynamicInvoke,
        StaticAndDynamicInvoke,
    }

    public enum MarketTimeTypes
    {
        Unknown,
        Normal,
        NormalExpect,
        BeforeOffTheClock,
        BeforeExpect,
        AfterOffTheClock,
        AfterExpect,
    }

    public enum CounterTypes
    {
        Chart,
        BiddingPrice,
        CircuitBreak,
        StockMaster,
        IndexMaster,
        StockConclusion,
        IndexConclusion,
        ETFConclusion,
    }

    public enum DataTypes
    {
        DaishinPublisher,
        RealTimeProvider,
        HistorySaver,
        Database,
        Dashboard,
    }

    public enum ConclusionTypes
    {
        Unknown,
        Sell,
        Buy,
    }

    public enum ValueAlteredTypes
    {
        None,
        ExRightDividend,    // 권배락
        ExRight,            // 권리락
        ExDividend,         // 배당락
        SplitFaceValue,     // 액면분할
        MergeFaceValue,     // 액면병합
        Consolidation,      // 주식병합
        Divestiture,        // 기업분할
        CapitalReduction,   // 감자
    }

    public enum MasteringStates
    {
        Ready,
        Running,
        Finished,
    }

    public enum MarketTypes
    {
        Unknown,
        INDEX,
        KOSPI,
        KOSDAQ,
        KONEX,
        ETF,
        ETN,
        ELW,
        FREEBOARD
    }
}
