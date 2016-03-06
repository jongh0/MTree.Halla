using MTree.RealTimeProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MTree.Provider
{
    public class ClientProvider : BaseProvider
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        protected InstanceContext context;
        protected RealTimeProviderClient client;

        public ClientProvider(object implementation) : base()
        {
            try
            {
                context = new InstanceContext(implementation);
                client = new RealTimeProviderClient(context, "RealTimeProviderConfiguration");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
    }
}
