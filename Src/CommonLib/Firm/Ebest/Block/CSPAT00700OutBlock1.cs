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
    public class CSPAT00700OutBlock1 : BlockBase
    {
        [PropertyIgnore]
        public override string BlockName => nameof(CSPAT00700OutBlock1);

        /// <summary>
        /// 레코드갯수 [5]
        /// </summary>
        public long RecCnt { get; set; }

        /// <summary>
        /// 원주문번호 [10]
        /// </summary>
        public long OrgOrdNo { get; set; }

        /// <summary>
        /// 계좌번호 [20]
        /// </summary>
        public string AcntNo { get; set; }

        /// <summary>
        /// 입력비밀번호 [8]
        /// </summary>
        public string InptPwd { get; set; }

        /// <summary>
        /// 종목번호 [12]
        /// </summary>
        public string IsuNo { get; set; }

        /// <summary>
        /// 주문수량 [16]
        /// </summary>
        public long OrdQty { get; set; }

        /// <summary>
        /// 호가유형코드 [2]
        /// </summary>
        public string OrdprcPtnCode { get; set; }

        /// <summary>
        /// 주문조건구분 [1]
        /// </summary>
        public string OrdCndiTpCode { get; set; }

        /// <summary>
        /// 주문가 [13.2]
        /// </summary>
        public double OrdPrc { get; set; }

        /// <summary>
        /// 통신매체코드 [2]
        /// </summary>
        public string CommdaCode { get; set; }

        /// <summary>
        /// 전략코드 [6]
        /// </summary>
        [PropertyIgnore]
        public string StrtgCode { get; set; }

        /// <summary>
        /// 그룹ID [20]
        /// </summary>
        public string GrpId { get; set; }

        /// <summary>
        /// 주문회차 [10]
        /// </summary>
        public long OrdSeqNo { get; set; }

        /// <summary>
        /// 포트폴리오번호 [10]
        /// </summary>
        public long PtflNo { get; set; }

        /// <summary>
        /// 바스켓번호 [10]
        /// </summary>
        public long BskNo { get; set; }

        /// <summary>
        /// 트렌치번호 [10]
        /// </summary>
        public long TrchNo { get; set; }

        /// <summary>
        /// 아이템번호 [10]
        /// </summary>
        public long ItemNo { get; set; }
    }
}
