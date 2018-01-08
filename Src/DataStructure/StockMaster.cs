using MongoDB.Bson.Serialization.Attributes;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataStructure
{
    [DataContract]
    [ProtoContract]
    public class StockMaster : Subscribable
    {
        // 종목명(Daishin)
        [BsonElement("N")]
        [DataMember(Name = "N")]
        [ProtoMember(1)]
        public string Name { get; set; }

        [BsonElement("MT")]
        [DataMember(Name = "MT")]
        [ProtoMember(2)]
        public MarketTypes MarketType { get; set; }

        // 결산월(Daishin)
        [BsonElement("SM")]
        [DataMember(Name = "SM")]
        [ProtoMember(3)]
        public int SettlementMonth { get; set; }

        // 액면가(Daishin)
        [BsonElement("FV")]
        [DataMember(Name = "FV")]
        [ProtoMember(4)]
        public int FaceValue { get; set; }

        // 상장일(Ebest)
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        [BsonElement("LD")]
        [DataMember(Name = "LD")]
        [ProtoMember(5)]
        public DateTime ListedDate { get; set; }

        // 유동주식수(Ebest)
        [BsonElement("CV")]
        [DataMember(Name = "CV")]
        [ProtoMember(6)]
        public long CirculatingVolume { get; set; }

        // 상장주식수(Daishin)
        [BsonElement("SV")]
        [DataMember(Name = "SV")]
        [ProtoMember(7)]
        public long ShareVolume { get; set; }

        // 자본금(Daishin)
        [BsonElement("LC")]
        [DataMember(Name = "LC")]
        [ProtoMember(8)]
        public long ListedCapital { get; set; }

        // 기준가(Daishin)
        [BsonElement("BP")]
        [DataMember(Name = "BP")]
        [ProtoMember(9)]
        public float BasisPrice { get; set; }

        // 상한가(Daishin)
        [BsonElement("UL")]
        [DataMember(Name = "UL")]
        [ProtoMember(10)]
        public int UpperLimit { get; set; }

        // 하한가(Daishin)
        [BsonElement("LL")]
        [DataMember(Name = "LL")]
        [ProtoMember(11)]
        public int LowerLimit { get; set; }

        // 전일종가(Daishin)
        [BsonElement("PCP")]
        [DataMember(Name = "PCP")]
        [ProtoMember(12)]
        public float PreviousClosingPrice { get; set; }

        // 전일거래량(Daishin)
        [BsonElement("PV")]
        [DataMember(Name = "PV")]
        [ProtoMember(13)]
        public long PreviousVolume { get; set; }

        // 호가단위(Daishin)
        [BsonElement("QU")]
        [DataMember(Name = "QU")]
        [ProtoMember(14)]
        public int QuantityUnit { get; set; }

        // 외국인한도(Daishin)
        [BsonElement("FL")]
        [DataMember(Name = "FL")]
        [ProtoMember(15)]
        public long ForeigneLimit { get; set; }

        // 외국인잔량(Daishin)
        [BsonElement("FAR")]
        [DataMember(Name = "FAR")]
        [ProtoMember(16)]
        public long ForeigneAvailableRemain { get; set; }

        // 외국인한도비율
        [BsonIgnore]
        [IgnoreDataMember]
        [ProtoIgnore]
        public float ForeigneLimitRate { get { return (float)ForeigneLimit / ShareVolume * 100; } }

        // 외국인보유
        [BsonIgnore]
        [IgnoreDataMember]
        [ProtoIgnore]
        public long ForeigneHold { get { return ForeigneLimit - ForeigneAvailableRemain; } }

        // 외국인소진율
        [BsonIgnore]
        [IgnoreDataMember]
        [ProtoIgnore]
        public float ForeigneExhaustingRate { get { return (float)ForeigneHold / ForeigneLimit * 100; } }

        // 외국인잔량률
        [BsonIgnore]
        [IgnoreDataMember]
        [ProtoIgnore]
        public float ForeigneAvailableRemainRate { get { return (float)ForeigneAvailableRemain / ShareVolume * 100; } }

        // 관리(Ebest)
        [BsonElement("AI")]
        [DataMember(Name = "AI")]
        [ProtoMember(17)]
        public bool AdministrativeIssue { get; set; }

        // 주의(Ebest)
        [BsonElement("IC")]
        [DataMember(Name = "IC")]
        [ProtoMember(18)]
        public bool InvestCaution { get; set; }

        // 경고(Ebest)
        [BsonElement("IW")]
        [DataMember(Name = "IW")]
        [ProtoMember(19)]
        public bool InvestWarning { get; set; }

        // 위험예고(Ebest)
        [BsonElement("IRN")]
        [DataMember(Name = "IRN")]
        [ProtoMember(20)]
        public bool InvestRiskNoticed { get; set; }

        // 위험(Ebest)
        [BsonElement("IR")]
        [DataMember(Name = "IR")]
        [ProtoMember(21)]
        public bool InvestRisk { get; set; }

        // 매매정지(Ebest)
        [BsonElement("TH")]
        [DataMember(Name = "TH")]
        [ProtoMember(22)]
        public bool TradingHalt { get; set; }

        // 매매중단(Ebest)
        [BsonElement("TS")]
        [DataMember(Name = "TS")]
        [ProtoMember(23)]
        public bool TradingSuspend { get; set; }

        // 불성실공시(Ebest)
        [BsonElement("UA")]
        [DataMember(Name = "UA")]
        [ProtoMember(24)]
        public bool UnfairAnnouncement { get; set; }

        // 단기과열(Ebest)
        [BsonElement("Ov")]
        [DataMember(Name = "Ov")]
        [ProtoMember(25)]
        public bool Overheated { get; set; }

        // 단기과열지정예고(Ebest)
        [BsonElement("ON")]
        [DataMember(Name = "ON")]
        [ProtoMember(26)]
        public bool OverheatNoticed { get; set; }

        // 정리매매(Ebest)
        [BsonElement("CT")]
        [DataMember(Name = "CT")]
        [ProtoMember(27)]
        public bool CleaningTrade { get; set; }

        // 투자유의(Ebest)
        [BsonElement("IA")]
        [DataMember(Name = "IA")]
        [ProtoMember(28)]
        public bool InvestAttention { get; set; }

        // 투자환기(Ebest)
        [BsonElement("CA")]
        [DataMember(Name = "CA")]
        [ProtoMember(29)]
        public bool CallingAttention { get; set; }

        // 자산(Kiwoom & Daishin)
        [BsonElement("As")]
        [DataMember(Name = "As")]
        [ProtoMember(30)]
        public double Asset { get; set; }

        // Bookvalue Per Share (Kiwoom)
        [DataMember]
        [ProtoMember(31)]
        public double BPS { get; set; }

        // Price Earning Ratio (Kiwoom)
        [DataMember]
        [ProtoMember(32)]
        public double PBR { get; set; }

        // 당기순이익(Kiwoom & Daishin)
        [BsonElement("NI")]
        [DataMember(Name = "NI")]
        [ProtoMember(33)]
        public double NetIncome { get; set; }

        // Earning Per Share (Kiwoom)
        [DataMember]
        [ProtoMember(34)]
        public double EPS { get; set; }

        // Price Earning Ratio (Kiwoom)
        [DataMember]
        [ProtoMember(35)]
        public double PER { get; set; }

        // Return On Equity (Kiwoom)
        [DataMember]
        [ProtoMember(36)]
        public double ROE { get; set; }

        // Enterprise Value (Kiwoom)
        [DataMember]
        [ProtoMember(37)]
        public double EV { get; set; }

        // 락(Ebest)
        [BsonElement("VAT")]
        [DataMember(Name = "VAT")]
        [ProtoMember(38)]
        public ValueAlteredTypes ValueAlteredType { get; set; }

        public override string ToString(params string[] excludeProperties)
        {
            return ToString(typeof(StockMaster), excludeProperties);
        }
    }
}
