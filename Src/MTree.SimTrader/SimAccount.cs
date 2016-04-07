using MTree.Trader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.SimTrader
{
    public class SimAccount
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public int Deposit { get; set; }

        public Dictionary<string, Order> OrderList { get; set; } = new Dictionary<string, Order>();

        public Dictionary<string, HoldingStock> HoldingStockList { get; set; } = new Dictionary<string, HoldingStock>();
    }
}
