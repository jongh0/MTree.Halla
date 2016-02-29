﻿using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStructure
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
        /// 종목명(대신)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 결산월(대신)
        /// </summary>
        [BsonElement("SMo")]
        public int SettlementMonth { get; set; }

        /// <summary>
        /// 액면가(대신)
        /// </summary>
        [BsonElement("FVa")]
        public int FaceValue { get; set; }

        /// <summary>
        /// 상장일(ebest)
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        [BsonElement("LDa")]
        public DateTime ListedDate { get; set; }

        /// <summary>
        /// 유동주식수(ebest)
        /// </summary>
        [BsonElement("CVo")]
        public long CirculatingVolume { get; set; }

        /// <summary>
        /// 상장주식수(대신)
        /// </summary>
        [BsonElement("SVo")]
        public long ShareVolume { get; set; }

        /// <summary>
        /// 자본금(대신)
        /// </summary>
        [BsonElement("LCa")]
        public long ListedCapital { get; set; }

        /// <summary>
        /// 기준가(대신)
        /// </summary>
        [BsonElement("BPr")]
        public float BasisPrice { get; set; }

        /// <summary>
        /// 상한가(대신)
        /// </summary>
        [BsonElement("ULi")]
        public int UpperLimit { get; set; }

        /// <summary>
        /// 하한가(대신)
        /// </summary>
        [BsonElement("LLi")]
        public int LowerLimit { get; set; }

        /// <summary>
        /// 전일종가
        /// </summary>
        [BsonElement("PCPr")]
        public float PreviousClosedPrice { get; set; }

        /// <summary>
        /// 전일거래량(대신)
        /// </summary>
        [BsonElement("PVo")]
        public long PreviousVolume { get; set; }

        /// <summary>
        /// 호가단위(대신)
        /// </summary>
        [BsonElement("QUn")]
        public int QuantityUnit { get; set; }

        /// <summary>
        /// 외국인한도(대신)
        /// </summary>
        [BsonElement("FLi")]
        public long ForeigneLimit { get; set; }

        /// <summary>
        /// 외국인잔량(대신)
        /// </summary>
        [BsonElement("FARe")]
        public long ForeigneAvailableRemain { get; set; }

        /// <summary>
        /// 외국인한도비율((float)ForeigneLimit / ShareVolume * 100)
        /// </summary>
        [BsonElement("FLRa")]
        public float ForeigneLimitRate { get; set; }

        /// <summary>
        /// 외국인보유(ForeigneLimit - ForeigneAvailableRemain)
        /// </summary>
        [BsonElement("FHo")]
        public long ForeigneHold { get; set; }

        /// <summary>
        /// 외국인소진율((float)ForeigneHold / ForeigneLimit * 100)
        /// </summary>
        [BsonElement("FERa")]
        public float ForeigneExhaustingRate { get; set; }

        /// <summary>
        /// 외국인잔량률((float)ForeigneAvailableRemain / ShareVolume * 100)
        /// </summary>
        [BsonElement("FARRa")]
        public float ForeigneAvailableRemainRate { get; set; }

        /// <summary>
        /// 매매정지(KRX)
        /// </summary>
        [BsonElement("THa")]
        public InvestWarningInfo TradingHalt { get; set; }

        /// <summary>
        /// 관리(KRX)
        /// </summary>
        [BsonElement("AIs")]
        public InvestWarningInfo AdministrativeIssue { get; set; }

        /// <summary>
        /// 주의(KRX)
        /// </summary>
        [BsonElement("ICa")]
        public InvestWarningInfo InvestCaution { get; set; }

        /// <summary>
        /// 경고(KRX)
        /// </summary>
        [BsonElement("IWa")]
        public InvestWarningInfo InvestWarning { get; set; }

        /// <summary>
        /// 위험(KRX)
        /// </summary>
        [BsonElement("IRi")]
        public InvestWarningInfo InvestmentRisk { get; set; }

        /// <summary>
        /// 불성실공시(KRX)
        /// </summary>
        [BsonElement("UAn")]
        public InvestWarningInfo UnfairAnnouncement { get; set; }

        /// <summary>
        /// 주의환기(KRX)
        /// </summary>
        [BsonElement("CAt")]
        public InvestWarningInfo CallingAttention { get; set; }

        /// <summary>
        /// Overheated(KRX)
        /// </summary>
        [BsonElement("CTr")]
        public InvestWarningInfo CleaningTrade { get; set; }

        /// <summary>
        /// 단기과열(KRX)
        /// </summary>
        [BsonElement("Ovh")]
        public InvestWarningInfo Overheated { get; set; }

        /// <summary>
        /// 자산(Naver)
        /// </summary>
        [BsonElement("Ass")]
        public double Asset { get; set; }

        /// <summary>
        /// Asset / ShareVolume
        /// </summary>
        public double BPS { get; set; }

        /// <summary>
        /// BasisPrice / BPS
        /// </summary>
        public double PBR { get; set; }

        /// <summary>
        /// 당기순이익(Naver(
        /// </summary>
        [BsonElement("NIn")]
        public double NetIncome { get; set; }

        /// <summary>
        /// NetIncome / ShareVolume
        /// </summary>
        public double EPS { get; set; }

        /// <summary>
        /// BasisPrice / EPS
        /// </summary>
        public double PER { get; set; }

        /// <summary>
        /// 락(ebest)
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
