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
                CallbackInstance.Opened += CallbackInstance_Opened;
                CallbackInstance.Closed += CallbackInstance_Closed;
                CallbackInstance.Faulted += CallbackInstance_Faulted;

                OpenService();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private void CallbackInstance_Faulted(object sender, EventArgs e)
        {
            logger.Error("Service faulted");
        }

        private void CallbackInstance_Closed(object sender, EventArgs e)
        {
            logger.Info("Service closed");
        }

        private void CallbackInstance_Opened(object sender, EventArgs e)
        {
            logger.Info("Service opened");
        }

        protected void OpenService()
        {
            try
            {
                logger.Info("Open service");

                ServiceClient = new ConsumerClient(CallbackInstance, "RealTimeConsumerConfig");
                ServiceClient.Open();
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
                    logger.Info("Close service");

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
