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
    public class t1405OutBlock1 : BlockBase
    {
        /// <summary>
        /// 한글명 [20]
        /// </summary>
        public string hname { get; set; }

        /// <summary>
        /// 현재가 [8]
        /// </summary>
        public long price { get; set; }

        /// <summary>
        /// 전일대비구분 [1]
        /// </summary>
        public string sign { get; set; }

        /// <summary>
        /// 전일대비 [8]
        /// </summary>
        public long change { get; set; }

        /// <summary>
        /// 등락율 [6.2]
        /// </summary>
        public float diff { get; set; }

        /// <summary>
        /// 누적거래량 [12]
        /// </summary>
        public long volume { get; set; }

        /// <summary>
        /// 지정일 [8]
        /// </summary>
        public string date { get; set; }

        /// <summary>
        /// 해제일 [8]
        /// </summary>
        public string edate { get; set; }

        /// <summary>
        /// 종목코드 [6]
        /// </summary>
        public string shcode { get; set; }
    }
}
