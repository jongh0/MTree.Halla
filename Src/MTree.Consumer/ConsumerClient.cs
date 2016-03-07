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

        public void KeepConnection()
        {
            base.Channel.KeepConnection();
        }

        public Guid RegisterConsumer()
        {
            return base.Channel.RegisterConsumer();
        }

        public void UnregisterConsumer(Guid clientId)
        {
            base.Channel.UnregisterConsumer(clientId);
        }
    }
}
