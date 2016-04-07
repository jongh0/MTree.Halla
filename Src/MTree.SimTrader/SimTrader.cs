using MTree.Trader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MTree.SimTrader
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class SimTrader : ITrader
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public SimAccountManager AccountManager { get; set; } = new SimAccountManager();

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
            List<HoldingStock> holdings = new List<HoldingStock>();

            var account = AccountManager.GetAccout(accountCode);
            if (account != null)
            {
                foreach (var holdingStock in account.HoldingStockList)
                {
                    holdings.Add(holdingStock);
                }
            }

            return holdings;
        }

        public OrderResult MakeOrder(Order order)
        {
            return AccountManager.MakeOrder(order);
        }
    }
}
