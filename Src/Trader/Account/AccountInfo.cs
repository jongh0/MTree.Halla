using CommonLib.Firm.Ebest.Block;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trader.Account
{
    public class AccountInfo
    {
        public List<HoldingStock> HoldingStocks { get; set; } = new List<HoldingStock>();

        /// <summary>
        /// 계좌번호
        /// </summary>
        public string AccountNumber { get; set; }

        /// <summary>
        /// 추정순자산
        /// </summary>
        public long EstimatedNetWorth { get; set; }

        /// <summary>
        /// 실현손익
        /// </summary>
        public long RealizedProfit { get; set; }

        /// <summary>
        /// 매입금액
        /// </summary>
        public long PurchasePrice { get; set; }

        /// <summary>
        /// 평가금액
        /// </summary>
        public long EvaluationPrice { get; set; }

        /// <summary>
        /// 평가손익
        /// </summary>
        public long EvaluationProfit { get; set; }
    }

    public class AccountInfoMappingProfile : AutoMapper.Profile
    {
        public AccountInfoMappingProfile()
        {
            CreateMap<t0424OutBlock, AccountInfo>()
                .ForMember(dest => dest.EstimatedNetWorth,
                           opts => opts.MapFrom(src => src.sunamt))
                .ForMember(dest => dest.RealizedProfit,
                           opts => opts.MapFrom(src => src.dtsunik))
                .ForMember(dest => dest.PurchasePrice,
                           opts => opts.MapFrom(src => src.mamt))
                .ForMember(dest => dest.EvaluationPrice,
                           opts => opts.MapFrom(src => src.tappamt))
                .ForMember(dest => dest.EvaluationProfit,
                           opts => opts.MapFrom(src => src.tdtsunik));
        }
    }
}
