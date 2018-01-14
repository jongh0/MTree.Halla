using CommonLib.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Firm.Ebest.Block
{
    /// <summary>
    /// 주식체결/미체결
    /// </summary>
    public class t0425OutBlock1 : BlockBase
    {
        [PropertyIgnore]
        public override string BlockName => nameof(t0425OutBlock1);

        /// <summary>
        /// 주문번호 [10]
        /// </summary>
        public long ordno { get; set; }

        /// <summary>
        /// 종목번호 [12]
        /// </summary>
        public string expcode { get; set; }

        /// <summary>
        /// 구분 [10]
        /// </summary>
        public string medosu { get; set; }

        /// <summary>
        /// 주문수량 [9]
        /// </summary>
        public long qty { get; set; }

        /// <summary>
        /// 주문가격 [9]
        /// </summary>
        public long price { get; set; }

        /// <summary>
        /// 체결수량 [9]
        /// </summary>
        public long cheqty { get; set; }

        /// <summary>
        /// 체결가격 [9]
        /// </summary>
        public long cheprice { get; set; }

        /// <summary>
        /// 미체결잔량 [9]
        /// </summary>
        public long ordrem { get; set; }

        /// <summary>
        /// 확인수량 [9]
        /// </summary>
        public long cfmqty { get; set; }

        /// <summary>
        /// 상태 [10]
        /// </summary>
        public string status { get; set; }

        /// <summary>
        /// 원주문번호 [10]
        /// </summary>
        public long orgordno { get; set; }

        /// <summary>
        /// 유형 [20]
        /// </summary>
        public string ordgb { get; set; }

        /// <summary>
        /// 주문시간 [8]
        /// </summary>
        public string ordtime { get; set; }

        /// <summary>
        /// 주문매체 [10]
        /// </summary>
        public string ordermtd { get; set; }

        /// <summary>
        /// 처리순번 [10]
        /// </summary>
        public long sysprocseq { get; set; }

        /// <summary>
        /// 호가유형 [2]
        /// </summary>
        public string hogagb { get; set; }

        /// <summary>
        /// 현재가 [8]
        /// </summary>
        public long price1 { get; set; }

        /// <summary>
        /// 주문구분 [2]
        /// </summary>
        public string orggb { get; set; }

        /// <summary>
        /// 신용구분 [2]
        /// </summary>
        public string singb { get; set; }

        /// <summary>
        /// 대출일자 [8]
        /// </summary>
        public string loandt { get; set; }
    }
}
