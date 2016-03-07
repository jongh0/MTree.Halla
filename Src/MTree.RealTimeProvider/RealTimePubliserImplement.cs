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
        private ConcurrentDictionary<Guid, IRealTimePublisherCallback> PublisherClients { get; set; } = new ConcurrentDictionary<Guid, IRealTimePublisherCallback>();

        public Guid RegisterPublisher()
        {
            try
            {
                var callback = OperationContext.Current.GetCallbackChannel<IRealTimePublisherCallback>();
                var clientId = Guid.NewGuid();

                PublisherClients.TryAdd(clientId, callback);
                logger.Info($"{clientId} publisher registered");

                return clientId;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            logger.Error($"Publisher register failed");
            return Guid.Empty;
        }

        public void UnregisterPublisher(Guid clientId)
        {
            try
            {
                if (clientId != null && 
                    clientId != Guid.Empty &&
                    PublisherClients.ContainsKey(clientId))
                {
                    IRealTimePublisherCallback callback;
                    PublisherClients.TryRemove(clientId, out callback);

                    logger.Info($"{clientId} publisher unregistered");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public void PublishBiddingPrice(BiddingPrice biddingPrice)
        {
            throw new NotImplementedException();
        }

        public void PublishCircuitBreak(CircuitBreak circuitBreak)
        {
            throw new NotImplementedException();
        }

        public void PublishIndexConclusion(IndexConclusion conclusion)
        {
            throw new NotImplementedException();
        }

        public void PublishStockConclusion(StockConclusion conclusion)
        {
            throw new NotImplementedException();
        }
    }
}
