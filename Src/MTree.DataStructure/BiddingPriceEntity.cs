using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.DataStructure
{
    [Serializable]
    public class BiddingPriceEntity
    {
        [BsonElement("I")]
        public int Index { get; set; }

        [BsonElement("P")]
        public float Price { get; set; }

        [BsonElement("A")]
        public long Amount { get; set; }

        public override string ToString()
        {
            return $"{Index}/{Price}/{Amount}";
        }
    }
}
