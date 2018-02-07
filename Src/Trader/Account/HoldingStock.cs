using FirmLib.Ebest.Block;
using CommonLib.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Trader.Account
{
    [DataContract]
    public class HoldingStock
    {
        /// <summary>
        /// 종목번호
        /// </summary>
        [DataMember]
        public string Code { get; set; }

        /// <summary>
        /// 종목명
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// 잔고수량
        /// </summary>
        [DataMember]
        public int Quantity { get; set; }

        /// <summary>
        /// 매도가능수량
        /// </summary>
        [DataMember]
        public int SellableQuantity { get; set; }

        /// <summary>
        /// 평균단가
        /// </summary>
        [DataMember]
        public long AveragePrice { get; set; }

        /// <summary>
        /// 매입금액
        /// </summary>
        [DataMember]
        public int PurchaseAmount { get; set; }

        /// <summary>
        /// 평가금액
        /// </summary>
        [DataMember]
        public long EvaluationAmount { get; set; }

        /// <summary>
        /// 평가손익
        /// </summary>
        [DataMember]
        public long EvaluationProfit { get; set; }

        /// <summary>
        /// 수익율
        /// </summary>
        [DataMember]
        public float ProfitRate { get; set; }

        public override string ToString()
        {
            return PropertyUtility.PrintNameValues(this, Environment.NewLine);
        }
    }

    public class HoldingStockMappingProfile : AutoMapper.Profile
    {
        public HoldingStockMappingProfile()
        {
             CreateMap<CSPAQ12300OutBlock3, HoldingStock>(AutoMapper.MemberList.None)
                .ForMember(dest => dest.Code,
                           opts => opts.MapFrom(src => src.IsuNo))
                .ForMember(dest => dest.Name,
                           opts => opts.MapFrom(src => src.IsuNm))
                .ForMember(dest => dest.Quantity,
                           opts => opts.MapFrom(src => src.BalQty))
                .ForMember(dest => dest.SellableQuantity,
                           opts => opts.MapFrom(src => src.SellAbleQty))
                .ForMember(dest => dest.AveragePrice,
                           opts => opts.MapFrom(src => src.AvrUprc))
                .ForMember(dest => dest.PurchaseAmount,
                           opts => opts.MapFrom(src => src.PchsAmt))
                .ForMember(dest => dest.EvaluationAmount,
                           opts => opts.MapFrom(src => src.BalEvalAmt))
                .ForMember(dest => dest.EvaluationProfit,
                           opts => opts.MapFrom(src => src.EvalPnl))
                .ForMember(dest => dest.ProfitRate,
                           opts => opts.MapFrom(src => src.PnlRat));
        }
    }
}
