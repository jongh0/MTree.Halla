using CommonLib.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Firm.Ebest.Block
{
    /// <summary>
    /// 현물 정정주문
    /// </summary>
    public class CSPAT00700OutBlock2 : BlockBase
    {
        [PropertyIgnore]
        public override string BlockName => nameof(CSPAT00700OutBlock2);

        /// <summary>
        /// 레코드갯수 [5]
        /// </summary>
        public long RecCnt { get; set; }

        /// <summary>
        /// 주문번호 [10]
        /// </summary>
        public long OrdNo { get; set; }

        /// <summary>
        /// 모주문번호 [10]
        /// </summary>
        public long PrntOrdNo { get; set; }

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
        /// 프로그램호가유형코드 [2]
        /// </summary>
        public string PrgmOrdprcPtnCode { get; set; }

        /// <summary>
        /// 공매도호가구분 [1]
        /// </summary>
        public string StslOrdprcTpCode { get; set; }

        /// <summary>
        /// 공매도가능여부 [1]
        /// </summary>
        public string StslAbleYn { get; set; }

        /// <summary>
        /// 신용거래코드 [3]
        /// </summary>
        public string MgntrnCode { get; set; }

        /// <summary>
        /// 대출일 [8]
        /// </summary>
        public string LoanDt { get; set; }

        /// <summary>
        /// 반대매매주문구분 [1]
        /// </summary>
        public string CvrgOrdTp { get; set; }

        /// <summary>
        /// 유동성공급자여부 [1]
        /// </summary>
        public string LpYn { get; set; }

        /// <summary>
        /// 관리사원번호 [9]
        /// </summary>
        public string MgempNo { get; set; }

        /// <summary>
        /// 주문금액 [16]
        /// </summary>
        public long OrdAmt { get; set; }

        /// <summary>
        /// 매매구분 [1]
        /// </summary>
        public string BnsTpCode { get; set; }

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
