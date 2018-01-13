using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Firm.Ebest.Block
{
    /// <summary>
    /// 관리/불성실/투자유의조회
    /// </summary>
    public class t1404OutBlock : BlockBase
    {
        public override string BlockName => nameof(t1404OutBlock);

        /// <summary>
        /// 종목코드_CTS [6]
        /// </summary>
        public string cts_shcode { get; set; }
    }
}
