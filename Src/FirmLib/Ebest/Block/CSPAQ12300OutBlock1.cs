using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirmLib.Ebest.Block
{
    /// <summary>
    /// 현물계좌 잔고내역 조회
    /// </summary>
    public class CSPAQ12300OutBlock1 : BlockBase
    {
        /// <summary>
        /// 레코드갯수 [5]
        /// </summary>
        public long RecCnt { get; set; }

        /// <summary>
        /// 계좌번호 [20]
        /// </summary>
        public string AcntNo { get; set; }

        /// <summary>
        /// 비밀번호 [8]
        /// </summary>
        public string Pwd { get; set; }

        /// <summary>
        /// 잔고생성구분 [1]
        /// </summary>
        public string BalCreTp { get; set; }

        /// <summary>
        /// 수수료적용구분 [1]
        /// </summary>
        public string CmsnAppTpCode { get; set; }

        /// <summary>
        /// D2잔고기준조회구분 [1]
        /// </summary>
        public string D2balBaseQryTp { get; set; }

        /// <summary>
        /// 단가구분 [1]
        /// </summary>
        public string UprcTpCode { get; set; }
    }
}
