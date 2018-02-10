using Trader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VirtualTrader
{
    public class VirtualAccountManager
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private Dictionary<string, VirtualAccount> AccountList { get; set; } = new Dictionary<string, VirtualAccount>();

        public List<string> GetAccountList()
        {
            List<string> list = new List<string>();

            try
            {
                foreach (var accNum in AccountList.Keys)
                {
                    list.Add(accNum);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return list;
        }

        public VirtualAccount GetAccout(string accNum)
        {
            if (AccountList.ContainsKey(accNum) == true)
                return AccountList[accNum];
            else
                return null;
        }

        public bool MakeOrder(StockOrder order)
        {
            try
            {
                var account = GetAccout(order.AccountNumber);
                if (account != null)
                    return account.MakeOrder(order);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return false;
        }
    }
}
