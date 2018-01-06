using XA_DATASETLib;

namespace CommonLib.Firm.Ebest.Query
{
    /// <summary>
    /// 업종현재가
    /// </summary>
    public class t1511OutBlock : BlockBase
    {
        /// <summary>
        /// 업종명
        /// </summary>
        public string hname { get; set; }

        /// <summary>
        /// 전일지수
        /// </summary>
        public float jniljisu { get; set; }
    }
}
