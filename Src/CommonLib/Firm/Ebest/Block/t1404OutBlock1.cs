using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Firm.Ebest.Block
{
    /// <summary>
    /// 관리/불성실/투자유의조회
    /// </summary>
    public class t1404OutBlock1 : BlockBase
    {
        public override string BlockName => nameof(t1404OutBlock1);

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
        /// 지정일주가 [8]
        /// </summary>
        public long tprice { get; set; }

        /// <summary>
        /// 지정일대비 [8]
        /// </summary>
        public long tchange { get; set; }

        /// <summary>
        /// 대비율 [6.2]
        /// </summary>
        public float tdiff { get; set; }

        /// <summary>
        /// 사유 [4]
        /// </summary>
        public string reason { get; set; }

        /// <summary>
        /// 종목코드 [6]
        /// </summary>
        public string shcode { get; set; }

        /// <summary>
        /// 해제일 [8]
        /// </summary>
        public string edate { get; set; }
    }
}
