﻿using Trader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trader.Account;

namespace VirtualTrader
{
    public class VirtualAccount
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public int Deposit { get; private set; } = 0;

        public List<StockOrder> OrderList { get; private set; } = new List<StockOrder>();

        public List<HoldingStock> HoldingStockList { get; private set; } = new List<HoldingStock>();

        public bool MakeOrder(StockOrder order)
        {
            var result = new StockOrderResult();

            try
            {
                // TODO : Deposit 모자라서 거래 안되는 경우 처리
                // TODO : Account class 자체가 DB에 들어가야하나?
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return false;
        }
    }
}
