using MTree.Trader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MTree.Trader
{
    public partial class TraderClient : DuplexClientBase<ITrader>, ITrader
    {
        public TraderClient(InstanceContext callbackInstance, string endpointConfigurationName) : base(callbackInstance, endpointConfigurationName) { }

        public List<string> GetAccountList()
        {
            return base.Channel.GetAccountList();
        }

        public int GetDeposit(string accountCode)
        {
            return base.Channel.GetDeposit(accountCode);
        }

        public List<HoldingStock> GetHoldingList(string accountCode)
        {
            return base.Channel.GetHoldingList(accountCode);
        }

        public OrderResult MakeOrder(Order order)
        {
            return base.Channel.MakeOrder(order);
        }
    }
}
