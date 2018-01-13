using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Firm.Ebest.Block
{
    /// <summary>
    /// 주식주문접수
    /// </summary>
    public class SC0OutBlock : BlockBase
    {
        public override string BlockName => "OutBlock";

        /// <summary>
        /// 라인일련번호 [10]
        /// </summary>
        public long lineseq { get; set; }

        /// <summary>
        /// 계좌번호 [11]
        /// </summary>
        public string accno { get; set; }

        /// <summary>
        /// 조작자ID [8]
        /// </summary>
        public string user { get; set; }

        /// <summary>
        /// 헤더길이 [6]
        /// </summary>
        public long len { get; set; }

        /// <summary>
        /// 헤더구분 [1]
        /// </summary>
        public string gubun { get; set; }

        /// <summary>
        /// 압축구분 [1]
        /// </summary>
        public string compress { get; set; }

        /// <summary>
        /// 암호구분 [1]
        /// </summary>
        public string encrypt { get; set; }

        /// <summary>
        /// 공통시작지점 [3]
        /// </summary>
        public long offset { get; set; }

        /// <summary>
        /// TRCODE [8]
        /// </summary>
        public string trcode { get; set; }

        /// <summary>
        /// 이용사번호 [3]
        /// </summary>
        public string comid { get; set; }

        /// <summary>
        /// 사용자ID [16]
        /// </summary>
        public string userid { get; set; }

        /// <summary>
        /// 접속매체 [2]
        /// </summary>
        public string media { get; set; }

        /// <summary>
        /// I/F일련번호 [3]
        /// </summary>
        public string ifid { get; set; }

        /// <summary>
        /// 전문일련번호 [9]
        /// </summary>
        public string seq { get; set; }

        /// <summary>
        /// TR추적ID [16]
        /// </summary>
        public string trid { get; set; }

        /// <summary>
        /// 공인IP [12]
        /// </summary>
        public string pubip { get; set; }

        /// <summary>
        /// 사설IP [12]
        /// </summary>
        public string prvip { get; set; }

        /// <summary>
        /// 처리지점번호 [3]
        /// </summary>
        public string pcbpno { get; set; }

        /// <summary>
        /// 지점번호 [3]
        /// </summary>
        public string bpno { get; set; }

        /// <summary>
        /// 단말번호 [8]
        /// </summary>
        public string termno { get; set; }

        /// <summary>
        /// 언어구분 [1]
        /// </summary>
        public string lang { get; set; }

        /// <summary>
        /// AP처리시간 [9]
        /// </summary>
        public long proctm { get; set; }

        /// <summary>
        /// 메세지코드 [4]
        /// </summary>
        public string msgcode { get; set; }

        /// <summary>
        /// 메세지출력구분 [1]
        /// </summary>
        public string outgu { get; set; }

        /// <summary>
        /// 압축요청구분 [1]
        /// </summary>
        public string compreq { get; set; }

        /// <summary>
        /// 기능키 [4]
        /// </summary>
        public string funckey { get; set; }

        /// <summary>
        /// 요청레코드개수 [4]
        /// </summary>
        public long reqcnt { get; set; }

        /// <summary>
        /// 예비영역 [6]
        /// </summary>
        public string filler { get; set; }

        /// <summary>
        /// 연속구분 [1]
        /// </summary>
        public string cont { get; set; }

        /// <summary>
        /// 연속키값 [18]
        /// </summary>
        public string contkey { get; set; }

        /// <summary>
        /// 가변시스템길이 [2]
        /// </summary>
        public long varlen { get; set; }

        /// <summary>
        /// 가변해더길이 [2]
        /// </summary>
        public long varhdlen { get; set; }

        /// <summary>
        /// 가변메시지길이 [2]
        /// </summary>
        public long varmsglen { get; set; }

        /// <summary>
        /// 조회발원지 [1]
        /// </summary>
        public string trsrc { get; set; }

        /// <summary>
        /// I/F이벤트ID [4]
        /// </summary>
        public string eventid { get; set; }

        /// <summary>
        /// I/F정보 [4]
        /// </summary>
        public string ifinfo { get; set; }

        /// <summary>
        /// 예비영역 [41]
        /// </summary>
        public string filler1 { get; set; }

        /// <summary>
        /// 주문체결구분 [2]
        /// </summary>
        public string ordchegb { get; set; }

        /// <summary>
        /// 시장구분 [2]
        /// </summary>
        public string marketgb { get; set; }

        /// <summary>
        /// 주문구분 [2]
        /// </summary>
        public string ordgb { get; set; }

        /// <summary>
        /// 원주문번호 [10]
        /// </summary>
        public long orgordno { get; set; }

        /// <summary>
        /// 계좌번호 [11]
        /// </summary>
        public string accno1 { get; set; }

        /// <summary>
        /// 계좌번호 [9]
        /// </summary>
        public string accno2 { get; set; }

        /// <summary>
        /// 비밀번호 [8]
        /// </summary>
        public string passwd { get; set; }

        /// <summary>
        /// 종목번호 [12]
        /// </summary>
        public string expcode { get; set; }

        /// <summary>
        /// 단축종목번호 [9]
        /// </summary>
        public string shtcode { get; set; }

        /// <summary>
        /// 종목명 [40]
        /// </summary>
        public string hname { get; set; }

        /// <summary>
        /// 주문수량 [16]
        /// </summary>
        public long ordqty { get; set; }

        /// <summary>
        /// 주문가격 [13]
        /// </summary>
        public long ordprice { get; set; }

        /// <summary>
        /// 주문조건 [1]
        /// </summary>
        public string hogagb { get; set; }

        /// <summary>
        /// 호가유형코드 [2]
        /// </summary>
        public string etfhogagb { get; set; }

        /// <summary>
        /// 프로그램호가구분 [2]
        /// </summary>
        public long pgmtype { get; set; }

        /// <summary>
        /// 공매도호가구분 [1]
        /// </summary>
        public long gmhogagb { get; set; }

        /// <summary>
        /// 공매도가능여부 [1]
        /// </summary>
        public long gmhogayn { get; set; }

        /// <summary>
        /// 신용구분 [3]
        /// </summary>
        public string singb { get; set; }

        /// <summary>
        /// 대출일 [8]
        /// </summary>
        public string loandt { get; set; }

        /// <summary>
        /// 반대매매주문구분 [1]
        /// </summary>
        public string cvrgordtp { get; set; }

        /// <summary>
        /// 전략코드 [6]
        /// </summary>
        public string strtgcode { get; set; }

        /// <summary>
        /// 그룹ID [20]
        /// </summary>
        public string groupid { get; set; }

        /// <summary>
        /// 주문회차 [10]
        /// </summary>
        public long ordseqno { get; set; }

        /// <summary>
        /// 포트폴리오번호 [10]
        /// </summary>
        public long prtno { get; set; }

        /// <summary>
        /// 바스켓번호 [10]
        /// </summary>
        public long basketno { get; set; }

        /// <summary>
        /// 트렌치번호 [10]
        /// </summary>
        public long trchno { get; set; }

        /// <summary>
        /// 아아템번호 [10]
        /// </summary>
        public long itemno { get; set; }

        /// <summary>
        /// 차입구분 [1]
        /// </summary>
        public long brwmgmyn { get; set; }

        /// <summary>
        /// 회원사번호 [3]
        /// </summary>
        public long mbrno { get; set; }

        /// <summary>
        /// 처리구분 [1]
        /// </summary>
        public string procgb { get; set; }

        /// <summary>
        /// 관리지점번호 [3]
        /// </summary>
        public string admbrchno { get; set; }

        /// <summary>
        /// 선물계좌번호 [20]
        /// </summary>
        public string futaccno { get; set; }

        /// <summary>
        /// 선물상품구분 [1]
        /// </summary>
        public string futmarketgb { get; set; }

        /// <summary>
        /// 통신매체구분 [2]
        /// </summary>
        public string tongsingb { get; set; }

        /// <summary>
        /// 유동성공급자구분 [1]
        /// </summary>
        public string lpgb { get; set; }

        /// <summary>
        /// DUMMY [20]
        /// </summary>
        public string dummy { get; set; }

        /// <summary>
        /// 주문번호 [10]
        /// </summary>
        public long ordno { get; set; }

        /// <summary>
        /// 주문시각 [9]
        /// </summary>
        public string ordtm { get; set; }

        /// <summary>
        /// 모주문번호 [10]
        /// </summary>
        public long prntordno { get; set; }

        /// <summary>
        /// 관리사원번호 [9]
        /// </summary>
        public string mgempno { get; set; }

        /// <summary>
        /// 원주문미체결수량 [16]
        /// </summary>
        public long orgordundrqty { get; set; }

        /// <summary>
        /// 원주문정정수량 [16]
        /// </summary>
        public long orgordmdfyqty { get; set; }

        /// <summary>
        /// 원주문취소수량 [16]
        /// </summary>
        public long ordordcancelqty { get; set; }

        /// <summary>
        /// 비회원사송신번호 [10]
        /// </summary>
        public long nmcpysndno { get; set; }

        /// <summary>
        /// 주문금액 [16]
        /// </summary>
        public long ordamt { get; set; }

        /// <summary>
        /// 매매구분 [1]
        /// </summary>
        public string bnstp { get; set; }

        /// <summary>
        /// 예비주문번호 [10]
        /// </summary>
        public long spareordno { get; set; }

        /// <summary>
        /// 반대매매일련번호 [10]
        /// </summary>
        public long cvrgseqno { get; set; }

        /// <summary>
        /// 예약주문번호 [10]
        /// </summary>
        public long rsvordno { get; set; }

        /// <summary>
        /// 복수주문일련번호 [10]
        /// </summary>
        public long mtordseqno { get; set; }

        /// <summary>
        /// 예비주문수량 [16]
        /// </summary>
        public long spareordqty { get; set; }

        /// <summary>
        /// 주문사원번호 [16]
        /// </summary>
        public string orduserid { get; set; }

        /// <summary>
        /// 실물주문수량 [16]
        /// </summary>
        public long spotordqty { get; set; }

        /// <summary>
        /// 재사용주문수량 [16]
        /// </summary>
        public long ordruseqty { get; set; }

        /// <summary>
        /// 현금주문금액 [16]
        /// </summary>
        public long mnyordamt { get; set; }

        /// <summary>
        /// 주문대용금액 [16]
        /// </summary>
        public long ordsubstamt { get; set; }

        /// <summary>
        /// 재사용주문금액 [16]
        /// </summary>
        public long ruseordamt { get; set; }

        /// <summary>
        /// 수수료주문금액 [16]
        /// </summary>
        public long ordcmsnamt { get; set; }

        /// <summary>
        /// 사용신용담보재사용금 [16]
        /// </summary>
        public long crdtuseamt { get; set; }

        /// <summary>
        /// 잔고수량 [16]
        /// </summary>
        public long secbalqty { get; set; }

        /// <summary>
        /// 실물가능수량 [16]
        /// </summary>
        public long spotordableqty { get; set; }

        /// <summary>
        /// 재사용가능수량(매도) [16]
        /// </summary>
        public long ordableruseqty { get; set; }

        /// <summary>
        /// 변동수량 [16]
        /// </summary>
        public long flctqty { get; set; }

        /// <summary>
        /// 잔고수량(D2) [16]
        /// </summary>
        public long secbalqtyd2 { get; set; }

        /// <summary>
        /// 매도주문가능수량 [16]
        /// </summary>
        public long sellableqty { get; set; }

        /// <summary>
        /// 미체결매도주문수량 [16]
        /// </summary>
        public long unercsellordqty { get; set; }

        /// <summary>
        /// 평균매입가 [13]
        /// </summary>
        public long avrpchsprc { get; set; }

        /// <summary>
        /// 매입금액 [16]
        /// </summary>
        public long pchsamt { get; set; }

        /// <summary>
        /// 예수금 [16]
        /// </summary>
        public long deposit { get; set; }

        /// <summary>
        /// 대용금 [16]
        /// </summary>
        public long substamt { get; set; }

        /// <summary>
        /// 위탁증거금현금 [16]
        /// </summary>
        public long csgnmnymgn { get; set; }

        /// <summary>
        /// 위탁증거금대용 [16]
        /// </summary>
        public long csgnsubstmgn { get; set; }

        /// <summary>
        /// 신용담보재사용금 [16]
        /// </summary>
        public long crdtpldgruseamt { get; set; }

        /// <summary>
        /// 주문가능현금 [16]
        /// </summary>
        public long ordablemny { get; set; }

        /// <summary>
        /// 주문가능대용 [16]
        /// </summary>
        public long ordablesubstamt { get; set; }

        /// <summary>
        /// 재사용가능금액 [16]
        /// </summary>
        public long ruseableamt { get; set; }
    }
}
