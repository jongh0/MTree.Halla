using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Firm.Ebest.Block
{
    /// <summary>
    /// 시간외단일가VI발동해제(DVI)
    /// </summary>
    public class DVIInBlock : BlockBase
    {
        public override string BlockName => "InBlock";

        /// <summary>
        /// 단축코드(KEY) [6]
        /// </summary>
        public string shcode { get; set; }
    }
}
