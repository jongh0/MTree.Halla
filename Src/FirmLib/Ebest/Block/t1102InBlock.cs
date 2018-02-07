using CommonLib.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirmLib.Ebest.Block
{
    /// <summary>
    /// 주식 현재가(시세) 조회
    /// </summary>
    public class t1102InBlock : BlockBase
    {
        /// <summary>
        /// 단축코드 [6]
        /// </summary>
        public string shcode { get; set; }
    }
}
