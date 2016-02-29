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
    public enum MarketTimeType
    {
        BeforeOffTheClock,
        BeforeExpect,
        Normal,
        AfterExpect,
        AfterOffTheClock,
    } 
    #endregion

    [Serializable]
    public class Conclusion : Subscribable
    {
        [BsonElement("Amt")]
        long Amount { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        [BsonElement("CTi")]
        DateTime ConcludedTime { get; set; }

        [BsonElement("MTTy")]
        MarketTimeType MarketTimeType { get; set; }

        [BsonElement("Prc")]
        float Price { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(base.ToString());
            sb.AppendLine($"{nameof(Amount)}: {Amount}");
            sb.AppendLine($"{nameof(ConcludedTime)}: {ConcludedTime}");
            sb.AppendLine($"{nameof(MarketTimeType)}: {MarketTimeType}");
            sb.AppendLine($"{nameof(Price)}: {Price}");

            return sb.ToString();
        }
    }
}
