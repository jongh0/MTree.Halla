using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Firm.Ebest
{
    /// <summary>
    /// 현물 정정주문
    /// </summary>
    public class CSPAT00700InBlock1 : BlockBase
    {
        /// <summary>
        /// 원주문번호
        /// </summary>
        public string OrgOrdNo { get; set; }

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
        /// 호가유형코드
        /// </summary>
        public string OrdprcPtnCode { get; set; }

        /// <summary>
        /// 주문조건구분
        /// </summary>
        public string OrdCndiTpCode { get; set; }
    }
}
