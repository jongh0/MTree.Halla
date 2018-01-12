using XA_DATASETLib;

namespace CommonLib.Firm.Ebest
{
    /// <summary>
    /// 현물 정상주문
    /// </summary>
    public class CSPAT00600OutBlock1 : BlockBase
    {
        /// <summary>
        /// 레코드갯수
        /// </summary>
        public long RecCnt { get; set; }

        /// <summary>
        /// 계좌번호
        /// </summary>
        public string AcntNo { get; set; }

        /// <summary>
        /// 입력비밀번호
        /// </summary>
        public string InptPwd { get; set; }

        /// <summary>
        /// 종목번호
        /// </summary>
        public string IsuNo { get; set; }

        /// <summary>
        /// 주문수량
        /// </summary>
        public long OrdQty { get; set; }

        /// <summary>
        /// 주문가
        /// </summary>
        public double OrdPrc { get; set; }

        /// <summary>
        /// 매매구분
        /// </summary>
        public string BnsTpCode { get; set; }

        /// <summary>
        /// 호가유형코드
        /// </summary>
        public string OrdprcPtnCode { get; set; }

        /// <summary>
        /// 프로그램호가유형코드
        /// </summary>
        public string PrgmOrdprcPtnCode { get; set; }

        /// <summary>
        /// 공매도가능여부
        /// </summary>
        public string StslAbleYn { get; set; }

        /// <summary>
        /// 공매도호가구분
        /// </summary>
        public string StslOrdprcTpCode { get; set; }

        /// <summary>
        /// 통신매체코드
        /// </summary>
        public string CommdaCode { get; set; }

        /// <summary>
        /// 신용거래코드
        /// </summary>
        public string MgntrnCode { get; set; }

        /// <summary>
        /// 대출일
        /// </summary>
        public string LoanDt { get; set; }

        /// <summary>
        /// 회원번호
        /// </summary>
        public string MbrNo { get; set; }

        /// <summary>
        /// 주문조건구분
        /// </summary>
        public string OrdCndiTpCode { get; set; }

        /// <summary>
        /// 전략코드
        /// </summary>
        public string StrtgCode { get; set; }

        /// <summary>
        /// 그룹ID
        /// </summary>
        public string GrpId { get; set; }

        /// <summary>
        /// 주문회차
        /// </summary>
        public long OrdSeqNo { get; set; }

        /// <summary>
        /// 포트폴리오번호
        /// </summary>
        public long PtflNo { get; set; }

        /// <summary>
        /// 바스켓번호
        /// </summary>
        public long BskNo { get; set; }

        /// <summary>
        /// 트렌치번호
        /// </summary>
        public long TrchNo { get; set; }

        /// <summary>
        /// 아이템번호
        /// </summary>
        public long ItemNo { get; set; }

        /// <summary>
        /// 운용지시번호
        /// </summary>
        public string OpDrtnNo { get; set; }

        /// <summary>
        /// 유동성공급자여부
        /// </summary>
        public string LpYn { get; set; }

        /// <summary>
        /// 반대매매구분
        /// </summary>
        public string CvrgTpCode { get; set; }
    }
}
