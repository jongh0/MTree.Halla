using CommonLib.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Firm.Ebest.Block
{
    /// <summary>
    /// 주식체결/미체결
    /// </summary>
    public class t0425OutBlock : BlockBase
    {
        [PropertyIgnore]
        public override string BlockName => nameof(t0425OutBlock);

        /// <summary>
        /// 총주문수량 [18]
        /// </summary>
        public long tqty { get; set; }

        /// <summary>
        /// 총체결수량 [18]
        /// </summary>
        public long tcheqty { get; set; }

        /// <summary>
        /// 총미체결수량 [18]
        /// </summary>
        public long tordrem { get; set; }

        /// <summary>
        /// 추정수수료 [18]
        /// </summary>
        public long cmss { get; set; }

        /// <summary>
        /// 총주문금액 [18]
        /// </summary>
        public long tamt { get; set; }

        /// <summary>
        /// 총매도체결금액 [18]
        /// </summary>
        public long tmdamt { get; set; }

        /// <summary>
        /// 총매수체결금액 [18]
        /// </summary>
        public long tmsamt { get; set; }

        /// <summary>
        /// 추정제세금 [18]
        /// </summary>
        public long tax { get; set; }

        /// <summary>
        /// 주문번호 [10]
        /// </summary>
        public string cts_ordno { get; set; }
    }
}
