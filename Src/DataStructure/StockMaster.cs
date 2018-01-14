using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataStructure
{
    [DataContract]
    public class StockMaster : Subscribable
    {
        // 종목명(Daishin)
        [BsonElement("N")]
        [DataMember(Name = "N")]
        public string Name { get; set; }

        [BsonElement("MT")]
        [DataMember(Name = "MT")]
        public MarketTypes MarketType { get; set; }

        // 결산월(Daishin)
        [BsonElement("SM")]
        [DataMember(Name = "SM")]
        public int SettlementMonth { get; set; }

        // 액면가(Daishin)
        [BsonElement("FV")]
        [DataMember(Name = "FV")]
        public int FaceValue { get; set; }

        // 상장일(Ebest)
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        [BsonElement("LD")]
        [DataMember(Name = "LD")]
        public DateTime ListedDate { get; set; }

        // 유동주식수(Ebest)
        [BsonElement("CV")]
        [DataMember(Name = "CV")]
        public long CirculatingVolume { get; set; }

        // 상장주식수(Daishin)
        [BsonElement("SV")]
        [DataMember(Name = "SV")]
        public long ShareVolume { get; set; }

        // 자본금(Daishin)
        [BsonElement("LC")]
        [DataMember(Name = "LC")]
        public long ListedCapital { get; set; }

        // 기준가(Daishin)
        [BsonElement("BP")]
        [DataMember(Name = "BP")]
        public float BasisPrice { get; set; }

        // 상한가(Daishin)
        [BsonElement("UL")]
        [DataMember(Name = "UL")]
        public int UpperLimit { get; set; }

        // 하한가(Daishin)
        [BsonElement("LL")]
        [DataMember(Name = "LL")]
        public int LowerLimit { get; set; }

        // 전일종가(Daishin)
        [BsonElement("PCP")]
        [DataMember(Name = "PCP")]
        public float PreviousClosingPrice { get; set; }

        // 전일거래량(Daishin)
        [BsonElement("PV")]
        [DataMember(Name = "PV")]
        public long PreviousVolume { get; set; }

        // 호가단위(Daishin)
        [BsonElement("QU")]
        [DataMember(Name = "QU")]
        public int QuantityUnit { get; set; }

        // 외국인한도(Daishin)
        [BsonElement("FL")]
        [DataMember(Name = "FL")]
        public long ForeigneLimit { get; set; }

        // 외국인잔량(Daishin)
        [BsonElement("FAR")]
        [DataMember(Name = "FAR")]
        public long ForeigneAvailableRemain { get; set; }

        // 외국인한도비율
        [BsonIgnore]
        [IgnoreDataMember]
        public float ForeigneLimitRate { get { return (float)ForeigneLimit / ShareVolume * 100; } }

        // 외국인보유
        [BsonIgnore]
        [IgnoreDataMember]
        public long ForeigneHold { get { return ForeigneLimit - ForeigneAvailableRemain; } }

        // 외국인소진율
        [BsonIgnore]
        [IgnoreDataMember]
        public float ForeigneExhaustingRate { get { return (float)ForeigneHold / ForeigneLimit * 100; } }

        // 외국인잔량률
        [BsonIgnore]
        [IgnoreDataMember]
        public float ForeigneAvailableRemainRate { get { return (float)ForeigneAvailableRemain / ShareVolume * 100; } }

        // 관리(Ebest)
        [BsonElement("AI")]
        [DataMember(Name = "AI")]
        public bool AdministrativeIssue { get; set; }

        // 주의(Ebest)
        [BsonElement("IC")]
        [DataMember(Name = "IC")]
        public bool InvestCaution { get; set; }

        // 경고(Ebest)
        [BsonElement("IW")]
        [DataMember(Name = "IW")]
        public bool InvestWarning { get; set; }

        // 위험예고(Ebest)
        [BsonElement("IRN")]
        [DataMember(Name = "IRN")]
        public bool InvestRiskNoticed { get; set; }

        // 위험(Ebest)
        [BsonElement("IR")]
        [DataMember(Name = "IR")]
        public bool InvestRisk { get; set; }

        // 매매정지(Ebest)
        [BsonElement("TH")]
        [DataMember(Name = "TH")]
        public bool TradingHalt { get; set; }

        // 매매중단(Ebest)
        [BsonElement("TS")]
        [DataMember(Name = "TS")]
        public bool TradingSuspend { get; set; }

        // 불성실공시(Ebest)
        [BsonElement("UA")]
        [DataMember(Name = "UA")]
        public bool UnfairAnnouncement { get; set; }

        // 단기과열(Ebest)
        [BsonElement("Ov")]
        [DataMember(Name = "Ov")]
        public bool Overheated { get; set; }

        // 단기과열지정예고(Ebest)
        [BsonElement("ON")]
        [DataMember(Name = "ON")]
        public bool OverheatNoticed { get; set; }

        // 정리매매(Ebest)
        [BsonElement("CT")]
        [DataMember(Name = "CT")]
        public bool CleaningTrade { get; set; }

        // 투자유의(Ebest)
        [BsonElement("IA")]
        [DataMember(Name = "IA")]
        public bool InvestAttention { get; set; }

        // 투자환기(Ebest)
        [BsonElement("CA")]
        [DataMember(Name = "CA")]
        public bool CallingAttention { get; set; }

        // 자산(Kiwoom & Daishin)
        [BsonElement("As")]
        [DataMember(Name = "As")]
        public double Asset { get; set; }

        // Bookvalue Per Share (Kiwoom)
        [DataMember]
        public double BPS { get; set; }

        // Price Earning Ratio (Kiwoom)
        [DataMember]
        public double PBR { get; set; }

        // 당기순이익(Kiwoom & Daishin)
        [BsonElement("NI")]
        [DataMember(Name = "NI")]
        public double NetIncome { get; set; }

        // Earning Per Share (Kiwoom)
        [DataMember]
        public double EPS { get; set; }

        // Price Earning Ratio (Kiwoom)
        [DataMember]
        public double PER { get; set; }

        // Return On Equity (Kiwoom)
        [DataMember]
        public double ROE { get; set; }

        // Enterprise Value (Kiwoom)
        [DataMember]
        public double EV { get; set; }

        // 락(Ebest)
        [BsonElement("VAT")]
        [DataMember(Name = "VAT")]
        public ValueAlteredTypes ValueAlteredType { get; set; }
    }
}
