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
        public AccountInformation SelectedAccount { get; set; }

        public IEnumerable<AccountInformation> AccountInfos { get; set; }

        public Order Order { get; set; }

        public Subscribable Subscribable { get; set; }
    }
}
