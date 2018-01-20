using CommonLib.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Firm.Ebest.Block
{
    /// <summary>
    /// 전체테마
    /// </summary>
    public class t8425OutBlock : BlockBase
    {
        [PropertyIgnore]
        public override string BlockName => nameof(t8425OutBlock);

        /// <summary>
        /// 테마명 [36]
        /// </summary>
        public string tmname { get; set; }

        /// <summary>
        /// 테마코드 [4]
        /// </summary>
        public string tmcode { get; set; }
    }
}
