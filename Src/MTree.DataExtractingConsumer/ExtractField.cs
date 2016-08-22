using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.DataExtractingConsumer
{
    public enum ExtractTypes
    {
        Stock,
        Index,
    }

    public enum StockConclusionField
    {
        Time,
        Amount,
        MarketTimeType,
        Price,
        ConclusionType,
    }

    public enum StockMasterField
    {
        MarketType,
        SettlementMonth,
        FaceValue,
        ListedDate,
        CirculatingVolume,
        ShareVolume,
        ListedCapital,
        BasisPrice,
        UpperLimit,
        LowerLimit,
        PreviousClosingPrice,
        PreviousVolume,
        QuantityUnit,
        ForeigneLimit,
        ForeigneAvailableRemain,
        ForeigneLimitRate,
        ForeigneHold,
        ForeigneExhaustingRate,
        ForeigneAvailableRemainRate,
        AdministrativeIssue,
        InvestCaution,
        InvestWarning,
        InvestRiskNoticed,
        InvestRisk,
        TradingHalt,
        TradingSuspend,
        UnfairAnnouncement,
        Overheated,
        OverheatNoticed,
        CleaningTrade,
        InvestAttention,
        CallingAttention,
        Asset,
        BPS,
        PBR,
        NetIncome,
        EPS,
        PER,
        ROE,
        EV,
        ValueAlteredType,
    }

    public enum StockTAField
    {
        MovingAverage_5_Sma,
        MovingAverage_5_Ema,
        MovingAverage_5_Wma,
        MovingAverage_5_Dema,
        MovingAverage_5_Tema,
        MovingAverage_5_Trima,
        MovingAverage_5_Kama,
        MovingAverage_5_Mama,
        MovingAverage_5_T3,
        MovingAverage_10_Sma,
        MovingAverage_10_Ema,
        MovingAverage_10_Wma,
        MovingAverage_10_Dema,
        MovingAverage_10_Tema,
        MovingAverage_10_Trima,
        MovingAverage_10_Kama,
        MovingAverage_10_Mama,
        MovingAverage_10_T3,
        MovingAverage_20_Sma,
        MovingAverage_20_Ema,
        MovingAverage_20_Wma,
        MovingAverage_20_Dema,
        MovingAverage_20_Tema,
        MovingAverage_20_Trima,
        MovingAverage_20_Kama,
        MovingAverage_20_Mama,
        MovingAverage_20_T3,
        AverageTrueRange_8,
        AverageTrueRange_14,
        AverageTrueRange_20,
        AverageTrueRange_50,
        AverageDirectionalIndexRating_14,
        AverageDirectionalIndexRating_30,
        AverageDirectionalIndexRating_100,
        AverageDirectionalIndexRating_150,
        AverageDirectionalIndex_14,
        MinusDI_14,
        PlusDI_14,
        MinusDM_14,
        PlusDM_14,
        AverageDirectionalIndex_30,
        MinusDI_30,
        PlusDI_30,
        MinusDM_30,
        PlusDM_30,
        AverageDirectionalIndex_100,
        MinusDI_100,
        PlusDI_100,
        MinusDM_100,
        PlusDM_100,
        AverageDirectionalIndex_150,
        MinusDI_150,
        PlusDI_150,
        MinusDM_150,
        PlusDM_150,
    }

    public enum IndexConclusionField
    {
        Time,
        Amount,
        MarketTimeType,
        Price,
        MarketCapitalization,
    }

    public enum IndexMasterField
    {
        BasisPrice,
    }

    public enum IndexTAField
    {
    }
}
