using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.DataStructure
{
    [Serializable]
    public class IndexConclusion : Conclusion
    {
        [BsonElement("Va")]
        public long MarketCapitalization { get; set; }

        [BsonElement("ULIC")]
        public int UpperLimitedItemCount { get; set; }

        [BsonElement("IIC")]
        public int IncreasingItemCount { get; set; }

        [BsonElement("SIC")]
        public int SteadyItemCount { get; set; }

        [BsonElement("DIC")]
        public int DecreasingItemCount { get; set; }

        [BsonElement("LLIC")]
        public int LowerLimitedItemCount { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(base.ToString());
            sb.AppendLine($"{nameof(MarketCapitalization)}: {MarketCapitalization}");
            sb.AppendLine($"{nameof(UpperLimitedItemCount)}: {UpperLimitedItemCount}");
            sb.AppendLine($"{nameof(IncreasingItemCount)}: {IncreasingItemCount}");
            sb.AppendLine($"{nameof(SteadyItemCount)}: {SteadyItemCount}");
            sb.AppendLine($"{nameof(DecreasingItemCount)}: {DecreasingItemCount}");
            sb.AppendLine($"{nameof(LowerLimitedItemCount)}: {LowerLimitedItemCount}");

            return sb.ToString();
        }
    }
}
