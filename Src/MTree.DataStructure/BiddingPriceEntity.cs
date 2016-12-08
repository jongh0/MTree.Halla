using MongoDB.Bson.Serialization.Attributes;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.DataStructure
{
    [Serializable]
    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    public class BiddingPriceEntity
    {
        [BsonElement("I")]
        public int Index { get; set; }

        [BsonElement("P")]
        public float Price { get; set; }

        [BsonElement("A")]
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
