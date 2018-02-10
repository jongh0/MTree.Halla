using DataStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trader;
using Trader.Account;

namespace Strategy
{
    public class TradeInformation
    {
        public AccountInformation Account { get; set; }

        public StockOrder Order { get; set; }

        public Subscribable Subscribable { get; set; }
    }
}
