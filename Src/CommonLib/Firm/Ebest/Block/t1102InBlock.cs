using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Firm.Ebest.Block
{
    /// <summary>
    /// 주식 현재가(시세) 조회
    /// </summary>
    public class t1102InBlock : BlockBase
    {
        public override string BlockName => nameof(t1102InBlock);

        /// <summary>
        /// 단축코드 [6]
        /// </summary>
        public string shcode { get; set; }
    }
}
