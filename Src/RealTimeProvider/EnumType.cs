using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeProvider
{
    public enum ExitProgramTypes
    {
        Normal,
        Force,
        Restart,
    }

    public enum SubscribeTypes
    {
        Chart,
        Mastering,
        BiddingPrice,
        CircuitBreak,
        StockConclusion,
        IndexConclusion,
        ETFConclusion,
    }

    public enum SubscribeScopes
    {
        All,
        Partial,
    }

    public enum MessageTypes
    {
        KeepAlive,
        CloseClient,
        MasteringDone,
        SubscribingDone,
        DaishinSessionDisconnected,
    }

    public enum MarketInfoTypes
    {
        WorkDate,
        StartTime,
        EndTime,
    }

    public enum CodeMapTypes
    {
        Theme,
        BizType,
        CapitalScale,
        Group
    }
}
