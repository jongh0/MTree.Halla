using CommonLib.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirmLib.Ebest.Block
{
    /// <summary>
    /// 테마종목별시세조회
    /// </summary>
    public class t1537OutBlock1 : BlockBase
    {
        /// <summary>
        /// 종목명 [20]
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
        /// 전일동시간 [9.2]
        /// </summary>
        public float jniltime { get; set; }

        /// <summary>
        /// 종목코드 [6]
        /// </summary>
        public string shcode { get; set; }

        /// <summary>
        /// 예상체결가 [8]
        /// </summary>
        public long yeprice { get; set; }

        /// <summary>
        /// 시가 [8]
        /// </summary>
        public long open { get; set; }

        /// <summary>
        /// 고가 [8]
        /// </summary>
        public long high { get; set; }

        /// <summary>
        /// 저가 [8]
        /// </summary>
        public long low { get; set; }

        /// <summary>
        /// 누적거래대금(단위:백만) [12]
        /// </summary>
        public long value { get; set; }

        /// <summary>
        /// 시가총액(단위:백만) [12]
        /// </summary>
        public long marketcap { get; set; }
    }
}
