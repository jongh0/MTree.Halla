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
        [BsonElement("MC")]
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
            try
            {
                foreach (var property in typeof(IndexConclusion).GetProperties())
                {
                    sb.AppendLine($"{property.Name}: {property.GetValue(this)}");
                }
            }
            catch { }

            return sb.ToString();
        }
    }
}
