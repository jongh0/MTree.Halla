using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Firm.Ebest.Block
{
    public class DVIOutBlock : BlockBase
    {
        /// <summary>
        /// 시간외단일가VI발동해제(DVI)
        /// </summary>
        public override string BlockName => "OutBlock";

        /// <summary>
        /// 구분(0:해제 1:정적발동 2:동적발동 3:정적&동적) [1]
        /// </summary>
        public string vi_gubun { get; set; }

        /// <summary>
        /// 정적VI발동기준가격 [8]
        /// </summary>
        public long svi_recprice { get; set; }

        /// <summary>
        /// 동적VI발동기준가격 [8]
        /// </summary>
        public long dvi_recprice { get; set; }

        /// <summary>
        /// VI발동가격 [8]
        /// </summary>
        public long vi_trgprice { get; set; }

        /// <summary>
        /// 단축코드(KEY) [6]
        /// </summary>
        public string shcode { get; set; }

        /// <summary>
        /// 참조코드(미사용) [6]
        /// </summary>
        public string ref_shcode { get; set; }

        /// <summary>
        /// 시간 [6]
        /// </summary>
        public string time { get; set; }
    }
}
