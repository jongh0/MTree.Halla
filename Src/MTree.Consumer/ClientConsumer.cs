using MTree.RealTimeProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MTree.Consumer
{
    public class ClientConsumer : BaseConsumer
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        protected Guid ClientId { get; set; } = Guid.Empty;

        protected InstanceContext CallbackInstance { get; set; }
        protected RealTimeConsumerClient ServiceClient { get; set; }

        public ClientConsumer() : base()
        {
            try
            {
                CallbackInstance = new InstanceContext(this);
                OpenService();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        protected void OpenService()
        {
            try
            {
                ServiceClient = new RealTimeConsumerClient(CallbackInstance, "RealTimeConsumerConfig");
                ServiceClient.Open();

                ClientId = ServiceClient.RegisterConsumer();

                logger.Info("ServiceClient opened");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        protected void CloseService()
        {
            try
            {
                if (ServiceClient != null)
                {
                    ServiceClient.UnregisterConsumer(ClientId);
                    ServiceClient.Close();
                    logger.Info("ServiceClient closed");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
    }
}
