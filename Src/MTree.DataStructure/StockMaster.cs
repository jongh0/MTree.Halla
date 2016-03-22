using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.DataStructure
{
    #region enum
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
    #endregion

    [Serializable]
    public class StockMaster : Subscribable
    {
        // 종목명(Daishin)
        [BsonElement("N")]
        public string Name { get; set; }

        [BsonElement("MT")]
        public MarketTypes MarketType { get; set; }

        // 결산월(Daishin)
        [BsonElement("SM")]
        public int SettlementMonth { get; set; }

        // 액면가(Daishin)
        [BsonElement("FV")]
        public int FaceValue { get; set; }

        // 상장일(Ebest)
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        [BsonElement("LD")]
        public DateTime ListedDate { get; set; }

        // 유동주식수(Ebest)
        [BsonElement("CV")]
        public long CirculatingVolume { get; set; }

        // 상장주식수(Daishin)
        [BsonElement("SV")]
        public long ShareVolume { get; set; }

        // 자본금(Daishin)
        [BsonElement("LC")]
        public long ListedCapital { get; set; }

        // 기준가(Daishin)
        [BsonElement("BP")]
        public float BasisPrice { get; set; }

        // 상한가(Daishin)
        [BsonElement("UL")]
        public int UpperLimit { get; set; }

        // 하한가(Daishin)
        [BsonElement("LL")]
        public int LowerLimit { get; set; }

        // 전일종가(Daishin)
        [BsonElement("PCP")]
        public float PreviousClosedPrice { get; set; }

        // 전일거래량(Daishin)
        [BsonElement("PV")]
        public long PreviousVolume { get; set; }

        // 호가단위(Daishin)
        [BsonElement("QU")]
        public int QuantityUnit { get; set; }

        // 외국인한도(Daishin)
        [BsonElement("FL")]
        public long ForeigneLimit { get; set; }

        // 외국인잔량(Daishin)
        [BsonElement("FAR")]
        public long ForeigneAvailableRemain { get; set; }

        // 외국인한도비율
        [BsonIgnore]
        public float ForeigneLimitRate { get { return (float)ForeigneLimit / ShareVolume * 100; } }

        // 외국인보유
        [BsonIgnore]
        public long ForeigneHold { get { return ForeigneLimit - ForeigneAvailableRemain; } }

        // 외국인소진율
        [BsonIgnore]
        public float ForeigneExhaustingRate { get { return (float)ForeigneHold / ForeigneLimit * 100; } }

        // 외국인잔량률
        [BsonIgnore]
        public float ForeigneAvailableRemainRate { get { return (float)ForeigneAvailableRemain / ShareVolume * 100; } }

        // 관리(Ebest)
        [BsonElement("AI")]
        public bool AdministrativeIssue { get; set; }

        // 주의(Ebest)
        [BsonElement("IC")]
        public bool InvestCaution { get; set; }

        // 경고(Ebest)
        [BsonElement("IW")]
        public bool InvestWarning { get; set; }

        // 위험예고(Ebest)
        [BsonElement("IRN")]
        public bool InvestmentRiskNoticed { get; set; }

        // 위험(Ebest)
        [BsonElement("IR")]
        public bool InvestmentRisk { get; set; }

        // 매매정지(Ebest)
        [BsonElement("TH")]
        public bool TradingHalt { get; set; }

        // 매매중단(Ebest)
        [BsonElement("TS")]
        public bool TradingSuspend { get; set; }

        // 불성실공시(Ebest)
        [BsonElement("UA")]
        public bool UnfairAnnouncement { get; set; }

        // 단기과열(Ebest)
        [BsonElement("Ov")]
        public bool Overheated { get; set; }

        // 단기과열지정예고(Ebest)
        [BsonElement("ON")]
        public bool OverheatNoticed { get; set; }

        // 정리매매(Ebest)
        [BsonElement("CT")]
        public bool CleaningTrade { get; set; }

        // 투자유의(Ebest)
        [BsonElement("IA")]
        public bool InvestAttention { get; set; }

        // 투자환기(Ebest)
        [BsonElement("CA")]
        public bool CallingAttention { get; set; }

        // 자산(Kiwoom & Daishin)
        [BsonElement("As")]
        public double Asset { get; set; }

        // Bookvalue Per Share (Kiwoom)
        public double BPS { get; set; }

        // Price Earning Ratio (Kiwoom)
        public double PBR { get; set; }

        // 당기순이익(Kiwoom & Daishin)
        [BsonElement("NI")]
        public double NetIncome { get; set; }

        // Earning Per Share (Kiwoom)
        public double EPS { get; set; }

        // Price Earning Ratio (Kiwoom)
        public double PER { get; set; }

        // Return On Equity (Kiwoom)
        public double ROE { get; set; }

        // Enterprise Value (Kiwoom)
        public double EV { get; set; }

        // 락(Ebest)
        [BsonElement("VAT")]
        public ValueAlteredTypes ValueAlteredType { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append(base.ToString());
                sb.AppendLine($"{nameof(Name)}: {Name}");
                sb.AppendLine($"{nameof(MarketType)}: {MarketType}");
                sb.AppendLine($"{nameof(SettlementMonth)}: {SettlementMonth}");
                sb.AppendLine($"{nameof(FaceValue)}: {FaceValue}");
                sb.AppendLine($"{nameof(ListedDate)}: {ListedDate}");
                sb.AppendLine($"{nameof(CirculatingVolume)}: {CirculatingVolume}");
                sb.AppendLine($"{nameof(ShareVolume)}: {ShareVolume}");
                sb.AppendLine($"{nameof(ListedCapital)}: {ListedCapital}");
                sb.AppendLine($"{nameof(BasisPrice)}: {BasisPrice}");
                sb.AppendLine($"{nameof(UpperLimit)}: {UpperLimit}");
                sb.AppendLine($"{nameof(LowerLimit)}: {LowerLimit}");
                sb.AppendLine($"{nameof(PreviousClosedPrice)}: {PreviousClosedPrice}");
                sb.AppendLine($"{nameof(PreviousVolume)}: {PreviousVolume}");
                sb.AppendLine($"{nameof(QuantityUnit)}: {QuantityUnit}");
                sb.AppendLine($"{nameof(ForeigneLimit)}: {ForeigneLimit}");
                sb.AppendLine($"{nameof(ForeigneAvailableRemain)}: {ForeigneAvailableRemain}");
                sb.AppendLine($"{nameof(ForeigneLimitRate)}: {ForeigneLimitRate}");
                sb.AppendLine($"{nameof(ForeigneHold)}: {ForeigneHold}");
                sb.AppendLine($"{nameof(ForeigneExhaustingRate)}: {ForeigneExhaustingRate}");
                sb.AppendLine($"{nameof(ForeigneAvailableRemainRate)}: {ForeigneAvailableRemainRate}");
                sb.AppendLine($"{nameof(AdministrativeIssue)}: {AdministrativeIssue}");
                sb.AppendLine($"{nameof(InvestCaution)}: {InvestCaution}");
                sb.AppendLine($"{nameof(InvestWarning)}: {InvestWarning}");
                sb.AppendLine($"{nameof(InvestmentRiskNoticed)}: {InvestmentRiskNoticed}");
                sb.AppendLine($"{nameof(InvestmentRisk)}: {InvestmentRisk}");
                sb.AppendLine($"{nameof(TradingHalt)}: {TradingHalt}");
                sb.AppendLine($"{nameof(TradingSuspend)}: {TradingSuspend}");
                sb.AppendLine($"{nameof(UnfairAnnouncement)}: {UnfairAnnouncement}");
                sb.AppendLine($"{nameof(Overheated)}: {Overheated}");
                sb.AppendLine($"{nameof(OverheatNoticed)}: {OverheatNoticed}");
                sb.AppendLine($"{nameof(CleaningTrade)}: {CleaningTrade}");
                sb.AppendLine($"{nameof(InvestAttention)}: {InvestAttention}");
                sb.AppendLine($"{nameof(CallingAttention)}: {CallingAttention}");
                sb.AppendLine($"{nameof(Asset)}: {Asset}");
                sb.AppendLine($"{nameof(BPS)}: {BPS}");
                sb.AppendLine($"{nameof(PBR)}: {PBR}");
                sb.AppendLine($"{nameof(NetIncome)}: {NetIncome}");
                sb.AppendLine($"{nameof(EPS)}: {EPS}");
                sb.AppendLine($"{nameof(PER)}: {PER}");
                sb.AppendLine($"{nameof(ValueAlteredType)}: {ValueAlteredType}");
            }
            catch { }

            return sb.ToString();
        }
    }
}
