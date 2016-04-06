using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.Trader
{
    public interface ITrader
    {
        List<string> GetAccounts();

        int GetDeposit(string account);

        OrderResult Order(Order order);

        List<HoldingStock> GetHoldingStocks(string account);
    }
}
