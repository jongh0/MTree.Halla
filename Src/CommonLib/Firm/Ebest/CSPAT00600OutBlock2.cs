using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Firm.Ebest
{
    /// <summary>
    /// 현물 정상주문
    /// </summary>
    public class CSPAT00600OutBlock2 : BlockBase
    {
        /// <summary>
        /// 레코드갯수 [5]
        /// </summary>
        public long RecCnt { get; set; }

        /// <summary>
        /// 주문번호 [10]
        /// </summary>
        public long OrdNo { get; set; }

        /// <summary>
        /// 주문시각 [9]
        /// </summary>
        public string OrdTime { get; set; }

        /// <summary>
        /// 주문시장코드 [2]
        /// </summary>
        public string OrdMktCode { get; set; }

        /// <summary>
        /// 주문유형코드 [2]
        /// </summary>
        public string OrdPtnCode { get; set; }

        /// <summary>
        /// 단축종목번호 [9]
        /// </summary>
        public string ShtnIsuNo { get; set; }

        /// <summary>
        /// 관리사원번호 [9]
        /// </summary>
        public string MgempNo { get; set; }

        /// <summary>
        /// 주문금액 [16]
        /// </summary>
        public long OrdAmt { get; set; }

        /// <summary>
        /// 예비주문번호 [10]
        /// </summary>
        public long SpareOrdNo { get; set; }

        /// <summary>
        /// 반대매매일련번호 [10]
        /// </summary>
        public long CvrgSeqno { get; set; }

        /// <summary>
        /// 예약주문번호 [10]
        /// </summary>
        public long RsvOrdNo { get; set; }

        /// <summary>
        /// 실물주문수량 [16]
        /// </summary>
        public long SpotOrdQty { get; set; }

        /// <summary>
        /// 재사용주문수량 [16]
        /// </summary>
        public long RuseOrdQty { get; set; }

        /// <summary>
        /// 현금주문금액 [16]
        /// </summary>
        public long MnyOrdAmt { get; set; }

        /// <summary>
        /// 대용주문금액 [16]
        /// </summary>
        public long SubstOrdAmt { get; set; }

        /// <summary>
        /// 재사용주문금액 [16]
        /// </summary>
        public long RuseOrdAmt { get; set; }

        /// <summary>
        /// 계좌명 [40]
        /// </summary>
        public string AcntNm { get; set; }

        /// <summary>
        /// 종목명 [40]
        /// </summary>
        public string IsuNm { get; set; }
    }
}
