﻿using DataStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Strategy.TradeDecision
{
    public interface ITradeDecision
    {
        TradeTypes GetTradeType(TradeInformation info);
    }
}
