using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStructure
{
    public class TradeConclusion : Subscribable
    {
        [BsonElement("AN")]
        public string AccountNumber { get; set; }
    }
}
