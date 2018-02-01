using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trader;
using Trader.Account;

namespace Strategy.TradeAvailable
{
    public class AccountAvailable : ITradeAvailable
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public string Name { get; set; } = nameof(AccountAvailable);

        public bool CanBuy(AccountInformation accInfo, Order order)
        {
            try
            {
                if (accInfo == null) throw new ArgumentNullException(nameof(accInfo));
                if (order == null) throw new ArgumentNullException(nameof(order));

                if (order.OrderType != OrderTypes.BuyNew) return false;

                return order.Price * order.Quantity <= accInfo.RealizedProfit; // 틀린 거 같음
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return false;
        }

        public bool CanSell(AccountInformation accInfo, Order order)
        {
            var holdingStock = accInfo.HoldingStocks.FirstOrDefault(s => s.Code == order.Code && s.Quantity > 0);
            return holdingStock != null;
        }
    }
}
