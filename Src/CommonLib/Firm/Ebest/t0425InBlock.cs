using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Firm.Ebest
{
    /// <summary>
    /// 주식체결/미체결
    /// </summary>
    public class t0425InBlock : BlockBase
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
        /// 종목번호 [12]
        /// </summary>
        public string expcode { get; set; }

        /// <summary>
        /// 체결구분 [1]
        /// </summary>
        public string chegb { get; set; }

        /// <summary>
        /// 매매구분 [1]
        /// </summary>
        public string medosu { get; set; }

        /// <summary>
        /// 정렬순서 [1]
        /// </summary>
        public string sortgb { get; set; }

        /// <summary>
        /// 주문번호 [10]
        /// </summary>
        public string cts_ordno { get; set; }
    }
}
