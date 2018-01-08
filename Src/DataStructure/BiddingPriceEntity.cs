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
    public class BiddingPriceEntity
    {
        [BsonElement("I")]
        [DataMember(Name = "I")]
        [ProtoMember(1)]
        public int Index { get; set; }

        [BsonElement("P")]
        [DataMember(Name = "P")]
        [ProtoMember(2)]
        public float Price { get; set; }

        [BsonElement("A")]
        [DataMember(Name = "A")]
        [ProtoMember(3)]
        public long Amount { get; set; }

        public BiddingPriceEntity()
        {
        }

        public BiddingPriceEntity(int index, float price, long amount)
        {
            Index = index;
            Price = price;
            Amount = amount;
        }

        public override string ToString()
        {
            return $"{Index}/{Price}/{Amount}";
        }
    }
}
