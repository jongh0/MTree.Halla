using MongoDB.Bson;
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
    public class StockConclusion : Conclusion
    {
        [BsonElement("CT")]
        [DataMember(Name = "CT")]
        public ConclusionTypes ConclusionType { get; set; }
    }
}
