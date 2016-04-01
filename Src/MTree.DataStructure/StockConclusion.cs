using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.DataStructure
{
    #region enum
    public enum ConclusionTypes
    {
        Sell,
        Buy,
    } 
    #endregion

    [Serializable]
    public class StockConclusion : Conclusion
    {
        [BsonElement("CT")]
        public ConclusionTypes ConclusionType { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                foreach (var property in typeof(StockConclusion).GetProperties())
                {
                    sb.AppendLine($"{property.Name}: {property.GetValue(this)}");
                }
            }
            catch { }

            return sb.ToString();
        }
    }
}
