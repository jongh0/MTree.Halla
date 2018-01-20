using CommonLib.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Firm.Ebest.Block
{
    /// <summary>
    /// 전체업종
    /// </summary>
    public class t8424OutBlock : BlockBase
    {
        /// <summary>
        /// 업종명 [20]
        /// </summary>
        public string hname { get; set; }

        /// <summary>
        /// 업종코드 [3]
        /// </summary>
        public string upcode { get; set; }
    }
}
