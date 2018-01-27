using Trader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trader.Account;

namespace VirtualTrader
{
    public partial class VirtualTrader_
    {
        private OrderResult CurrOrderResult { get; set; }

        public VirtualAccountManager AccountManager { get; set; } = new VirtualAccountManager();

        public List<AccountInfo> GetAccountInfoList()
        {
            throw new NotImplementedException();
        }

        public List<string> GetAccountList()
        {
            return AccountManager.GetAccountList();
        }

        public long GetDeposit(string accNum, string accPw)
        {
            var account = AccountManager.GetAccout(accNum);

            if (account != null)
                return account.Deposit;
            else
                return 0;
        }

        public List<HoldingStock> GetHoldingList(string accNum)
        {
            var account = AccountManager.GetAccout(accNum);
            if (account != null)
                return account.HoldingStockList;

            return null;
        }

        public bool MakeOrder(Order order)
        {
            return AccountManager.MakeOrder(order);
        }
    }
}
