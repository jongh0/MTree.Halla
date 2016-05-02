using MongoDB.Bson;
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
        Unknown,
        Sell,
        Buy,
    } 
    #endregion

    [Serializable]
    public class StockConclusion : Conclusion
    {
        [BsonElement("CT")]
        public ConclusionTypes ConclusionType { get; set; }

        public override string ToString(params string[] excludeProperties)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                foreach (var property in typeof(StockConclusion).GetProperties())
                {
                    if (excludeProperties.Contains(property.Name) == false)
                    {
                        if (property.PropertyType.Name == "DateTime")
                        {
                            sb.Append($"{property.Name}: {ReceivedTime.Hour}:{ReceivedTime.Minute}:{ReceivedTime.Second}:{ReceivedTime.Millisecond}, ");
                        }
                        else
                        {
                            sb.Append($"{property.Name}: {property.GetValue(this)}, ");
                        }
                    }
                }
            }
            catch { }

            return sb.ToString();
        }
    }
}
