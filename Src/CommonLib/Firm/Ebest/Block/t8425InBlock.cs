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
    public class t8425InBlock : BlockBase
    {
        [PropertyIgnore]
        public override string BlockName => nameof(t8425InBlock);

        /// <summary>
        /// Dummy [1]
        /// </summary>
        public string dummy { get; set; }
    }
}
