using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Firm.Ebest.Block
{
    public class t1511InBlock : BlockBase
    {
        public override string BlockName => nameof(t1511InBlock);

        /// <summary>
        /// 업종코드 [3]
        /// </summary>
        public string upcode { get; set; }
    }
}
