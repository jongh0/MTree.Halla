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

        public bool CanBuy(TradeInformation info)
        {
            try
            {
                if (info == null) throw new ArgumentNullException(nameof(info));
                if (info.SelectedAccount == null) throw new ArgumentNullException(nameof(info.SelectedAccount));
                if (info.Order == null) throw new ArgumentNullException(nameof(info.Order));

                if (info.Order.OrderType != OrderTypes.BuyNew) return false;

                return info.Order.Price * info.Order.Quantity <= info.SelectedAccount.OrderableAmount;
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
                if (info.SelectedAccount == null) throw new ArgumentNullException(nameof(info.SelectedAccount));
                if (info.Order == null) throw new ArgumentNullException(nameof(info.Order));

                var holdingStock = info.SelectedAccount.HoldingStocks.FirstOrDefault(s => s.Code == info.Order.Code && s.Quantity > 0);
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
