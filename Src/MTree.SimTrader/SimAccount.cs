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

        public List<Order> OrderList { get; set; } = new List<Order>();

        public List<HoldingStock> HoldingStockList { get; set; } = new List<HoldingStock>();
    }
}
