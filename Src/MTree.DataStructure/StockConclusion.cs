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
                    if (property.Name != "Id")
                    {
                        //if (property.Name == "Time")
                        //    sb.Append($"{property.Name}: {((DateTime)property.GetValue(this)).Year}{((DateTime)property.GetValue(this)).Month}{((DateTime)property.GetValue(this)).Day} {((DateTime)property.GetValue(this)).Hour}:{((DateTime)property.GetValue(this)).Minute}:{((DateTime)property.GetValue(this)).Second}.{((DateTime)property.GetValue(this)).Millisecond}, ");
                        //else
                            sb.Append($"{property.Name}: {property.GetValue(this)}, ");
                    }
                    else
                    {
                        //sb.Append($"{property.Name}: {((ObjectId)property.GetValue(this))}, ");
                        //sb.Append($"TimeStamp: {((ObjectId)property.GetValue(this)).Timestamp}, ");
                    }
                }
            }
            catch { }

            return sb.ToString();
        }
    }
}
