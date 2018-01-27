using Trader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using RealTimeProvider;
using Trader.Account;

namespace Trader
{
    public partial class TraderClient : DuplexClientBase<IRealTimeTrader>, IRealTimeTrader
    {
        public TraderClient(InstanceContext callbackInstance, string endpointConfigurationName) : base(callbackInstance, endpointConfigurationName) { }

        public void SendMessage(MessageTypes type, string message)
        {
            if (State != CommunicationState.Opened) return;
            base.Channel.SendMessage(type, message);
        }

        public void RegisterTraderContract(Guid clientId, TraderContract contract)
        {
            if (State != CommunicationState.Opened) return;
            base.Channel.RegisterTraderContract(clientId, contract);
        }

        public void UnregisterTraderContract(Guid clientId)
        {
            if (State != CommunicationState.Opened) return;
            base.Channel.UnregisterTraderContract(clientId);
        }

        public List<AccountInfo> GetAccountInfoList()
        {
            if (State != CommunicationState.Opened) return null;
            return base.Channel.GetAccountInfoList();
        }

        public bool MakeOrder(Order order)
        {
            if (State != CommunicationState.Opened) return false;
            return base.Channel.MakeOrder(order);
        }
    }
}
