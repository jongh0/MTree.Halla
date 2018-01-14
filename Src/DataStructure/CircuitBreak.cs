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
    public class CircuitBreak : Subscribable
    {
        [BsonElement("CBT")]
        [DataMember(Name = "CBT")]
        public CircuitBreakTypes CircuitBreakType { get; set; }

        [BsonElement("IP")]
        [DataMember(Name = "IP")]
        public float InvokePrice { get; set; }
    }
}
