using CommonLib.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Firm.Ebest.Block
{
    /// <summary>
    /// 주식 현재가(시세) 조회
    /// </summary>
    public class t1102OutBlock : BlockBase
    {
        [PropertyIgnore]
        public override string BlockName => nameof(t1102OutBlock);

        /// <summary>
        /// 한글명 [20]
        /// </summary>
        public string hname { get; set; }

        /// <summary>
        /// 현재가 [8]
        /// </summary>
        public long price { get; set; }

        /// <summary>
        /// 전일대비구분 [1]
        /// </summary>
        public string sign { get; set; }

        /// <summary>
        /// 전일대비 [8]
        /// </summary>
        public long change { get; set; }

        /// <summary>
        /// 등락율 [6.2]
        /// </summary>
        public float diff { get; set; }

        /// <summary>
        /// 누적거래량 [12]
        /// </summary>
        public long volume { get; set; }

        /// <summary>
        /// 기준가 [8]
        /// </summary>
        public long recprice { get; set; }

        /// <summary>
        /// 가중평균 [8]
        /// </summary>
        public long avg { get; set; }

        /// <summary>
        /// 상한가 [8]
        /// </summary>
        public long uplmtprice { get; set; }

        /// <summary>
        /// 하한가 [8]
        /// </summary>
        public long dnlmtprice { get; set; }

        /// <summary>
        /// 전일거래량 [12]
        /// </summary>
        public long jnilvolume { get; set; }

        /// <summary>
        /// 거래량차 [12]
        /// </summary>
        public long volumediff { get; set; }

        /// <summary>
        /// 시가 [8]
        /// </summary>
        public long open { get; set; }

        /// <summary>
        /// 시가시간 [6]
        /// </summary>
        public string opentime { get; set; }

        /// <summary>
        /// 고가 [8]
        /// </summary>
        public long high { get; set; }

        /// <summary>
        /// 고가시간 [6]
        /// </summary>
        public string hightime { get; set; }

        /// <summary>
        /// 저가 [8]
        /// </summary>
        public long low { get; set; }

        /// <summary>
        /// 저가시간 [6]
        /// </summary>
        public string lowtime { get; set; }

        /// <summary>
        /// 52최고가 [8]
        /// </summary>
        public long high52w { get; set; }

        /// <summary>
        /// 52최고가일 [8]
        /// </summary>
        public string high52wdate { get; set; }

        /// <summary>
        /// 52최저가 [8]
        /// </summary>
        public long low52w { get; set; }

        /// <summary>
        /// 52최저가일 [8]
        /// </summary>
        public string low52wdate { get; set; }

        /// <summary>
        /// 소진율 [6.2]
        /// </summary>
        public float exhratio { get; set; }

        /// <summary>
        /// PER [6.2]
        /// </summary>
        public float per { get; set; }

        /// <summary>
        /// PBRX [6.2]
        /// </summary>
        public float pbrx { get; set; }

        /// <summary>
        /// 상장주식수(천) [12]
        /// </summary>
        public long listing { get; set; }

        /// <summary>
        /// 증거금율 [8]
        /// </summary>
        public long jkrate { get; set; }

        /// <summary>
        /// 수량단위 [5]
        /// </summary>
        public string memedan { get; set; }

        /// <summary>
        /// 매도증권사코드1 [3]
        /// </summary>
        [PropertyIgnore]
        public string offernocd1 { get; set; }

        /// <summary>
        /// 매수증권사코드1 [3]
        /// </summary>
        [PropertyIgnore]
        public string bidnocd1 { get; set; }

        /// <summary>
        /// 매도증권사명1 [6]
        /// </summary>
        [PropertyIgnore]
        public string offerno1 { get; set; }

        /// <summary>
        /// 매수증권사명1 [6]
        /// </summary>
        [PropertyIgnore]
        public string bidno1 { get; set; }

        /// <summary>
        /// 총매도수량1 [8]
        /// </summary>
        public long dvol1 { get; set; }

        /// <summary>
        /// 총매수수량1 [8]
        /// </summary>
        public long svol1 { get; set; }

        /// <summary>
        /// 매도증감1 [8]
        /// </summary>
        public long dcha1 { get; set; }

        /// <summary>
        /// 매수증감1 [8]
        /// </summary>
        public long scha1 { get; set; }

        /// <summary>
        /// 매도비율1 [6.2]
        /// </summary>
        public float ddiff1 { get; set; }

        /// <summary>
        /// 매수비율1 [6.2]
        /// </summary>
        public float sdiff1 { get; set; }

        /// <summary>
        /// 매도증권사코드2 [3]
        /// </summary>
        [PropertyIgnore]
        public string offernocd2 { get; set; }

        /// <summary>
        /// 매수증권사코드2 [3]
        /// </summary>
        [PropertyIgnore]
        public string bidnocd2 { get; set; }

        /// <summary>
        /// 매도증권사명2 [6]
        /// </summary>
        [PropertyIgnore]
        public string offerno2 { get; set; }

        /// <summary>
        /// 매수증권사명2 [6]
        /// </summary>
        [PropertyIgnore]
        public string bidno2 { get; set; }

        /// <summary>
        /// 총매도수량2 [8]
        /// </summary>
        public long dvol2 { get; set; }

        /// <summary>
        /// 총매수수량2 [8]
        /// </summary>
        public long svol2 { get; set; }

        /// <summary>
        /// 매도증감2 [8]
        /// </summary>
        public long dcha2 { get; set; }

        /// <summary>
        /// 매수증감2 [8]
        /// </summary>
        public long scha2 { get; set; }

        /// <summary>
        /// 매도비율2 [6.2]
        /// </summary>
        public float ddiff2 { get; set; }

        /// <summary>
        /// 매수비율2 [6.2]
        /// </summary>
        public float sdiff2 { get; set; }

        /// <summary>
        /// 매도증권사코드3 [3]
        /// </summary>
        [PropertyIgnore]
        public string offernocd3 { get; set; }

        /// <summary>
        /// 매수증권사코드3 [3]
        /// </summary>
        [PropertyIgnore]
        public string bidnocd3 { get; set; }

        /// <summary>
        /// 매도증권사명3 [6]
        /// </summary>
        [PropertyIgnore]
        public string offerno3 { get; set; }

        /// <summary>
        /// 매수증권사명3 [6]
        /// </summary>
        [PropertyIgnore]
        public string bidno3 { get; set; }

        /// <summary>
        /// 총매도수량3 [8]
        /// </summary>
        public long dvol3 { get; set; }

        /// <summary>
        /// 총매수수량3 [8]
        /// </summary>
        public long svol3 { get; set; }

        /// <summary>
        /// 매도증감3 [8]
        /// </summary>
        public long dcha3 { get; set; }

        /// <summary>
        /// 매수증감3 [8]
        /// </summary>
        public long scha3 { get; set; }

        /// <summary>
        /// 매도비율3 [6.2]
        /// </summary>
        public float ddiff3 { get; set; }

        /// <summary>
        /// 매수비율3 [6.2]
        /// </summary>
        public float sdiff3 { get; set; }

        /// <summary>
        /// 매도증권사코드4 [3]
        /// </summary>
        [PropertyIgnore]
        public string offernocd4 { get; set; }

        /// <summary>
        /// 매수증권사코드4 [3]
        /// </summary>
        [PropertyIgnore]
        public string bidnocd4 { get; set; }

        /// <summary>
        /// 매도증권사명4 [6]
        /// </summary>
        [PropertyIgnore]
        public string offerno4 { get; set; }

        /// <summary>
        /// 매수증권사명4 [6]
        /// </summary>
        [PropertyIgnore]
        public string bidno4 { get; set; }

        /// <summary>
        /// 총매도수량4 [8]
        /// </summary>
        public long dvol4 { get; set; }

        /// <summary>
        /// 총매수수량4 [8]
        /// </summary>
        public long svol4 { get; set; }

        /// <summary>
        /// 매도증감4 [8]
        /// </summary>
        public long dcha4 { get; set; }

        /// <summary>
        /// 매수증감4 [8]
        /// </summary>
        public long scha4 { get; set; }

        /// <summary>
        /// 매도비율4 [6.2]
        /// </summary>
        public float ddiff4 { get; set; }

        /// <summary>
        /// 매수비율4 [6.2]
        /// </summary>
        public float sdiff4 { get; set; }

        /// <summary>
        /// 매도증권사코드5 [3]
        /// </summary>
        [PropertyIgnore]
        public string offernocd5 { get; set; }

        /// <summary>
        /// 매수증권사코드5 [3]
        /// </summary>
        [PropertyIgnore]
        public string bidnocd5 { get; set; }

        /// <summary>
        /// 매도증권사명5 [6]
        /// </summary>
        [PropertyIgnore]
        public string offerno5 { get; set; }

        /// <summary>
        /// 매수증권사명5 [6]
        /// </summary>
        [PropertyIgnore]
        public string bidno5 { get; set; }

        /// <summary>
        /// 총매도수량5 [8]
        /// </summary>
        public long dvol5 { get; set; }

        /// <summary>
        /// 총매수수량5 [8]
        /// </summary>
        public long svol5 { get; set; }

        /// <summary>
        /// 매도증감5 [8]
        /// </summary>
        public long dcha5 { get; set; }

        /// <summary>
        /// 매수증감5 [8]
        /// </summary>
        public long scha5 { get; set; }

        /// <summary>
        /// 매도비율5 [6.2]
        /// </summary>
        public float ddiff5 { get; set; }

        /// <summary>
        /// 매수비율5 [6.2]
        /// </summary>
        public float sdiff5 { get; set; }

        /// <summary>
        /// 외국계매도합계수량 [12]
        /// </summary>
        public long fwdvl { get; set; }

        /// <summary>
        /// 외국계매도직전대비 [12]
        /// </summary>
        public long ftradmdcha { get; set; }

        /// <summary>
        /// 외국계매도비율 [6.2]
        /// </summary>
        public float ftradmddiff { get; set; }

        /// <summary>
        /// 외국계매수합계수량 [12]
        /// </summary>
        public long fwsvl { get; set; }

        /// <summary>
        /// 외국계매수직전대비 [12]
        /// </summary>
        public long ftradmscha { get; set; }

        /// <summary>
        /// 외국계매수비율 [6.2]
        /// </summary>
        public float ftradmsdiff { get; set; }

        /// <summary>
        /// 회전율 [6.2]
        /// </summary>
        public float vol { get; set; }

        /// <summary>
        /// 단축코드 [6]
        /// </summary>
        public string shcode { get; set; }

        /// <summary>
        /// 누적거래대금 [12]
        /// </summary>
        public long value { get; set; }

        /// <summary>
        /// 전일동시간거래량 [12]
        /// </summary>
        public long jvolume { get; set; }

        /// <summary>
        /// 연중최고가 [8]
        /// </summary>
        public long highyear { get; set; }

        /// <summary>
        /// 연중최고일자 [8]
        /// </summary>
        public string highyeardate { get; set; }

        /// <summary>
        /// 연중최저가 [8]
        /// </summary>
        public long lowyear { get; set; }

        /// <summary>
        /// 연중최저일자 [8]
        /// </summary>
        public string lowyeardate { get; set; }

        /// <summary>
        /// 목표가 [8]
        /// </summary>
        public long target { get; set; }

        /// <summary>
        /// 자본금 [12]
        /// </summary>
        public long capital { get; set; }

        /// <summary>
        /// 유동주식수 [12]
        /// </summary>
        public long abscnt { get; set; }

        /// <summary>
        /// 액면가 [8]
        /// </summary>
        public long parprice { get; set; }

        /// <summary>
        /// 결산월 [2]
        /// </summary>
        [PropertyIgnore]
        public string gsmm { get; set; }

        /// <summary>
        /// 대용가 [8]
        /// </summary>
        public long subprice { get; set; }

        /// <summary>
        /// 시가총액 [12]
        /// </summary>
        public long total { get; set; }

        /// <summary>
        /// 상장일 [8]
        /// </summary>
        public string listdate { get; set; }

        /// <summary>
        /// 전분기명 [10]
        /// </summary>
        [PropertyIgnore]
        public string name { get; set; }

        /// <summary>
        /// 전분기매출액 [12]
        /// </summary>
        public long bfsales { get; set; }

        /// <summary>
        /// 전분기영업이익 [12]
        /// </summary>
        public long bfoperatingincome { get; set; }

        /// <summary>
        /// 전분기경상이익 [12]
        /// </summary>
        public long bfordinaryincome { get; set; }

        /// <summary>
        /// 전분기순이익 [12]
        /// </summary>
        public long bfnetincome { get; set; }

        /// <summary>
        /// 전분기EPS [13.2]
        /// </summary>
        public float bfeps { get; set; }

        /// <summary>
        /// 전전분기명 [10]
        /// </summary>
        public string name2 { get; set; }

        /// <summary>
        /// 전전분기매출액 [12]
        /// </summary>
        public long bfsales2 { get; set; }

        /// <summary>
        /// 전전분기영업이익 [12]
        /// </summary>
        public long bfoperatingincome2 { get; set; }

        /// <summary>
        /// 전전분기경상이익 [12]
        /// </summary>
        public long bfordinaryincome2 { get; set; }

        /// <summary>
        /// 전전분기순이익 [12]
        /// </summary>
        public long bfnetincome2 { get; set; }

        /// <summary>
        /// 전전분기EPS [13.2]
        /// </summary>
        public float bfeps2 { get; set; }

        /// <summary>
        /// 전년대비매출액 [7.2]
        /// </summary>
        public float salert { get; set; }

        /// <summary>
        /// 전년대비영업이익 [7.2]
        /// </summary>
        public float opert { get; set; }

        /// <summary>
        /// 전년대비경상이익 [7.2]
        /// </summary>
        public float ordrt { get; set; }

        /// <summary>
        /// 전년대비순이익 [7.2]
        /// </summary>
        public float netrt { get; set; }

        /// <summary>
        /// 전년대비EPS [7.2]
        /// </summary>
        public float epsrt { get; set; }

        /// <summary>
        /// 락구분 [10]
        /// </summary>
        public string info1 { get; set; }

        /// <summary>
        /// 관리/급등구분 [10]
        /// </summary>
        public string info2 { get; set; }

        /// <summary>
        /// 정지/연장구분 [10]
        /// </summary>
        public string info3 { get; set; }

        /// <summary>
        /// 투자/불성실구분 [12]
        /// </summary>
        public string info4 { get; set; }

        /// <summary>
        /// 장구분 [10]
        /// </summary>
        public string janginfo { get; set; }

        /// <summary>
        /// T.PER [6.2]
        /// </summary>
        public float t_per { get; set; }

        /// <summary>
        /// 통화ISO코드 [3]
        /// </summary>
        public string tonghwa { get; set; }

        /// <summary>
        /// 총매도대금1 [18]
        /// </summary>
        public long dval1 { get; set; }

        /// <summary>
        /// 총매수대금1 [18]
        /// </summary>
        public long sval1 { get; set; }

        /// <summary>
        /// 총매도대금2 [18]
        /// </summary>
        public long dval2 { get; set; }

        /// <summary>
        /// 총매수대금2 [18]
        /// </summary>
        public long sval2 { get; set; }

        /// <summary>
        /// 총매도대금3 [18]
        /// </summary>
        public long dval3 { get; set; }

        /// <summary>
        /// 총매수대금3 [18]
        /// </summary>
        public long sval3 { get; set; }

        /// <summary>
        /// 총매도대금4 [18]
        /// </summary>
        public long dval4 { get; set; }

        /// <summary>
        /// 총매수대금4 [18]
        /// </summary>
        public long sval4 { get; set; }

        /// <summary>
        /// 총매도대금5 [18]
        /// </summary>
        public long dval5 { get; set; }

        /// <summary>
        /// 총매수대금5 [18]
        /// </summary>
        public long sval5 { get; set; }

        /// <summary>
        /// 총매도평단가1 [8]
        /// </summary>
        public long davg1 { get; set; }

        /// <summary>
        /// 총매수평단가1 [8]
        /// </summary>
        public long savg1 { get; set; }

        /// <summary>
        /// 총매도평단가2 [8]
        /// </summary>
        public long davg2 { get; set; }

        /// <summary>
        /// 총매수평단가2 [8]
        /// </summary>
        public long savg2 { get; set; }

        /// <summary>
        /// 총매도평단가3 [8]
        /// </summary>
        public long davg3 { get; set; }

        /// <summary>
        /// 총매수평단가3 [8]
        /// </summary>
        public long savg3 { get; set; }

        /// <summary>
        /// 총매도평단가4 [8]
        /// </summary>
        public long davg4 { get; set; }

        /// <summary>
        /// 총매수평단가4 [8]
        /// </summary>
        public long savg4 { get; set; }

        /// <summary>
        /// 총매도평단가5 [8]
        /// </summary>
        public long davg5 { get; set; }

        /// <summary>
        /// 총매수평단가5 [8]
        /// </summary>
        public long savg5 { get; set; }

        /// <summary>
        /// 외국계매도대금 [18]
        /// </summary>
        public long ftradmdval { get; set; }

        /// <summary>
        /// 외국계매수대금 [18]
        /// </summary>
        public long ftradmsval { get; set; }

        /// <summary>
        /// 외국계매도평단가 [8]
        /// </summary>
        public long ftradmdvag { get; set; }

        /// <summary>
        /// 외국계매수평단가 [8]
        /// </summary>
        public long ftradmsvag { get; set; }

        /// <summary>
        /// 투자주의환기 [8]
        /// </summary>
        public string info5 { get; set; }

        /// <summary>
        /// 기업인수목적회사여부 [1]
        /// </summary>
        public string spac_gubun { get; set; }

        /// <summary>
        /// 발행가격 [8]
        /// </summary>
        public long issueprice { get; set; }

        /// <summary>
        /// 배분적용구분코드(1:배분발생2:배분해제그외:미발생) [1]
        /// </summary>
        public string alloc_gubun { get; set; }

        /// <summary>
        /// 배분적용구분 [8]
        /// </summary>
        public string alloc_text { get; set; }

        /// <summary>
        /// 단기과열/VI발동 [10]
        /// </summary>
        public string shterm_text { get; set; }

        /// <summary>
        /// 정적VI상한가 [8]
        /// </summary>
        public long svi_uplmtprice { get; set; }

        /// <summary>
        /// 정적VI하한가 [8]
        /// </summary>
        public long svi_dnlmtprice { get; set; }
    }
}
