using MTree.DataStructure;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MTree.RealTimeProvider
{
    public partial class RealTimeProvider
    {
        private ConcurrentDictionary<Guid, IRealTimeConsumerCallback> ConsumerClients { get; set; } = new ConcurrentDictionary<Guid, IRealTimeConsumerCallback>();

        public Guid RegisterConsumer()
        {
            try
            {
                var callback = OperationContext.Current.GetCallbackChannel<IRealTimeConsumerCallback>();
                var clientId = Guid.NewGuid();

                ConsumerClients.TryAdd(clientId, callback);
                logger.Info($"{clientId} consumer registered");

                return clientId;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            logger.Error($"Consumer register failed");
            return Guid.Empty;
        }

        public void UnregisterConsumer(Guid clientId)
        {
            try
            {
                if (clientId != null &&
                    clientId != Guid.Empty &&
                    ConsumerClients.ContainsKey(clientId))
                {
                    IRealTimeConsumerCallback callback;
                    ConsumerClients.TryRemove(clientId, out callback);

                    logger.Info($"{clientId} consumer unregistered");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
    }
}
