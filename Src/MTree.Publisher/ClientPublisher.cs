using MTree.RealTimeProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MTree.Publisher
{
    public class ClientPublisher : BasePublisher
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        protected InstanceContext CallbackInstance { get; set; }
        protected RealTimePublisherClient ServiceClient { get; set; }

        public ClientPublisher() : base()
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
                ServiceClient = new RealTimePublisherClient(CallbackInstance, "RealTimePublisherConfig");
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
                ServiceClient?.Close();
                logger.Info("ServiceClient closed");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
    }
}
