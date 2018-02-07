using CommonLib.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirmLib.Ebest.Block
{
    /// <summary>
    /// 현물 정상주문
    /// </summary>
    public class CSPAT00600InBlock1 : BlockBase
    {
        /// <summary>
        /// 계좌번호 [20]
        /// </summary>
        public string AcntNo { get; set; }

        /// <summary>
        /// 입력비밀번호 [8]
        /// </summary>
        public string InptPwd { get; set; }

        /// <summary>
        /// 종목번호 [12]
        /// </summary>
        public string IsuNo { get; set; }

        /// <summary>
        /// 주문수량 [16]
        /// </summary>
        public long OrdQty { get; set; }

        /// <summary>
        /// 주문가 [13.2]
        /// </summary>
        public double OrdPrc { get; set; }

        /// <summary>
        /// 매매구분 [1]
        /// </summary>
        public string BnsTpCode { get; set; }

        /// <summary>
        /// 호가유형코드 [2]
        /// </summary>
        public string OrdprcPtnCode { get; set; }

        /// <summary>
        /// 신용거래코드 [3]
        /// </summary>
        public string MgntrnCode { get; set; }

        /// <summary>
        /// 대출일 [8]
        /// </summary>
        public string LoanDt { get; set; }

        /// <summary>
        /// 주문조건구분 [1]
        /// </summary>
        public string OrdCndiTpCode { get; set; }
    }
}
