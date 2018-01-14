using CommonLib.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Firm.Ebest.Block
{
    /// <summary>
    /// 주식잔고2
    /// </summary>
    public class t0424OutBlock : BlockBase
    {
        [PropertyIgnore]
        public override string BlockName => nameof(t0424OutBlock);

        /// <summary>
        /// 추정순자산 [18]
        /// </summary>
        public long sunamt { get; set; }

        /// <summary>
        /// 실현손익 [18]
        /// </summary>
        public long dtsunik { get; set; }

        /// <summary>
        /// 매입금액 [18]
        /// </summary>
        public long mamt { get; set; }

        /// <summary>
        /// 추정D2예수금 [18]
        /// </summary>
        public long sunamt1 { get; set; }

        /// <summary>
        /// CTS_종목번호 [22]
        /// </summary>
        public string cts_expcode { get; set; }

        /// <summary>
        /// 평가금액 [18]
        /// </summary>
        public long tappamt { get; set; }

        /// <summary>
        /// 평가손익 [18]
        /// </summary>
        public long tdtsunik { get; set; }
    }
}
