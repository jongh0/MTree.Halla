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
    public class IndexConclusion : Conclusion
    {
        [BsonElement("MC")]
        [DataMember(Name = "MC")]
        public long MarketCapitalization { get; set; }
    }
}
