using CommonLib.Firm.Ebest;
using CommonLib.Firm.Ebest.Block;
using CommonLib.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trader
{
    public class Order
    {
        public string AccountNumber { get; set; }

        public string AccountPassword { get; set; }

        public string Code { get; set; }

        public long Quantity { get; set; }

        public long Price { get; set; }

        public PriceTypes PriceType { get; set; }

        public OrderTypes OrderType { get; set; }

        public string OriginOrderNumber { get; set; }

        public override string ToString()
        {
            return PropertyUtility.PrintNameValues(this);
        }
    }

    public class OrderMappingProfile : AutoMapper.Profile
    {
        public OrderMappingProfile()
        {
            CreateMap<Order, CSPAT00600InBlock1>()
                .ForMember(dest => dest.AcntNo,
                           opts => opts.MapFrom(src => src.AccountNumber))
                .ForMember(dest => dest.InptPwd,
                           opts => opts.MapFrom(src => src.AccountPassword))
                .ForMember(dest => dest.IsuNo,
                           opts => opts.MapFrom(src => src.Code))
                .ForMember(dest => dest.OrdQty,
                           opts => opts.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.BnsTpCode,
                           opts => opts.MapFrom(src => src.OrderType == OrderTypes.BuyNew ? "2" : "1"))
                .ForMember(dest => dest.OrdPrc,
                           opts => opts.MapFrom(src => src.PriceType == PriceTypes.LimitPrice ? src.Price : 0))
                .ForMember(dest => dest.OrdprcPtnCode,
                           opts => opts.MapFrom(src => src.PriceType == PriceTypes.LimitPrice ? "00" : "03"))
                .ForMember(dest => dest.MgntrnCode,
                           opts => opts.MapFrom(src => "000"))
                .ForMember(dest => dest.LoanDt,
                           opts => opts.MapFrom(src => ""))
                .ForMember(dest => dest.OrdCndiTpCode,
                        opts => opts.MapFrom(src => "0"));

            CreateMap<Order, CSPAT00700InBlock1>()
                .ForMember(dest => dest.OrgOrdNo,
                           opts => opts.MapFrom(src => long.Parse(src.OriginOrderNumber)))
                .ForMember(dest => dest.AcntNo,
                           opts => opts.MapFrom(src => src.AccountNumber))
                .ForMember(dest => dest.InptPwd,
                           opts => opts.MapFrom(src => src.AccountPassword))
                .ForMember(dest => dest.IsuNo,
                           opts => opts.MapFrom(src => src.Code))
                .ForMember(dest => dest.OrdQty,
                           opts => opts.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.OrdPrc,
                           opts => opts.MapFrom(src => src.PriceType == PriceTypes.LimitPrice ? src.Price : 0))
                .ForMember(dest => dest.OrdprcPtnCode,
                           opts => opts.MapFrom(src => src.PriceType == PriceTypes.LimitPrice ? "00" : "03"))
                .ForMember(dest => dest.OrdCndiTpCode,
                        opts => opts.MapFrom(src => "0"));

            CreateMap<Order, CSPAT00800InBlock1>()
                .ForMember(dest => dest.OrgOrdNo,
                           opts => opts.MapFrom(src => long.Parse(src.OriginOrderNumber)))
                .ForMember(dest => dest.AcntNo,
                           opts => opts.MapFrom(src => src.AccountNumber))
                .ForMember(dest => dest.InptPwd,
                           opts => opts.MapFrom(src => src.AccountPassword))
                .ForMember(dest => dest.IsuNo,
                           opts => opts.MapFrom(src => src.Code))
                .ForMember(dest => dest.OrdQty,
                           opts => opts.MapFrom(src => src.Quantity));
        }
    }
}
