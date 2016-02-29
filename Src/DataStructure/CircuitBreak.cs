using Configuration;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStructure
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
        [BsonElement("CBSt")]
        public CircuitBreakType CircuitBreakState { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        [BsonElement("ETi")]
        public DateTime EventTime { get; set; }

        [BsonElement("BPr")]
        public float BasePrice { get; set; }

        [BsonElement("IPr")]
        public float InvokePrice { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(base.ToString());
            sb.AppendLine($"{nameof(CircuitBreakState)}: {CircuitBreakState}");
            sb.AppendLine($"{nameof(EventTime)}: {EventTime}");
            sb.AppendLine($"{nameof(BasePrice)}: {BasePrice}");
            sb.AppendLine($"{nameof(InvokePrice)}: {InvokePrice}");

            return sb.ToString();
        }
    }
}
