using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Firm.Ebest.Block
{
    public class SC2OutBlock : BlockBase
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
        /// 주문체결유형코드 [2]
        /// </summary>
        public string ordxctptncode { get; set; }

        /// <summary>
        /// 주문시장코드 [2]
        /// </summary>
        public string ordmktcode { get; set; }

        /// <summary>
        /// 주문유형코드 [2]
        /// </summary>
        public string ordptncode { get; set; }

        /// <summary>
        /// 관리지점번호 [3]
        /// </summary>
        public string mgmtbrnno { get; set; }

        /// <summary>
        /// 계좌번호 [11]
        /// </summary>
        public string accno1 { get; set; }

        /// <summary>
        /// 계좌번호 [9]
        /// </summary>
        public string accno2 { get; set; }

        /// <summary>
        /// 계좌명 [40]
        /// </summary>
        public string acntnm { get; set; }

        /// <summary>
        /// 종목번호 [12]
        /// </summary>
        public string Isuno { get; set; }

        /// <summary>
        /// 종목명 [40]
        /// </summary>
        public string Isunm { get; set; }

        /// <summary>
        /// 주문번호 [10]
        /// </summary>
        public long ordno { get; set; }

        /// <summary>
        /// 원주문번호 [10]
        /// </summary>
        public long orgordno { get; set; }

        /// <summary>
        /// 체결번호 [10]
        /// </summary>
        public long execno { get; set; }

        /// <summary>
        /// 주문수량 [16]
        /// </summary>
        public long ordqty { get; set; }

        /// <summary>
        /// 주문가격 [13]
        /// </summary>
        public long ordprc { get; set; }

        /// <summary>
        /// 체결수량 [16]
        /// </summary>
        public long execqty { get; set; }

        /// <summary>
        /// 체결가격 [13]
        /// </summary>
        public long execprc { get; set; }

        /// <summary>
        /// 정정확인수량 [16]
        /// </summary>
        public long mdfycnfqty { get; set; }

        /// <summary>
        /// 정정확인가격 [16]
        /// </summary>
        public long mdfycnfprc { get; set; }

        /// <summary>
        /// 취소확인수량 [16]
        /// </summary>
        public long canccnfqty { get; set; }

        /// <summary>
        /// 거부수량 [16]
        /// </summary>
        public long rjtqty { get; set; }

        /// <summary>
        /// 주문처리유형코드 [4]
        /// </summary>
        public long ordtrxptncode { get; set; }

        /// <summary>
        /// 복수주문일련번호 [10]
        /// </summary>
        public long mtiordseqno { get; set; }

        /// <summary>
        /// 주문조건 [1]
        /// </summary>
        public string ordcndi { get; set; }

        /// <summary>
        /// 호가유형코드 [2]
        /// </summary>
        public string ordprcptncode { get; set; }

        /// <summary>
        /// 비저축체결수량 [16]
        /// </summary>
        public long nsavtrdqty { get; set; }

        /// <summary>
        /// 단축종목번호 [9]
        /// </summary>
        public string shtnIsuno { get; set; }

        /// <summary>
        /// 운용지시번호 [12]
        /// </summary>
        public string opdrtnno { get; set; }

        /// <summary>
        /// 반대매매주문구분 [1]
        /// </summary>
        public string cvrgordtp { get; set; }

        /// <summary>
        /// 미체결수량(주문) [16]
        /// </summary>
        public long unercqty { get; set; }

        /// <summary>
        /// 원주문미체결수량 [16]
        /// </summary>
        public long orgordunercqty { get; set; }

        /// <summary>
        /// 원주문정정수량 [16]
        /// </summary>
        public long orgordmdfyqty { get; set; }

        /// <summary>
        /// 원주문취소수량 [16]
        /// </summary>
        public long orgordcancqty { get; set; }

        /// <summary>
        /// 주문평균체결가격 [13]
        /// </summary>
        public long ordavrexecprc { get; set; }

        /// <summary>
        /// 주문금액 [16]
        /// </summary>
        public long ordamt { get; set; }

        /// <summary>
        /// 표준종목번호 [12]
        /// </summary>
        public string stdIsuno { get; set; }

        /// <summary>
        /// 전표준종목번호 [12]
        /// </summary>
        public string bfstdIsuno { get; set; }

        /// <summary>
        /// 매매구분 [1]
        /// </summary>
        public string bnstp { get; set; }

        /// <summary>
        /// 주문거래유형코드 [2]
        /// </summary>
        public string ordtrdptncode { get; set; }

        /// <summary>
        /// 신용거래코드 [3]
        /// </summary>
        public string mgntrncode { get; set; }

        /// <summary>
        /// 수수료합산코드 [2]
        /// </summary>
        public string adduptp { get; set; }

        /// <summary>
        /// 통신매체코드 [2]
        /// </summary>
        public string commdacode { get; set; }

        /// <summary>
        /// 대출일 [8]
        /// </summary>
        public string Loandt { get; set; }

        /// <summary>
        /// 회원/비회원사번호 [3]
        /// </summary>
        public long mbrnmbrno { get; set; }

        /// <summary>
        /// 주문계좌번호 [20]
        /// </summary>
        public string ordacntno { get; set; }

        /// <summary>
        /// 집계지점번호 [3]
        /// </summary>
        public string agrgbrnno { get; set; }

        /// <summary>
        /// 관리사원번호 [9]
        /// </summary>
        public string mgempno { get; set; }

        /// <summary>
        /// 선물연계지점번호 [3]
        /// </summary>
        public string futsLnkbrnno { get; set; }

        /// <summary>
        /// 선물연계계좌번호 [20]
        /// </summary>
        public string futsLnkacntno { get; set; }

        /// <summary>
        /// 선물시장구분 [1]
        /// </summary>
        public string futsmkttp { get; set; }

        /// <summary>
        /// 등록시장코드 [2]
        /// </summary>
        public string regmktcode { get; set; }

        /// <summary>
        /// 현금증거금률 [7]
        /// </summary>
        public long mnymgnrat { get; set; }

        /// <summary>
        /// 대용증거금률 [9]
        /// </summary>
        public long substmgnrat { get; set; }

        /// <summary>
        /// 현금체결금액 [16]
        /// </summary>
        public long mnyexecamt { get; set; }

        /// <summary>
        /// 대용체결금액 [16]
        /// </summary>
        public long ubstexecamt { get; set; }

        /// <summary>
        /// 수수료체결금액 [16]
        /// </summary>
        public long cmsnamtexecamt { get; set; }

        /// <summary>
        /// 신용담보체결금액 [16]
        /// </summary>
        public long crdtpldgexecamt { get; set; }

        /// <summary>
        /// 신용체결금액 [16]
        /// </summary>
        public long crdtexecamt { get; set; }

        /// <summary>
        /// 전일재사용체결금액 [16]
        /// </summary>
        public long prdayruseexecval { get; set; }

        /// <summary>
        /// 금일재사용체결금액 [16]
        /// </summary>
        public long crdayruseexecval { get; set; }

        /// <summary>
        /// 실물체결수량 [16]
        /// </summary>
        public long spotexecqty { get; set; }

        /// <summary>
        /// 공매도체결수량 [16]
        /// </summary>
        public long stslexecqty { get; set; }

        /// <summary>
        /// 전략코드 [6]
        /// </summary>
        public string strtgcode { get; set; }

        /// <summary>
        /// 그룹Id [20]
        /// </summary>
        public string grpId { get; set; }

        /// <summary>
        /// 주문회차 [10]
        /// </summary>
        public long ordseqno { get; set; }

        /// <summary>
        /// 포트폴리오번호 [10]
        /// </summary>
        public long ptflno { get; set; }

        /// <summary>
        /// 바스켓번호 [10]
        /// </summary>
        public long bskno { get; set; }

        /// <summary>
        /// 트렌치번호 [10]
        /// </summary>
        public long trchno { get; set; }

        /// <summary>
        /// 아이템번호 [10]
        /// </summary>
        public long itemno { get; set; }

        /// <summary>
        /// 주문자Id [16]
        /// </summary>
        public string orduserId { get; set; }

        /// <summary>
        /// 차입관리여부 [1]
        /// </summary>
        public long brwmgmtYn { get; set; }

        /// <summary>
        /// 외국인고유번호 [6]
        /// </summary>
        public string frgrunqno { get; set; }

        /// <summary>
        /// 거래세징수구분 [1]
        /// </summary>
        public string trtzxLevytp { get; set; }

        /// <summary>
        /// 유동성공급자구분 [1]
        /// </summary>
        public string lptp { get; set; }

        /// <summary>
        /// 체결시각 [9]
        /// </summary>
        public string exectime { get; set; }

        /// <summary>
        /// 거래소수신체결시각 [9]
        /// </summary>
        public string rcptexectime { get; set; }

        /// <summary>
        /// 잔여대출금액 [16]
        /// </summary>
        public long rmndLoanamt { get; set; }

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
        /// 잔고수량(d2) [16]
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
        public long pchsant { get; set; }

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
