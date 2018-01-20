using CommonLib.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Firm.Ebest.Block
{
    /// <summary>
    /// 투자경고/매매정지/정리매매조회
    /// </summary>
    public class t1405InBlock : BlockBase
    {
        [PropertyIgnore]
        public override string BlockName => nameof(t1405InBlock);

        /// <summary>
        /// 구분 [1]
        /// </summary>
        public string gubun { get; set; }

        /// <summary>
        /// 종목체크 [1]
        /// </summary>
        public string jongchk { get; set; }

        /// <summary>
        /// 종목코드_CTS [6]
        /// </summary>
        public string cts_shcode { get; set; }
    }
}
