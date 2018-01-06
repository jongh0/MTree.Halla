using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Firm.Ebest.Query
{
    /// <summary>
    /// 현물 취소주문
    /// </summary>
    public class CSPAT00800InBlock1 : BlockBase
    {
        #region Property
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
        #endregion
    }
}
