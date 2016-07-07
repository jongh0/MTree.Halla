using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.DataExtractor
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
        MovingAverage,
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
