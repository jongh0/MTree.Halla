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
                ServiceClient = new ConsumerClient(CallbackInstance, "RealTimeConsumerConfig");
                ServiceClient.Open();

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
                    ServiceClient.RequestUnsubscription(ClientId);
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
