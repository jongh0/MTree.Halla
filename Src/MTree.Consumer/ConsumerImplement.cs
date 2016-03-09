using MTree.RealTimeProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MTree.Consumer
{
    public class ConsumerImplement : ConsumerBase
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        protected Guid ClientId { get; set; } = Guid.NewGuid();

        protected InstanceContext CallbackInstance { get; set; }
        protected ConsumerClient ServiceClient { get; set; }

        public ConsumerImplement() : base()
        {
            try
            {
                CallbackInstance = new InstanceContext(this);
                OpenChannel();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        protected void OpenChannel()
        {
            try
            {
                logger.Error($"Open {ProcessName} channel");

                ServiceClient = new ConsumerClient(CallbackInstance, "RealTimeConsumerConfig");
                ServiceClient.InnerChannel.Opened += InnerChannel_Opened;
                ServiceClient.InnerChannel.Closed += InnerChannel_Closed;
                ServiceClient.Open();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private void InnerChannel_Closed(object sender, EventArgs e)
        {
            logger.Error($"{ProcessName} channel closed");
        }

        private void InnerChannel_Opened(object sender, EventArgs e)
        {
            logger.Error($"{ProcessName} channel opened");
        }

        protected void CloseChannel()
        {
            try
            {
                if (ServiceClient != null)
                {
                    logger.Error($"Close {ProcessName} channel");

                    ServiceClient.UnregisterSubscribeContractAll(ClientId);
                    ServiceClient.Close();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
    }
}
