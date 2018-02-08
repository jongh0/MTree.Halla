﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trader;
using Trader.Account;

namespace Strategy.TradeAvailable
{
    public interface ITradeAvailable
    {
        string Name { get; set; }

        bool CanBuy(TradeInformation info);

        bool CanSell(TradeInformation info);
    }
}
