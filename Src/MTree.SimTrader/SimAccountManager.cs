using MTree.Trader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.SimTrader
{
    public class SimAccountManager
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private Dictionary<string, SimAccount> AccountList { get; set; } = new Dictionary<string, SimAccount>();

        public List<string> GetAccountList()
        {
            List<string> list = new List<string>();

            foreach (var account in AccountList.Keys)
            {
                list.Add(account);
            }

            return list;
        }

        public SimAccount GetAccout(string accountCode)
        {
            if (AccountList.ContainsKey(accountCode) == true)
                return AccountList[accountCode];
            else
                return null;
        }

        public OrderResult MakeOrder(Order order)
        {
            var result = new OrderResult();



            return result;
        }
    }
}
