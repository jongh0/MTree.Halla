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
    public class CSPAQ12300OutBlock2 : BlockBase
    {
        /// <summary>
        /// 레코드갯수 [5]
        /// </summary>
        public long RecCnt { get; set; }

        /// <summary>
        /// 계좌명 [40]
        /// </summary>
        public string AcntNm { get; set; }

        /// <summary>
        /// 현금주문가능금액 [16]
        /// </summary>
        public long MnyOrdAbleAmt { get; set; }

        /// <summary>
        /// 출금가능금액 [16]
        /// </summary>
        public long MnyoutAbleAmt { get; set; }

        /// <summary>
        /// 거래소금액 [16]
        /// </summary>
        public long SeOrdAbleAmt { get; set; }

        /// <summary>
        /// 코스닥금액 [16]
        /// </summary>
        public long KdqOrdAbleAmt { get; set; }

        /// <summary>
        /// HTS주문가능금액 [16]
        /// </summary>
        public long HtsOrdAbleAmt { get; set; }

        /// <summary>
        /// 증거금률100퍼센트주문가능금액 [16]
        /// </summary>
        public long MgnRat100pctOrdAbleAmt { get; set; }

        /// <summary>
        /// 잔고평가금액 [16]
        /// </summary>
        public long BalEvalAmt { get; set; }

        /// <summary>
        /// 매입금액 [16]
        /// </summary>
        public long PchsAmt { get; set; }

        /// <summary>
        /// 미수금액 [16]
        /// </summary>
        public long RcvblAmt { get; set; }

        /// <summary>
        /// 손익율 [18.6]
        /// </summary>
        public double PnlRat { get; set; }

        /// <summary>
        /// 투자원금 [20]
        /// </summary>
        public long InvstOrgAmt { get; set; }

        /// <summary>
        /// 투자손익금액 [16]
        /// </summary>
        public long InvstPlAmt { get; set; }

        /// <summary>
        /// 신용담보주문금액 [16]
        /// </summary>
        public long CrdtPldgOrdAmt { get; set; }

        /// <summary>
        /// 예수금 [16]
        /// </summary>
        public long Dps { get; set; }

        /// <summary>
        /// D1예수금 [16]
        /// </summary>
        public long D1Dps { get; set; }

        /// <summary>
        /// D2예수금 [16]
        /// </summary>
        public long D2Dps { get; set; }

        /// <summary>
        /// 주문일 [8]
        /// </summary>
        public string OrdDt { get; set; }

        /// <summary>
        /// 현금증거금액 [16]
        /// </summary>
        public long MnyMgn { get; set; }

        /// <summary>
        /// 대용증거금액 [16]
        /// </summary>
        public long SubstMgn { get; set; }

        /// <summary>
        /// 대용금액 [16]
        /// </summary>
        public long SubstAmt { get; set; }

        /// <summary>
        /// 전일매수체결금액 [16]
        /// </summary>
        public long PrdayBuyExecAmt { get; set; }

        /// <summary>
        /// 전일매도체결금액 [16]
        /// </summary>
        public long PrdaySellExecAmt { get; set; }

        /// <summary>
        /// 금일매수체결금액 [16]
        /// </summary>
        public long CrdayBuyExecAmt { get; set; }

        /// <summary>
        /// 금일매도체결금액 [16]
        /// </summary>
        public long CrdaySellExecAmt { get; set; }

        /// <summary>
        /// 평가손익합계 [15]
        /// </summary>
        public long EvalPnlSum { get; set; }

        /// <summary>
        /// 예탁자산총액 [16]
        /// </summary>
        public long DpsastTotamt { get; set; }

        /// <summary>
        /// 대출금액 [16]
        /// </summary>
        public long LoanAmt { get; set; }
    }
}
