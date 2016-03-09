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

        public void NoOperation()
        {
            base.Channel.NoOperation();
        }

        public void RegisterSubscribeContract(Guid clientId, SubscribeContract subscription)
        {
            base.Channel.RegisterSubscribeContract(clientId, subscription);
        }

        public void UnregisterSubscribeContractAll(Guid clientId)
        {
            base.Channel.UnregisterSubscribeContractAll(clientId);
        }

        public void UnregisterSubscribeContract(Guid clientId, SubscribeType type)
        {
            base.Channel.UnregisterSubscribeContract(clientId, type);
        }
    }
}
