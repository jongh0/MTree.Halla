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
