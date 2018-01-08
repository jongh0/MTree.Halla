﻿using Trader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using RealTimeProvider;

namespace Trader
{
    public partial class TraderClient : DuplexClientBase<ITrader>, ITrader
    {
        public TraderClient(InstanceContext callbackInstance, string endpointConfigurationName) : base(callbackInstance, endpointConfigurationName) { }

        public void NotifyMessage(MessageTypes type, string message)
        {
            base.Channel.NotifyMessage(type, message);
        }

        public void RegisterContract(Guid clientId, TraderContract contract)
        {
            base.Channel.RegisterContract(clientId, contract);
        }

        public void UnregisterContract(Guid clientId)
        {
            base.Channel.UnregisterContract(clientId);
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