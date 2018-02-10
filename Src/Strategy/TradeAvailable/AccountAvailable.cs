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

        public bool CanBuy(TradeInformation info)
        {
            try
            {
                if (info == null) throw new ArgumentNullException(nameof(info));
                if (info.Account == null) throw new ArgumentNullException(nameof(info.Account));
                if (info.Order == null) throw new ArgumentNullException(nameof(info.Order));

                if (info.Order.OrderType != OrderTypes.BuyNew) return false;

                return info.Order.Price * info.Order.Quantity <= info.Account.OrderableAmount;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return false;
        }

        public bool CanSell(TradeInformation info)
        {
            try
            {
                if (info == null) throw new ArgumentNullException(nameof(info));
                if (info.Account == null) throw new ArgumentNullException(nameof(info.Account));
                if (info.Order == null) throw new ArgumentNullException(nameof(info.Order));

                var holdingStock = info.Account.HoldingStocks.FirstOrDefault(s => s.Code == info.Order.Code && s.Quantity > 0);
                return holdingStock != null;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return false;
        }
    }
}
