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
    public class t1537InBlock : BlockBase
    {
        [PropertyIgnore]
        public override string BlockName => nameof(t1537InBlock);

        /// <summary>
        /// 테마코드 [4]
        /// </summary>
        public string tmcode { get; set; }
    }
}
