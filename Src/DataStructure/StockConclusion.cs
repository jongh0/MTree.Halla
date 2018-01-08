using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataStructure
{
    [DataContract]
    [ProtoContract]
    public class StockConclusion : Conclusion
    {
        [BsonElement("CT")]
        [DataMember(Name = "CT")]
        [ProtoMember(1)]
        public ConclusionTypes ConclusionType { get; set; }

        public override string ToString(params string[] excludeProperties)
        {
            return ToString(typeof(StockConclusion), excludeProperties);
        }
    }
}
