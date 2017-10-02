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
    public class CodeMapDbObject : Subscribable
    {
        [BsonElement("CM")]
        [DataMember(Name = "CM")]
        public string CodeMap { get; set; }
    }
}
