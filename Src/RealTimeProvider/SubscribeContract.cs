using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeProvider
{
    [DataContract]
    [ProtoContract]
    public class SubscribeContract
    {
        [DataMember]
        [ProtoMember(1)]
        public SubscribeTypes Type { get; set; }

        [DataMember]
        [ProtoMember(2)]
        public SubscribeScopes Scope { get; set; } = SubscribeScopes.All;

        [DataMember]
        [ProtoMember(3)]
        public HashSet<string> Codes { get; set; } = new HashSet<string>();

        [IgnoreDataMember]
        [ProtoIgnore]
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
