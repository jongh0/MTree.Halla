using XA_DATASETLib;

namespace CommonLib.Firm.Ebest.Query
{
    /// <summary>
    /// 주식 현재가(시세) 조회
    /// </summary>
    public class t1102OutBlock : BlockBase
    {
        /// <summary>
        /// 한글명
        /// </summary>
        public string hname { get; set; }

        /// <summary>
        /// 유동주식수
        /// </summary>
        public long abscnt { get; set; }

        /// <summary>
        /// 상장일
        /// </summary>
        public string listdate { get; set; }

        /// <summary>
        /// 락구분
        /// </summary>
        public string info1 { get; set; }

        /// <summary>
        /// 정지/연장구분
        /// </summary>
        public string info3 { get; set; }
    }
}
