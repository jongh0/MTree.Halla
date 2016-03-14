﻿using MongoDB.Bson.Serialization.Attributes;
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
        [BsonElement("I")]
        public double Index { get; set; }

        [BsonElement("Vo")]
        public double Volume { get; set; }

        [BsonElement("Va")]
        public double Value { get; set; }

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
            sb.AppendLine($"{nameof(Index)}: {Index}");
            sb.AppendLine($"{nameof(Volume)}: {Volume}");
            sb.AppendLine($"{nameof(Value)}: {Value}");
            sb.AppendLine($"{nameof(UpperLimitedItemCount)}: {UpperLimitedItemCount}");
            sb.AppendLine($"{nameof(IncreasingItemCount)}: {IncreasingItemCount}");
            sb.AppendLine($"{nameof(SteadyItemCount)}: {SteadyItemCount}");
            sb.AppendLine($"{nameof(DecreasingItemCount)}: {DecreasingItemCount}");
            sb.AppendLine($"{nameof(LowerLimitedItemCount)}: {LowerLimitedItemCount}");

            return sb.ToString();
        }
    }
}
