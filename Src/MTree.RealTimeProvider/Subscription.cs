using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.RealTimeProvider
{
    public enum SubscriptionType
    {
        BiddingPrice,
        StockConclusion,
        IndexConclusion,
    }

    public enum SubscriptionWay
    {
        All,
        Partial,
    }

    [Serializable]
    public class Subscription
    {
        public SubscriptionType Type { get; set; }

        public SubscriptionWay Way { get; set; } = SubscriptionWay.All;

        public HashSet<string> Codes { get; set; } = new HashSet<string>();

        public IRealTimeConsumerCallback Callback { get; set; } = null;

        public bool ContainCode(string code)
        {
            return Codes.Contains(code);
        }
    }
}
