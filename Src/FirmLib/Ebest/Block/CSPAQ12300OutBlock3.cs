using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirmLib.Ebest.Block
{
    /// <summary>
    /// 현물계좌 잔고내역 조회
    /// </summary>
    public class CSPAQ12300OutBlock3 : BlockBase
    {
        /// <summary>
        /// 종목번호 [12]
        /// </summary>
        public string IsuNo { get; set; }

        /// <summary>
        /// 종목명 [40]
        /// </summary>
        public string IsuNm { get; set; }

        /// <summary>
        /// 잔고수량 [16]
        /// </summary>
        public long BalQty { get; set; }

        /// <summary>
        /// 매매기준잔고수량 [16]
        /// </summary>
        public long BnsBaseBalQty { get; set; }

        /// <summary>
        /// 금일매수체결수량 [16]
        /// </summary>
        public long CrdayBuyExecQty { get; set; }

        /// <summary>
        /// 금일매도체결수량 [16]
        /// </summary>
        public long CrdaySellExecQty { get; set; }

        /// <summary>
        /// 매도가 [21.4]
        /// </summary>
        public double SellPrc { get; set; }

        /// <summary>
        /// 매수가 [21.4]
        /// </summary>
        public double BuyPrc { get; set; }

        /// <summary>
        /// 매도손익금액 [16]
        /// </summary>
        public long SellPnlAmt { get; set; }

        /// <summary>
        /// 손익율 [18.6]
        /// </summary>
        public double PnlRat { get; set; }

        /// <summary>
        /// 현재가 [15.2]
        /// </summary>
        public double NowPrc { get; set; }

        /// <summary>
        /// 신용금액 [16]
        /// </summary>
        public long CrdtAmt { get; set; }

        /// <summary>
        /// 만기일 [8]
        /// </summary>
        public string DueDt { get; set; }

        /// <summary>
        /// 전일매도체결가 [13.2]
        /// </summary>
        public double PrdaySellExecPrc { get; set; }

        /// <summary>
        /// 전일매도수량 [16]
        /// </summary>
        public long PrdaySellQty { get; set; }

        /// <summary>
        /// 전일매수체결가 [13.2]
        /// </summary>
        public double PrdayBuyExecPrc { get; set; }

        /// <summary>
        /// 전일매수수량 [16]
        /// </summary>
        public long PrdayBuyQty { get; set; }

        /// <summary>
        /// 대출일 [8]
        /// </summary>
        public string LoanDt { get; set; }

        /// <summary>
        /// 평균단가 [13.2]
        /// </summary>
        public double AvrUprc { get; set; }

        /// <summary>
        /// 매도가능수량 [16]
        /// </summary>
        public long SellAbleQty { get; set; }

        /// <summary>
        /// 매도주문수량 [16]
        /// </summary>
        public long SellOrdQty { get; set; }

        /// <summary>
        /// 금일매수체결금액 [16]
        /// </summary>
        public long CrdayBuyExecAmt { get; set; }

        /// <summary>
        /// 금일매도체결금액 [16]
        /// </summary>
        public long CrdaySellExecAmt { get; set; }

        /// <summary>
        /// 전일매수체결금액 [16]
        /// </summary>
        public long PrdayBuyExecAmt { get; set; }

        /// <summary>
        /// 전일매도체결금액 [16]
        /// </summary>
        public long PrdaySellExecAmt { get; set; }

        /// <summary>
        /// 잔고평가금액 [16]
        /// </summary>
        public long BalEvalAmt { get; set; }

        /// <summary>
        /// 평가손익 [16]
        /// </summary>
        public long EvalPnl { get; set; }

        /// <summary>
        /// 현금주문가능금액 [16]
        /// </summary>
        public long MnyOrdAbleAmt { get; set; }

        /// <summary>
        /// 주문가능금액 [16]
        /// </summary>
        public long OrdAbleAmt { get; set; }

        /// <summary>
        /// 매도미체결수량 [16]
        /// </summary>
        public long SellUnercQty { get; set; }

        /// <summary>
        /// 매도미결제수량 [16]
        /// </summary>
        public long SellUnsttQty { get; set; }

        /// <summary>
        /// 매수미체결수량 [16]
        /// </summary>
        public long BuyUnercQty { get; set; }

        /// <summary>
        /// 매수미결제수량 [16]
        /// </summary>
        public long BuyUnsttQty { get; set; }

        /// <summary>
        /// 미결제수량 [16]
        /// </summary>
        public long UnsttQty { get; set; }

        /// <summary>
        /// 미체결수량 [16]
        /// </summary>
        public long UnercQty { get; set; }

        /// <summary>
        /// 전일종가 [15.2]
        /// </summary>
        public double PrdayCprc { get; set; }

        /// <summary>
        /// 매입금액 [16]
        /// </summary>
        public long PchsAmt { get; set; }

        /// <summary>
        /// 등록시장코드 [2]
        /// </summary>
        public string RegMktCode { get; set; }

        /// <summary>
        /// 대출상세분류코드 [2]
        /// </summary>
        public string LoanDtlClssCode { get; set; }

        /// <summary>
        /// 예탁담보대출수량 [16]
        /// </summary>
        public long DpspdgLoanQty { get; set; }
    }
}
