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
    public class StockMaster
    {
        /// <summary>
        /// 종목명(대신)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 결산월(대신)
        /// </summary>
        public int SettlementMonth { get; set; }

        /// <summary>
        /// 액면가(대신)
        /// </summary>
        public int FaceValue { get; set; }

        /// <summary>
        /// 상장일(ebest)
        /// </summary>
        public DateTime ListedDate { get; set; }

        /// <summary>
        /// 유동주식수(ebest)
        /// </summary>
        public long CirculatingVolume { get; set; }

        /// <summary>
        /// 상장주식수(대신)
        /// </summary>
        public long ShareVolume { get; set; }

        /// <summary>
        /// 자본금(대신)
        /// </summary>
        public long ListedCapital { get; set; }

        /// <summary>
        /// 기준가(대신)
        /// </summary>
        public float BasisPrice { get; set; }

        /// <summary>
        /// 상한가(대신)
        /// </summary>
        public int UpperLimit { get; set; }

        /// <summary>
        /// 하한가(대신)
        /// </summary>
        public int LowerLimit { get; set; }

        /// <summary>
        /// 전일종가
        /// </summary>
        public float PreviousClosedPrice { get; set; }

        /// <summary>
        /// 전일거래량(대신)
        /// </summary>
        public long PreviousVolume { get; set; }

        /// <summary>
        /// 호가단위(대신)
        /// </summary>
        public int QuantityUnit { get; set; }

        /// <summary>
        /// 외국인한도(대신)
        /// </summary>
        public long ForeigneLimit { get; set; }

        /// <summary>
        /// 외국인잔량(대신)
        /// </summary>
        public long ForeigneAvailableRemain { get; set; }

        /// <summary>
        /// 외국인한도비율((float)ForeigneLimit / ShareVolume * 100)
        /// </summary>
        public float ForeigneLimitRate { get; set; }

        /// <summary>
        /// 외국인보유(ForeigneLimit - ForeigneAvailableRemain)
        /// </summary>
        public long ForeigneHold { get; set; }

        /// <summary>
        /// 외국인소진율((float)ForeigneHold / ForeigneLimit * 100)
        /// </summary>
        public float ForeigneExhaustingRate { get; set; }

        /// <summary>
        /// 외국인잔량률((float)ForeigneAvailableRemain / ShareVolume * 100)
        /// </summary>
        public float ForeigneAvailableRemainRate { get; set; }

        /// <summary>
        /// 매매정지(KRX)
        /// </summary>
        public InvestWarning TradingHalt { get; set; }

        /// <summary>
        /// 관리(KRX)
        /// </summary>
        public InvestWarning AdministrativeIssue { get; set; }

        /// <summary>
        /// 주의(KRX)
        /// </summary>
        public InvestWarning InvestCaution { get; set; }

        /// <summary>
        /// 경고(KRX)
        /// </summary>
        public InvestWarning InvestWarning { get; set; } // TODO : InvestWarning 이름 중복됨

        /// <summary>
        /// 위험(KRX)
        /// </summary>
        public InvestWarning InvestmentRisk { get; set; }

        /// <summary>
        /// 불성실공시(KRX)
        /// </summary>
        public InvestWarning UnfairAnnouncement { get; set; }

        /// <summary>
        /// 주의환기(KRX)
        /// </summary>
        public InvestWarning CallingAttention { get; set; }

        /// <summary>
        /// Overheated(KRX)
        /// </summary>
        public InvestWarning CleaningTrade { get; set; }

        /// <summary>
        /// 단기과열(KRX)
        /// </summary>
        public InvestWarning Overheated { get; set; }

        /// <summary>
        /// 자산(Naver)
        /// </summary>
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
        public ValueAlteredType ValueAltered { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(base.ToString());
            sb.AppendLine($"Name: {Name}");

            return sb.ToString();
        }
    }
}
