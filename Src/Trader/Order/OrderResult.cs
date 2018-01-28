using CommonLib.Firm.Ebest.Block;
using CommonLib.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trader
{
    public class OrderResult
    {
        public string OrderNumber { get; set; }
        
        public string Code { get; set; }

        public OrderResultTypes ResultType { get; set; }

        public long ConcludedQuantity { get; set; }

        public long ConcludedPrice { get; set; }

        public long OrderedQuantity { get; set; }

        public long OrderedPrice { get; set; }

        public string AccountNumber { get; set; }

        public override string ToString()
        {
            return PropertyUtility.PrintNameValues(this);
        }
    }

    public class OrderResultMappingProfile : AutoMapper.Profile
    {
        public OrderResultMappingProfile()
        {
            CreateMap<SC0OutBlock, OrderResult>(AutoMapper.MemberList.None)
                .ForMember(dest => dest.AccountNumber,
                           opts => opts.MapFrom(src => src.accno))
                .ForMember(dest => dest.OrderNumber,
                           opts => opts.MapFrom(src => src.ordno.ToString()))
                .ForMember(dest => dest.Code,
                           opts => opts.MapFrom(src => src.expcode))
                .ForMember(dest => dest.OrderedQuantity,
                           opts => opts.MapFrom(src => src.ordqty))
                .ForMember(dest => dest.ResultType,
                           opts => opts.MapFrom(src => OrderResultTypes.Submitted));

            CreateMap<SC1OutBlock, OrderResult>(AutoMapper.MemberList.None)
                .ForMember(dest => dest.AccountNumber,
                           opts => opts.MapFrom(src => src.accno))
                .ForMember(dest => dest.OrderNumber,
                           opts => opts.MapFrom(src => src.ordno.ToString()))
                .ForMember(dest => dest.Code,
                           opts => opts.MapFrom(src => src.Isuno))
                .ForMember(dest => dest.OrderedQuantity,
                           opts => opts.MapFrom(src => src.ordqty))
                .ForMember(dest => dest.OrderedPrice,
                           opts => opts.MapFrom(src => src.ordprc))
                .ForMember(dest => dest.ConcludedQuantity,
                           opts => opts.MapFrom(src => src.execqty))
                .ForMember(dest => dest.ConcludedPrice,
                           opts => opts.MapFrom(src => src.execprc))
                .ForMember(dest => dest.ResultType,
                           opts => opts.MapFrom(src => OrderResultTypes.Concluded));

            CreateMap<SC2OutBlock, OrderResult>(AutoMapper.MemberList.None)
                .ForMember(dest => dest.AccountNumber,
                           opts => opts.MapFrom(src => src.accno))
                .ForMember(dest => dest.OrderNumber,
                           opts => opts.MapFrom(src => src.ordno.ToString()))
                .ForMember(dest => dest.Code,
                           opts => opts.MapFrom(src => src.Isuno))
                .ForMember(dest => dest.OrderedQuantity,
                           opts => opts.MapFrom(src => src.ordqty))
                .ForMember(dest => dest.OrderedPrice,
                           opts => opts.MapFrom(src => src.ordprc))
                .ForMember(dest => dest.ConcludedQuantity,
                           opts => opts.MapFrom(src => src.execqty))
                .ForMember(dest => dest.ConcludedPrice,
                           opts => opts.MapFrom(src => src.execprc))
                .ForMember(dest => dest.ResultType,
                           opts => opts.MapFrom(src => OrderResultTypes.Modified));

            CreateMap<SC3OutBlock, OrderResult>(AutoMapper.MemberList.None)
                .ForMember(dest => dest.AccountNumber,
                           opts => opts.MapFrom(src => src.accno))
                .ForMember(dest => dest.OrderNumber,
                           opts => opts.MapFrom(src => src.ordno.ToString()))
                .ForMember(dest => dest.Code,
                           opts => opts.MapFrom(src => src.Isuno))
                .ForMember(dest => dest.OrderedQuantity,
                           opts => opts.MapFrom(src => src.ordqty))
                .ForMember(dest => dest.OrderedPrice,
                           opts => opts.MapFrom(src => src.ordprc))
                .ForMember(dest => dest.ConcludedQuantity,
                           opts => opts.MapFrom(src => src.execqty))
                .ForMember(dest => dest.ConcludedPrice,
                           opts => opts.MapFrom(src => src.execprc))
                .ForMember(dest => dest.ResultType,
                           opts => opts.MapFrom(src => OrderResultTypes.Canceled));

            CreateMap<SC4OutBlock, OrderResult>(AutoMapper.MemberList.None)
                .ForMember(dest => dest.AccountNumber,
                           opts => opts.MapFrom(src => src.accno))
                .ForMember(dest => dest.OrderNumber,
                           opts => opts.MapFrom(src => src.ordno.ToString()))
                .ForMember(dest => dest.Code,
                           opts => opts.MapFrom(src => src.Isuno))
                .ForMember(dest => dest.OrderedQuantity,
                           opts => opts.MapFrom(src => src.ordqty))
                .ForMember(dest => dest.OrderedPrice,
                           opts => opts.MapFrom(src => src.ordprc))
                .ForMember(dest => dest.ConcludedQuantity,
                           opts => opts.MapFrom(src => src.execqty))
                .ForMember(dest => dest.ConcludedPrice,
                           opts => opts.MapFrom(src => src.execprc))
                .ForMember(dest => dest.ResultType,
                           opts => opts.MapFrom(src => OrderResultTypes.Rejected));
        }
    }
}
