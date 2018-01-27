using CommonLib.Firm.Ebest.Block;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trader.Account
{
    public class HoldingStock
    {
        /// <summary>
        /// 종목번호
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 종목명
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 잔고수량
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 매도가능수량
        /// </summary>
        public int SellableQuantity { get; set; }

        /// <summary>
        /// 평균단가
        /// </summary>
        public long AveragePrice { get; set; }

        /// <summary>
        /// 매입금액
        /// </summary>
        public int PurchasePrice { get; set; }

        /// <summary>
        /// 평가금액
        /// </summary>
        public long EvaluationPrice { get; set; }

        /// <summary>
        /// 평가손익
        /// </summary>
        public long EvaluationProfit { get; set; }

        /// <summary>
        /// 수익율
        /// </summary>
        public float ProfitRate { get; set; }
    }

    public class HoldingStockMappingProfile : AutoMapper.Profile
    {
        public HoldingStockMappingProfile()
        {
            CreateMap<t0424OutBlock1, HoldingStock>()
                .ForMember(dest => dest.Code,
                           opts => opts.MapFrom(src => src.expcode))
                .ForMember(dest => dest.Name,
                           opts => opts.MapFrom(src => src.hname))
                .ForMember(dest => dest.Quantity,
                           opts => opts.MapFrom(src => src.janqty))
                .ForMember(dest => dest.SellableQuantity,
                           opts => opts.MapFrom(src => src.mdposqt))
                .ForMember(dest => dest.AveragePrice,
                           opts => opts.MapFrom(src => src.pamt))
                .ForMember(dest => dest.PurchasePrice,
                           opts => opts.MapFrom(src => src.mamt))
                .ForMember(dest => dest.EvaluationPrice,
                           opts => opts.MapFrom(src => src.appamt))
                .ForMember(dest => dest.EvaluationProfit,
                           opts => opts.MapFrom(src => src.dtsunik))
                .ForMember(dest => dest.ProfitRate,
                           opts => opts.MapFrom(src => src.sunikrt));
        }
    }
}
