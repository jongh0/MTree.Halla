using CommonLib.Firm.Ebest.Block;
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
    public class AccountInformation
    {
        /// <summary>
        /// 계좌번호
        /// </summary>
        [DataMember]
        public string AccountNumber { get; set; }

        /// <summary>
        /// 추정순자산
        /// </summary>
        [DataMember]
        public long EstimatedNetWorth { get; set; }

        /// <summary>
        /// 실현손익
        /// </summary>
        [DataMember]
        public long RealizedProfit { get; set; }

        /// <summary>
        /// 매입금액
        /// </summary>
        [DataMember]
        public long PurchasePrice { get; set; }

        /// <summary>
        /// 평가금액
        /// </summary>
        [DataMember]
        public long EvaluationPrice { get; set; }

        /// <summary>
        /// 평가손익
        /// </summary>
        [DataMember]
        public long EvaluationProfit { get; set; }

        /// <summary>
        /// 보유주식
        /// </summary>
        [DataMember]
        public List<HoldingStock> HoldingStocks { get; set; } = new List<HoldingStock>();

        public override string ToString()
        {
            return PropertyUtility.PrintNameValues(this, Environment.NewLine);
        }
    }

    public class AccountInfoMappingProfile : AutoMapper.Profile
    {
        public AccountInfoMappingProfile()
        {
            CreateMap<t0424OutBlock, AccountInformation>()
                .ForMember(dest => dest.EstimatedNetWorth,
                           opts => opts.MapFrom(src => src.sunamt))
                .ForMember(dest => dest.RealizedProfit,
                           opts => opts.MapFrom(src => src.dtsunik))
                .ForMember(dest => dest.PurchasePrice,
                           opts => opts.MapFrom(src => src.mamt))
                .ForMember(dest => dest.EvaluationPrice,
                           opts => opts.MapFrom(src => src.tappamt))
                .ForMember(dest => dest.EvaluationProfit,
                           opts => opts.MapFrom(src => src.tdtsunik))
                .ForMember(dest => dest.HoldingStocks,
                           opts => opts.Ignore())
                .ForMember(dest => dest.AccountNumber,
                           opts => opts.Ignore());
        }
    }
}
