using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.Trader
{
    public enum OrderTypes
    {
        BuyNew = 1,
        SellNew = 2,
        BuyCancel = 3,
        SellCancel = 4,
        BuyModify = 5,
        SellModify = 6,
    }

    public enum PriceTypes
    {
        LimitPrice,
        MarketPrice,
        // 00:지정가, 03:시장가, 05:조건부지정가, 06:최유리지정가, 07:최우선지정가,
        // 10:지정가IOC, 13:시장가IOC, 16:최유리IOC, 20:지정가FOK, 23:시장가FOK,
        // 26:최유리FOK, 61:장개시전시간외, 62:시간외단일가매매, 81:시간외종가
    }

    [Serializable]
    public class Order
    {
        public string AccountNumber { get; set; }

        public string AccountPassword { get; set; }

        public string Code { get; set; }
        
        public int Quantity { get; set; }

        public int Price { get; set; }

        public PriceTypes PriceType { get; set; }

        public OrderTypes OrderType { get; set; }

        public string OriginOrderNumber { get; set; }

        public override string ToString()
        {
            return $"{AccountNumber}/{Code}/{Quantity}/{Price}/{PriceType}/{OrderType}/{OriginOrderNumber}";
        }
    }
}
