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

        public List<string> GetAccountList()
        {
            if (State != CommunicationState.Opened) return null;
            return base.Channel.GetAccountList();
        }

        public long GetDeposit(string accNum, string accPw)
        {
            if (State != CommunicationState.Opened) return 0;
            return base.Channel.GetDeposit(accNum, accPw);
        }

        public List<HoldingStock> GetHoldingList(string accNum)
        {
            if (State != CommunicationState.Opened) return null;
            return base.Channel.GetHoldingList(accNum);
        }

        public bool MakeOrder(Order order)
        {
            if (State != CommunicationState.Opened) return false;
            return base.Channel.MakeOrder(order);
        }
    }
}
