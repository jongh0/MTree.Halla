using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.DataStructure
{
    #region enum
    public enum CircuitBreakTypes
    {
        Clear,
        StaticInvoke,
        DynamicInvoke,
        StaticAndDynamicInvoke,
    } 
    #endregion

    [Serializable]
    public class CircuitBreak : Subscribable
    {
        [BsonElement("CBT")]
        public CircuitBreakTypes CircuitBreakType { get; set; }

        [BsonElement("IP")]
        public float InvokePrice { get; set; }

        public override string ToString(params string[] excludeProperties)
        {
            return ToString(typeof(CircuitBreak), excludeProperties);
        }
    }
}
