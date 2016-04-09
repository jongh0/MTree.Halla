using MTree.Trader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.VirtualTrader
{
    public class VirtualAccount
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public int Deposit { get; private set; } = 0;

        public List<Order> OrderList { get; private set; } = new List<Order>();

        public List<HoldingStock> HoldingStockList { get; private set; } = new List<HoldingStock>();

        public OrderResult MakeOrder(Order order)
        {
            var result = new OrderResult();

            try
            {
                // TODO : Deposit 모자라서 거래 안되는 경우 처리
                // TODO : Account class 자체가 DB에 들어가야하나?
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return result;
        }
    }
}
