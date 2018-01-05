using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trader
{
    public class HoldingStock
    {
        public int Amount { get; set; }

        public string Code { get; set; }

        public int PurchasePrice { get; set; }

        public int SellableAmount { get; set; }
    }
}
