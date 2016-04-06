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

        public List<string> GetAccounts()
        {
            throw new NotImplementedException();
        }

        public int GetDeposit(string account)
        {
            throw new NotImplementedException();
        }

        public List<HoldingStock> GetHoldingStocks(string account)
        {
            throw new NotImplementedException();
        }

        public OrderResult MakeOrder(Order order)
        {
            throw new NotImplementedException();
        }
    }
}
