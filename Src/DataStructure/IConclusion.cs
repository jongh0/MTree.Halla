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
    public class IConclusion : ISubscribable
    {
        long Amount { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        DateTime ConcludedTime { get; set; }

        MarketTimeType MarketTimeType { get; set; }

        float Price { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(base.ToString());
            sb.AppendLine($"Amount: {Amount}");
            sb.AppendLine($"ConcludedTime: {ConcludedTime.ToString(Config.Default.DateTimeFormat)}");
            sb.AppendLine($"MarketTimeType: {MarketTimeType}");
            sb.AppendLine($"Price: {Price}");

            return sb.ToString();
        }
    }
}
