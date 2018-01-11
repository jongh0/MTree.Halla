using System;
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
            if (State != CommunicationState.Opened) return;
            base.Channel.NotifyMessage(type, message);
        }

        public void RegisterConsumerContract(Guid clientId, SubscribeContract subscription)
        {
            if (State != CommunicationState.Opened) return;
            base.Channel.RegisterConsumerContract(clientId, subscription);
        }

        public void UnregisterConsumerContractAll(Guid clientId)
        {
            if (State != CommunicationState.Opened) return;
            base.Channel.UnregisterConsumerContractAll(clientId);
        }

        public void UnregisterConsumerContract(Guid clientId, SubscribeTypes type)
        {
            if (State != CommunicationState.Opened) return;
            base.Channel.UnregisterConsumerContract(clientId, type);
        }

        public List<Candle> GetChart(string code, DateTime startDate, DateTime endDate, CandleTypes candleType)
        {
            if (State != CommunicationState.Opened) return null;
            return base.Channel.GetChart(code, startDate, endDate, candleType);
        }
    }
}
