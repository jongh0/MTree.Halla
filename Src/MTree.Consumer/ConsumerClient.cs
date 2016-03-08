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

        public void RequestSubscription(Guid clientId, Subscription subscription)
        {
            base.Channel.RequestSubscription(clientId, subscription);
        }

        public void RequestUnsubscriptionAll(Guid clientId)
        {
            base.Channel.RequestUnsubscriptionAll(clientId);
        }

        public void RequestUnsubscription(Guid clientId, SubscriptionType type)
        {
            base.Channel.RequestUnsubscription(clientId, type);
        }
    }
}
