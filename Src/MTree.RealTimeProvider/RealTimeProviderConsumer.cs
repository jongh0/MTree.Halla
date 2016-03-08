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
        private ConcurrentDictionary<Guid, Subscription> BiddingPriceSubscriptions { get; set; } = new ConcurrentDictionary<Guid, Subscription>();
        private ConcurrentDictionary<Guid, Subscription> StockConclusionSubscriptions { get; set; } = new ConcurrentDictionary<Guid, Subscription>();
        private ConcurrentDictionary<Guid, Subscription> IndexConclusionSubscriptions { get; set; } = new ConcurrentDictionary<Guid, Subscription>();
        
        public void RequestSubscription(Guid clientId, Subscription subscription)
        {
            try
            {
                var subscriptionList = GetSubscriptionList(subscription.Type);
                if (subscriptionList == null) return;

                if (subscriptionList.ContainsKey(clientId) == true)
                {
                    Subscription currSubscription;
                    if (subscriptionList.TryGetValue(clientId, out currSubscription) == true)
                    {
                        if (subscriptionList.TryUpdate(clientId, subscription, currSubscription) == true)
                            logger.Info($"{clientId} / {subscription.Type} subscription updated");
                    }
                }
                else
                {
                    subscription.Callback = OperationContext.Current.GetCallbackChannel<IRealTimeConsumerCallback>();

                    if (subscriptionList.TryAdd(clientId, subscription) == true)
                        logger.Info($"{clientId} / {subscription.Type} subscription added");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public void RequestUnsubscription(Guid clientId)
        {
            RequestUnsubscription(clientId, SubscriptionType.BiddingPrice);
            RequestUnsubscription(clientId, SubscriptionType.StockConclusion);
            RequestUnsubscription(clientId, SubscriptionType.IndexConclusion);
        }

        public void RequestUnsubscription(Guid clientId, SubscriptionType type)
        {
            try
            {
                var subscriptionList = GetSubscriptionList(type);
                if (subscriptionList == null) return;

                if (subscriptionList.ContainsKey(clientId) == true)
                {
                    Subscription temp;
                    if (subscriptionList.TryRemove(clientId, out temp) == true)
                    {
                        temp.Callback = null;
                        logger.Info($"{clientId} / {type} subscription removed");
                    }
                }
                else
                {
                    logger.Info($"{clientId} / {type} subscription not exist");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private ConcurrentDictionary<Guid, Subscription> GetSubscriptionList(SubscriptionType type)
        {
            switch (type)
            {
                case SubscriptionType.BiddingPrice:
                    return BiddingPriceSubscriptions;
                case SubscriptionType.StockConclusion:
                    return StockConclusionSubscriptions;
                case SubscriptionType.IndexConclusion:
                    return IndexConclusionSubscriptions;
                default:
                    return null;
            }
        }
    }
}
