using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataStructure
{
    [DataContract]
    public abstract class Conclusion : Subscribable
    {
        [BsonElement("A")]
        [DataMember(Name = "A")]
        public long Amount { get; set; }

        [BsonElement("MTT")]
        [DataMember(Name = "MTT")]
        public MarketTimeTypes MarketTimeType { get; set; }

        [BsonElement("P")]
        [DataMember(Name = "P")]
        public float Price { get; set; }
    }
}
