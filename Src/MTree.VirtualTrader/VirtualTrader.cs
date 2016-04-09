using MTree.Trader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MTree.VirtualTrader
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class VirtualTrader : ITrader
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public VirtualAccountManager AccountManager { get; set; } = new VirtualAccountManager();

        public List<string> GetAccountList()
        {
            return AccountManager.GetAccountList();
        }

        public int GetDeposit(string accountCode)
        {
            var account = AccountManager.GetAccout(accountCode);

            if (account != null)
                return account.Deposit;
            else
                return 0;
        }

        public List<HoldingStock> GetHoldingList(string accountCode)
        {
            var account = AccountManager.GetAccout(accountCode);
            if (account != null)
                return account.HoldingStockList;

            return null;
        }

        public OrderResult MakeOrder(Order order)
        {
            return AccountManager.MakeOrder(order);
        }
    }
}
