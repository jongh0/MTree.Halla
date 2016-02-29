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
    public class CircuitBreak : ISubscribable
    {
        public CircuitBreakType CircuitBreakState { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime EventTime { get; set; }

        public float BasePrice { get; set; }

        public float InvokePrice { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(base.ToString());
            sb.AppendLine($"CircuitBreakState: {CircuitBreakState}");
            sb.AppendLine($"EventTime: {EventTime.ToString(Config.Default.DateTimeFormat)}");
            sb.AppendLine($"BasePrice: {BasePrice}");
            sb.AppendLine($"InvokePrice: {InvokePrice}");

            return sb.ToString();
        }
    }
}
