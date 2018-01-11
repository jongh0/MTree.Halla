using Trader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using RealTimeProvider;

namespace Trader
{
    public partial class TraderClient : DuplexClientBase<IRealTimeTrader>, IRealTimeTrader
    {
        public TraderClient(InstanceContext callbackInstance, string endpointConfigurationName) : base(callbackInstance, endpointConfigurationName) { }

        public void NotifyMessage(MessageTypes type, string message)
        {
            base.Channel.NotifyMessage(type, message);
        }

        public void RegisterTraderContract(Guid clientId, TraderContract contract)
        {
            base.Channel.RegisterTraderContract(clientId, contract);
        }

        public void UnregisterTraderContract(Guid clientId)
        {
            base.Channel.UnregisterTraderContract(clientId);
        }

        public List<string> GetAccountList()
        {
            return base.Channel.GetAccountList();
        }

        public long GetDeposit(string accNum, string accPw)
        {
            return base.Channel.GetDeposit(accNum, accPw);
        }

        public List<HoldingStock> GetHoldingList(string accNum)
        {
            return base.Channel.GetHoldingList(accNum);
        }

        public bool MakeOrder(Order order)
        {
            return base.Channel.MakeOrder(order);
        }
    }
}
