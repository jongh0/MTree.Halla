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
    public class t8424InBlock : BlockBase
    {
        [PropertyIgnore]
        public override string BlockName => nameof(t8424InBlock);

        /// <summary>
        /// 구분1 [1]
        /// </summary>
        public string gubun1 { get; set; }
    }
}
