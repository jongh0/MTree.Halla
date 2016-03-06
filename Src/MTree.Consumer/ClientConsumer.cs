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

        protected InstanceContext context;
        protected RealTimeProviderClient client;

        public ClientConsumer() : base()
        {
            try
            {
                context = new InstanceContext(this);
                client = new RealTimeProviderClient(context, "RealTimeProviderConfiguration");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
    }
}
