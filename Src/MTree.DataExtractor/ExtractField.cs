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

    public enum StockFieldTypes
    {
        Time,
        // Master
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
        // Conclusion
        Amount,
        MarketTimeType,
        Price,
        ConclusionType,
    }

    public enum IndexFieldTypes
    {
        Time,
        // Master
        BasisPrice,
        // Conclusion
        Amount,
        MarketTimeType,
        Price,
        MarketCapitalization,
    }

    class ExtractField
    {
    }
}
