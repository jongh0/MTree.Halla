using CommonLib.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Firm.Ebest.Block
{
    /// <summary>
    /// 주식잔고2
    /// </summary>
    public class t0424OutBlock1 : BlockBase
    {
        /// <summary>
        /// 종목번호 [12]
        /// </summary>
        public string expcode { get; set; }

        /// <summary>
        /// 잔고구분 [10]
        /// </summary>
        public string jangb { get; set; }

        /// <summary>
        /// 잔고수량 [18]
        /// </summary>
        public long janqty { get; set; }

        /// <summary>
        /// 매도가능수량 [18]
        /// </summary>
        public long mdposqt { get; set; }

        /// <summary>
        /// 평균단가 [18]
        /// </summary>
        public long pamt { get; set; }

        /// <summary>
        /// 매입금액 [18]
        /// </summary>
        public long mamt { get; set; }

        /// <summary>
        /// 대출금액 [18]
        /// </summary>
        public long sinamt { get; set; }

        /// <summary>
        /// 만기일자 [8]
        /// </summary>
        public string lastdt { get; set; }

        /// <summary>
        /// 당일매수금액 [18]
        /// </summary>
        public long msat { get; set; }

        /// <summary>
        /// 당일매수단가 [18]
        /// </summary>
        public long mpms { get; set; }

        /// <summary>
        /// 당일매도금액 [18]
        /// </summary>
        public long mdat { get; set; }

        /// <summary>
        /// 당일매도단가 [18]
        /// </summary>
        public long mpmd { get; set; }

        /// <summary>
        /// 전일매수금액 [18]
        /// </summary>
        public long jsat { get; set; }

        /// <summary>
        /// 전일매수단가 [18]
        /// </summary>
        public long jpms { get; set; }

        /// <summary>
        /// 전일매도금액 [18]
        /// </summary>
        public long jdat { get; set; }

        /// <summary>
        /// 전일매도단가 [18]
        /// </summary>
        public long jpmd { get; set; }

        /// <summary>
        /// 처리순번 [10]
        /// </summary>
        public long sysprocseq { get; set; }

        /// <summary>
        /// 대출일자 [8]
        /// </summary>
        public string loandt { get; set; }

        /// <summary>
        /// 종목명 [20]
        /// </summary>
        public string hname { get; set; }

        /// <summary>
        /// 시장구분 [1]
        /// </summary>
        public string marketgb { get; set; }

        /// <summary>
        /// 종목구분 [1]
        /// </summary>
        public string jonggb { get; set; }

        /// <summary>
        /// 보유비중 [10.2]
        /// </summary>
        public float janrt { get; set; }

        /// <summary>
        /// 현재가 [8]
        /// </summary>
        public long price { get; set; }

        /// <summary>
        /// 평가금액 [18]
        /// </summary>
        public long appamt { get; set; }

        /// <summary>
        /// 평가손익 [18]
        /// </summary>
        public long dtsunik { get; set; }

        /// <summary>
        /// 수익율 [10.2]
        /// </summary>
        public float sunikrt { get; set; }

        /// <summary>
        /// 수수료 [10]
        /// </summary>
        public long fee { get; set; }

        /// <summary>
        /// 제세금 [10]
        /// </summary>
        public long tax { get; set; }

        /// <summary>
        /// 신용이자 [10]
        /// </summary>
        public long sininter { get; set; }
    }
}
