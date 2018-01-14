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
    public class IndexMaster : Subscribable
    {
        [BsonElement("N")]
        [DataMember(Name = "N")]
        public string Name { get; set; }

        [BsonElement("BP")]
        [DataMember(Name = "BP")]
        public float BasisPrice { get; set; }
    }
}
