using MTree.Trader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.VirtualTrader
{
    public class VirtualAccountManager
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private Dictionary<string, VirtualAccount> AccountList { get; set; } = new Dictionary<string, VirtualAccount>();

        public List<string> GetAccountList()
        {
            List<string> list = new List<string>();

            try
            {
                foreach (var account in AccountList.Keys)
                {
                    list.Add(account);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return list;
        }

        public VirtualAccount GetAccout(string accountCode)
        {
            if (AccountList.ContainsKey(accountCode) == true)
                return AccountList[accountCode];
            else
                return null;
        }

        public OrderResult MakeOrder(Order order)
        {
            try
            {
                var account = GetAccout(order.Account);
                if (account != null)
                    return account.MakeOrder(order);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return null;
        }
    }
}
