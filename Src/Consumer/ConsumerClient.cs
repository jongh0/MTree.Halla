﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel.Channels;
using System.ServiceModel;
using DataStructure;
using RealTimeProvider;

namespace Consumer
{
    public partial class ConsumerClient : DuplexClientBase<IRealTimeConsumer>, IRealTimeConsumer
    {
        public ConsumerClient(InstanceContext callbackInstance, string endpointConfigurationName) : base(callbackInstance, endpointConfigurationName) { }

        public void NotifyMessage(MessageTypes type, string message)
        {
            base.Channel.NotifyMessage(type, message);
        }

        public void RegisterContract(Guid clientId, SubscribeContract subscription)
        {
            base.Channel.RegisterContract(clientId, subscription);
        }

        public void UnregisterContractAll(Guid clientId)
        {
            base.Channel.UnregisterContractAll(clientId);
        }

        public void UnregisterContract(Guid clientId, SubscribeTypes type)
        {
            base.Channel.UnregisterContract(clientId, type);
        }

        public List<Candle> GetChart(string code, DateTime startDate, DateTime endDate, CandleTypes candleType)
        {
            return base.Channel.GetChart(code, startDate, endDate, candleType);
        }
    }
}