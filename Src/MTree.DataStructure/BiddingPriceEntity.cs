using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace MTree.DataStructure
{
    [DataContract]
    public class BiddingPriceEntity
    {
        [BsonElement("I")]
        [DataMember(Name = "I")]
        public int Index { get; set; }

        [BsonElement("P")]
        [DataMember(Name = "P")]
        public float Price { get; set; }

        [BsonElement("A")]
        [DataMember(Name = "A")]
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
