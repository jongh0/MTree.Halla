using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Firm.Ebest
{
    /// <summary>
    /// 주식종목조회
    /// </summary>
    public class t8430OutBlock : BlockBase
    {
        /// <summary>
        /// 종목명 [20]
        /// </summary>
        public string hname { get; set; }

        /// <summary>
        /// 단축코드 [6]
        /// </summary>
        public string shcode { get; set; }

        /// <summary>
        /// 확장코드 [12]
        /// </summary>
        public string expcode { get; set; }

        /// <summary>
        /// ETF구분(1:ETF) [1]
        /// </summary>
        public string etfgubun { get; set; }

        /// <summary>
        /// 상한가 [8]
        /// </summary>
        public long uplmtprice { get; set; }

        /// <summary>
        /// 하한가 [8]
        /// </summary>
        public long dnlmtprice { get; set; }

        /// <summary>
        /// 전일가 [8]
        /// </summary>
        public long jnilclose { get; set; }

        /// <summary>
        /// 주문수량단위 [5]
        /// </summary>
        public string memedan { get; set; }

        /// <summary>
        /// 기준가 [8]
        /// </summary>
        public long recprice { get; set; }

        /// <summary>
        /// 구분(1:코스피2:코스닥) [1]
        /// </summary>
        public string gubun { get; set; }
    }
}
