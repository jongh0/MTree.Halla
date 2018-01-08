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
    [ProtoInclude(10, typeof(StockConclusion))]
    [ProtoInclude(11, typeof(IndexConclusion))]
    public class Conclusion : Subscribable
    {
        [BsonElement("A")]
        [DataMember(Name = "A")]
        [ProtoMember(1)]
        public long Amount { get; set; }

        [BsonElement("MTT")]
        [DataMember(Name = "MTT")]
        [ProtoMember(2)]
        public MarketTimeTypes MarketTimeType { get; set; }

        [BsonElement("P")]
        [DataMember(Name = "P")]
        [ProtoMember(3)]
        public float Price { get; set; }

        public override string ToString(params string[] excludeProperties)
        {
            return ToString(typeof(Conclusion), excludeProperties);
        }
    }
}
