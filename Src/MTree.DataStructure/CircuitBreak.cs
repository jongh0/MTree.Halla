using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.DataStructure
{
    #region enum
    public enum CircuitBreakType
    {
        Clear,
        StaticInvoke,
        DynamicInvoke,
    } 
    #endregion

    [Serializable]
    public class CircuitBreak : Subscribable
    {
        [BsonElement("CBS")]
        public CircuitBreakType CircuitBreakState { get; set; }

        [BsonElement("BP")]
        public float BasePrice { get; set; }

        [BsonElement("IP")]
        public float InvokePrice { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(base.ToString());
            sb.AppendLine($"{nameof(CircuitBreakState)}: {CircuitBreakState}");
            sb.AppendLine($"{nameof(BasePrice)}: {BasePrice}");
            sb.AppendLine($"{nameof(InvokePrice)}: {InvokePrice}");

            return sb.ToString();
        }
    }
}
