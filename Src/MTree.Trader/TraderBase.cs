using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MTree.Trader
{
    public class TraderBase : TraderCallback
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        protected Guid ClientId { get; } = Guid.NewGuid();

        protected InstanceContext CallbackInstance { get; set; }
        protected TraderClient ServiceClient { get; set; }

        public TraderBase(string endpointConfigurationName)
        {
            try
            {
                CallbackInstance = new InstanceContext(this);
                OpenChannel(endpointConfigurationName);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        protected void OpenChannel(string endpointConfigurationName)
        {
            try
            {
                logger.Info($"[{GetType().Name}] Open channel");

                ServiceClient = new TraderClient(CallbackInstance, endpointConfigurationName);
                ServiceClient.InnerChannel.Opened += ServiceClient_Opened;
                ServiceClient.InnerChannel.Closed += ServiceClient_Closed;
                ServiceClient.Open();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        protected void CloseChannel()
        {
            try
            {
                if (ServiceClient != null)
                {
                    logger.Info($"[{GetType().Name}] Close channel");
                    ServiceClient.Close();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        protected virtual void ServiceClient_Opened(object sender, EventArgs e)
        {
            logger.Info($"[{GetType().Name}] Channel opened");
        }

        protected virtual void ServiceClient_Closed(object sender, EventArgs e)
        {
            logger.Info($"[{GetType().Name}] Channel closed");
        }
    }
}
