using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStructure
{
    [Serializable]
    public class BiddingPriceEntity
    {
        [BsonElement("Idx")]
        public int Index { get; set; }

        [BsonElement("Prc")]
        public float Price { get; set; }

        [BsonElement("Amt")]
        public long Amount { get; set; }

        public override string ToString()
        {
            return $"{nameof(Index)}: {Index}, {nameof(Price)}: {Price}, {nameof(Amount)}: {Amount}";
        }
    }
}
