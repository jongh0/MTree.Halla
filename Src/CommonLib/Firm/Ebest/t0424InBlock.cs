using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Firm.Ebest
{
    /// <summary>
    /// 주식잔고2
    /// </summary>
    public class t0424InBlock : BlockBase
    {
        /// <summary>
        /// 계좌번호 [11]
        /// </summary>
        public string accno { get; set; }

        /// <summary>
        /// 비밀번호 [8]
        /// </summary>
        public string passwd { get; set; }

        /// <summary>
        /// 단가구분 [1]
        /// </summary>
        public string prcgb { get; set; }

        /// <summary>
        /// 체결구분 [1]
        /// </summary>
        public string chegb { get; set; }

        /// <summary>
        /// 단일가구분 [1]
        /// </summary>
        public string dangb { get; set; }

        /// <summary>
        /// 제비용포함여부 [1]
        /// </summary>
        public string charge { get; set; }

        /// <summary>
        /// CTS_종목번호 [22]
        /// </summary>
        public string cts_expcode { get; set; }
    }
}
