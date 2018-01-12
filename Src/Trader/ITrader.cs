using CommonLib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trader
{
    public interface ITrader
    {
        event Action<string> StateNotified;

        List<string> GetAccountList();

        long GetDeposit(string accNum, string accPw);

        List<HoldingStock> GetHoldingList(string accNum);

        bool MakeOrder(Order order);
    }
}
