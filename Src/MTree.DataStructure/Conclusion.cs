using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.DataStructure
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
        public long Amount { get; set; }

        [BsonElement("MTT")]
        public MarketTimeType MarketTimeType { get; set; }

        public float Price { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(base.ToString());
            sb.AppendLine($"{nameof(Amount)}: {Amount}");
            sb.AppendLine($"{nameof(MarketTimeType)}: {MarketTimeType}");
            sb.AppendLine($"{nameof(Price)}: {Price}");

            return sb.ToString();
        }
    }
}
