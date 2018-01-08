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
    public class IndexMaster : Subscribable
    {
        [BsonElement("N")]
        [DataMember(Name = "N")]
        [ProtoMember(1)]
        public string Name { get; set; }

        [BsonElement("BP")]
        [DataMember(Name = "BP")]
        [ProtoMember(2)]
        public float BasisPrice { get; set; }

        public override string ToString(params string[] excludeProperties)
        {
            return ToString(typeof(IndexMaster), excludeProperties);
        }
    }
}
