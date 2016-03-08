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

        public void RegisterPublisher(Guid clientId)
        {
            try
            {
                if (PublisherClients.ContainsKey(clientId) == true)
                {
                    logger.Info($"{clientId} publisher already exist");
                }
                else
                {
                    var callback = OperationContext.Current.GetCallbackChannel<IRealTimePublisherCallback>();
                    PublisherClients.TryAdd(clientId, callback);

                    logger.Info($"{clientId} publisher registered");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public void UnregisterPublisher(Guid clientId)
        {
            try
            {
                if (PublisherClients.ContainsKey(clientId) == true)
                {
                    IRealTimePublisherCallback temp;
                    PublisherClients.TryRemove(clientId, out temp);

                    logger.Info($"{clientId} publisher unregistered");
                }
                else
                {
                    logger.Warn($"{clientId} publisher not exist");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public void PublishBiddingPrice(BiddingPrice biddingPrice)
        {
            BiddingPriceQueue.Enqueue(biddingPrice);
        }

        public void PublishCircuitBreak(CircuitBreak circuitBreak)
        {
        }

        public void PublishIndexConclusion(IndexConclusion conclusion)
        {
            IndexConclusionQueue.Enqueue(conclusion);
        }

        public void PublishStockConclusion(StockConclusion conclusion)
        {
            StockConclusionQueue.Enqueue(conclusion);
        }
    }
}
