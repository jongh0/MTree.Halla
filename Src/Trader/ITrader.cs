using CommonLib.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trader.Account;

namespace Trader
{
    public interface ITrader
    {
        event Action<TraderStateTypes, string> StateNotified;

        List<string> GetAccountNumbers();

        List<AccountInformation> GetAccountInformations();

        bool MakeOrder(StockOrder order);
    }
}
