using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.DataStructure
{
    #region enum
    public enum ValueAlteredType
    {
        None,
        /// <summary>
        /// 권배락
        /// </summary>
        ExRightDividend,
        /// <summary>
        /// 권리락
        /// </summary>
        ExRight,
        /// <summary>
        /// 배당락
        /// </summary>
        ExDividend,
        /// <summary>
        /// 액면분할
        /// </summary>
        SplitFaceValue,
        /// <summary>
        /// 액면병합
        /// </summary>
        MergeFaceValue,
        /// <summary>
        /// 주식병합
        /// </summary>
        Consolidation,
        /// <summary>
        /// 기업분할
        /// </summary>
        Divestiture,
        /// <summary>
        /// 감자
        /// </summary>
        CapitalReduction,
    } 
    #endregion

    [Serializable]
    public class StockMaster : Subscribable
    {
        /// <summary>
        /// 종목명(Daishin)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 결산월(Daishin)
        /// </summary>
        [BsonElement("SMo")]
        public int SettlementMonth { get; set; }

        /// <summary>
        /// 액면가(Daishin)
        /// </summary>
        [BsonElement("FVa")]
        public int FaceValue { get; set; }

        /// <summary>
        /// 상장일(Ebest)
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        [BsonElement("LDa")]
        public DateTime ListedDate { get; set; }

        /// <summary>
        /// 유동주식수(Ebest)
        /// </summary>
        [BsonElement("CVo")]
        public long CirculatingVolume { get; set; }

        /// <summary>
        /// 상장주식수(Daishin)
        /// </summary>
        [BsonElement("SVo")]
        public long ShareVolume { get; set; }

        /// <summary>
        /// 자본금(Daishin)
        /// </summary>
        [BsonElement("LCa")]
        public long ListedCapital { get; set; }

        /// <summary>
        /// 기준가(Daishin)
        /// </summary>
        [BsonElement("BPr")]
        public float BasisPrice { get; set; }

        /// <summary>
        /// 상한가(Daishin)
        /// </summary>
        [BsonElement("ULi")]
        public int UpperLimit { get; set; }

        /// <summary>
        /// 하한가(Daishin)
        /// </summary>
        [BsonElement("LLi")]
        public int LowerLimit { get; set; }

        /// <summary>
        /// 전일종가
        /// </summary>
        [BsonElement("PCPr")]
        public float PreviousClosedPrice { get; set; }

        /// <summary>
        /// 전일거래량(Daishin)
        /// </summary>
        [BsonElement("PVo")]
        public long PreviousVolume { get; set; }

        /// <summary>
        /// 호가단위(Daishin)
        /// </summary>
        [BsonElement("QUn")]
        public int QuantityUnit { get; set; }

        /// <summary>
        /// 외국인한도(Daishin)
        /// </summary>
        [BsonElement("FLi")]
        public long ForeigneLimit { get; set; }

        /// <summary>
        /// 외국인잔량(Daishin)
        /// </summary>
        [BsonElement("FARe")]
        public long ForeigneAvailableRemain { get; set; }

        /// <summary>
        /// 외국인한도비율
        /// </summary>
        [BsonElement("FLRa")]
        public float ForeigneLimitRate { get { return (float)ForeigneLimit / ShareVolume * 100; } }

        /// <summary>
        /// 외국인보유
        /// </summary>
        [BsonElement("FHo")]
        public long ForeigneHold { get { return ForeigneLimit - ForeigneAvailableRemain; } }

        /// <summary>
        /// 외국인소진율
        /// </summary>
        [BsonElement("FERa")]
        public float ForeigneExhaustingRate { get { return (float)ForeigneHold / ForeigneLimit * 100; } }

        /// <summary>
        /// 외국인잔량률
        /// </summary>
        [BsonElement("FARRa")]
        public float ForeigneAvailableRemainRate { get { return (float)ForeigneAvailableRemain / ShareVolume * 100; } }

        /// <summary>
        /// 매매정지(KRX)
        /// </summary>
        [BsonElement("THa")]
        public InvestWarningEntity TradingHalt { get; set; }

        /// <summary>
        /// 관리(KRX)
        /// </summary>
        [BsonElement("AIs")]
        public InvestWarningEntity AdministrativeIssue { get; set; }

        /// <summary>
        /// 주의(KRX)
        /// </summary>
        [BsonElement("ICa")]
        public InvestWarningEntity InvestCaution { get; set; }

        /// <summary>
        /// 경고(KRX)
        /// </summary>
        [BsonElement("IWa")]
        public InvestWarningEntity InvestWarning { get; set; }

        /// <summary>
        /// 위험(KRX)
        /// </summary>
        [BsonElement("IRi")]
        public InvestWarningEntity InvestmentRisk { get; set; }

        /// <summary>
        /// 불성실공시(KRX)
        /// </summary>
        [BsonElement("UAn")]
        public InvestWarningEntity UnfairAnnouncement { get; set; }

        /// <summary>
        /// 주의환기(KRX)
        /// </summary>
        [BsonElement("CAt")]
        public InvestWarningEntity CallingAttention { get; set; }

        /// <summary>
        /// Overheated(KRX)
        /// </summary>
        [BsonElement("CTr")]
        public InvestWarningEntity CleaningTrade { get; set; }

        /// <summary>
        /// 단기과열(KRX)
        /// </summary>
        [BsonElement("Ovh")]
        public InvestWarningEntity Overheated { get; set; }

        /// <summary>
        /// 자산(Naver)
        /// </summary>
        [BsonElement("Ass")]
        public double Asset { get; set; }

        /// <summary>
        /// Bookvalue Per Share
        /// </summary>
        public double BPS { get { return Asset / ShareVolume; } }

        /// <summary>
        /// Price Earning Ratio 
        /// </summary>
        public double PBR { get { return BasisPrice / BPS; } }

        /// <summary>
        /// 당기순이익(Naver)
        /// </summary>
        [BsonElement("NIn")]
        public double NetIncome { get; set; }

        /// <summary>
        /// Earning Per Share
        /// </summary>
        public double EPS { get { return NetIncome / ShareVolume; } }

        /// <summary>
        /// Price Earning Ratio
        /// </summary>
        public double PER { get { return BasisPrice / EPS; } }

        /// <summary>
        /// 락(Ebest)
        /// </summary>
        [BsonElement("VAl")]
        public ValueAlteredType ValueAltered { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(base.ToString());
            sb.AppendLine($"{nameof(Name)}: {Name}");
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
            sb.AppendLine($"{nameof(TradingHalt)}: {TradingHalt}");
            sb.AppendLine($"{nameof(AdministrativeIssue)}: {AdministrativeIssue}");
            sb.AppendLine($"{nameof(InvestCaution)}: {InvestCaution}");
            sb.AppendLine($"{nameof(InvestWarning)}: {InvestWarning}");
            sb.AppendLine($"{nameof(InvestmentRisk)}: {InvestmentRisk}");
            sb.AppendLine($"{nameof(UnfairAnnouncement)}: {UnfairAnnouncement}");
            sb.AppendLine($"{nameof(CallingAttention)}: {CallingAttention}");
            sb.AppendLine($"{nameof(CleaningTrade)}: {CleaningTrade}");
            sb.AppendLine($"{nameof(Overheated)}: {Overheated}");
            sb.AppendLine($"{nameof(Asset)}: {Asset}");
            sb.AppendLine($"{nameof(BPS)}: {BPS}");
            sb.AppendLine($"{nameof(PBR)}: {PBR}");
            sb.AppendLine($"{nameof(NetIncome)}: {NetIncome}");
            sb.AppendLine($"{nameof(EPS)}: {EPS}");
            sb.AppendLine($"{nameof(PER)}: {PER}");
            sb.AppendLine($"{nameof(ValueAltered)}: {ValueAltered}");

            return sb.ToString();
        }
    }
}
