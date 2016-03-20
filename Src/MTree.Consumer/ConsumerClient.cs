using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel.Channels;
using System.ServiceModel;
using MTree.DataStructure;
using MTree.RealTimeProvider;

namespace MTree.Consumer
{
    public partial class ConsumerClient : DuplexClientBase<IRealTimeConsumer>, IRealTimeConsumer
    {
        public ConsumerClient(InstanceContext callbackInstance, string endpointConfigurationName) : base(callbackInstance, endpointConfigurationName) { }

        public void SendMessage(MessageTypes type, string message)
        {
            base.Channel.SendMessage(type, message);
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
    }
}
