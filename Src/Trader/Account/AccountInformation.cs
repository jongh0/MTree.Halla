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
    public class AccountInformation
    {
        /// <summary>
        /// 계좌번호
        /// </summary>
        [DataMember]
        public string AccountNumber { get; set; }

        /// <summary>
        /// 계좌명
        /// </summary>
        [DataMember]
        public string AccountName { get; set; }

        /// <summary>
        /// 현금주문가능금액
        /// </summary>
        [DataMember]
        public long OrderableAmount { get; set; }

        /// <summary>
        /// 출금가능금액
        /// </summary>
        [DataMember]
        public long WithdrawableAmount { get; set; }

        /// <summary>
        /// 잔고평가금액
        /// </summary>
        [DataMember]
        public long EvaluationAmount { get; set; }

        /// <summary>
        /// 매입금액
        /// </summary>
        [DataMember]
        public long PurchaseAmount { get; set; }

        /// <summary>
        /// 손익율
        /// </summary>
        [DataMember]
        public double ProfitRate { get; set; }

        /// <summary>
        /// 투자원금
        /// </summary>
        [DataMember]
        public long InvestAmount { get; set; }

        /// <summary>
        /// 투자손익금액
        /// </summary>
        [DataMember]
        public long InvestProfitAmount { get; set; }

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
            CreateMap<CSPAQ12300OutBlock2, AccountInformation>(AutoMapper.MemberList.None)
                .ForMember(dest => dest.AccountName,
                           opts => opts.MapFrom(src => src.AcntNm))
                .ForMember(dest => dest.OrderableAmount,
                           opts => opts.MapFrom(src => src.MnyOrdAbleAmt))
                .ForMember(dest => dest.WithdrawableAmount,
                           opts => opts.MapFrom(src => src.MnyoutAbleAmt))
                .ForMember(dest => dest.EvaluationAmount,
                           opts => opts.MapFrom(src => src.BalEvalAmt))
                .ForMember(dest => dest.PurchaseAmount,
                           opts => opts.MapFrom(src => src.PchsAmt))
                .ForMember(dest => dest.ProfitRate,
                           opts => opts.MapFrom(src => src.PnlRat))
                .ForMember(dest => dest.InvestAmount,
                           opts => opts.MapFrom(src => src.InvstOrgAmt))
                .ForMember(dest => dest.InvestProfitAmount,
                           opts => opts.MapFrom(src => src.InvstPlAmt));
        }
    }
}
