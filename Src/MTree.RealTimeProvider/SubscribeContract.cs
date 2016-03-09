using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.RealTimeProvider
{
    public enum SubscribeType
    {
        BiddingPrice,
        StockConclusion,
        IndexConclusion,
    }

    public enum SubscribeWay
    {
        All,
        Partial,
    }

    [Serializable]
    public class SubscribeContract
    {
        public SubscribeType Type { get; set; }

        public SubscribeWay Way { get; set; } = SubscribeWay.All;

        public HashSet<string> Codes { get; set; } = new HashSet<string>();

        public IRealTimeConsumerCallback Callback { get; set; } = null;

        public bool ContainCode(string code)
        {
            return Codes.Contains(code);
        }
    }
}
