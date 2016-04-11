using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.RealTimeProvider
{
    public enum SubscribeTypes
    {
        Chart,
        Mastering,
        BiddingPrice,
        CircuitBreak,
        StockConclusion,
        IndexConclusion,
    }

    public enum SubscribeScopes
    {
        All,
        Partial,
    }

    [Serializable]
    public class SubscribeContract
    {
        public SubscribeTypes Type { get; set; }

        public SubscribeScopes Scope { get; set; } = SubscribeScopes.All;

        public HashSet<string> Codes { get; set; } = new HashSet<string>();

        public IRealTimeConsumerCallback Callback { get; set; } = null;

        public SubscribeContract()
        {
        }

        public SubscribeContract(SubscribeTypes type)
        {
            Type = type;
        }

        public SubscribeContract(SubscribeTypes type, SubscribeScopes scope, List<string> codes)
        {
            Type = type;
            Scope = scope;

            if (scope == SubscribeScopes.Partial && codes != null)
            {
                foreach (var code in codes)
                {
                    Codes.Add(code);
                }
            }
        }

        public override string ToString()
        {
            return $"{Type}/{Scope}";
        }

        public bool ContainCode(string code)
        {
            return Codes.Contains(code);
        }
    }
}
