using CommonLib.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Firm.Ebest.Block
{
    /// <summary>
    /// 현물 취소주문
    /// </summary>
    public class CSPAT00800InBlock1 : BlockBase
    {
        [PropertyIgnore]
        public override string BlockName => nameof(CSPAT00800InBlock1);

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
    }
}
