using CommonLib.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Firm.Ebest.Block
{
    /// <summary>
    /// 테마종목별시세조회
    /// </summary>
    public class t1537OutBlock : BlockBase
    {
        /// <summary>
        /// 테마코드 [4]
        /// </summary>
        public string tmcode { get; set; }

        /// <summary>
        /// 상승종목수 [4]
        /// </summary>
        public long upcnt { get; set; }

        /// <summary>
        /// 테마종목수 [4]
        /// </summary>
        public long tmcnt { get; set; }

        /// <summary>
        /// 상승종목비율 [4]
        /// </summary>
        public long uprate { get; set; }

        /// <summary>
        /// 테마명 [36]
        /// </summary>
        public string tmname { get; set; }
    }
}
