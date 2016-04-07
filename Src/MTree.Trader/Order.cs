using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.Trader
{
    public enum OrderTypes
    {
        Buy,
        BuyModify,
        Sell,
        SellModify,
        Cancel,
    }

    [Serializable]
    public class Order
    {
        public string Account { get; set; }

        public string Code { get; set; }
        
        public int Amount { get; set; }

        public int OrderPrice { get; set; }

        public OrderTypes OrderType { get; set; }
    }
}
