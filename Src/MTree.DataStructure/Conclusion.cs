using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.DataStructure
{
    #region enum
    public enum MarketTimeTypes
    {
        Unknown,
        Normal,
        NormalExpect,
        BeforeOffTheClock,
        BeforeExpect,
        AfterOffTheClock,
        AfterExpect,
    } 
    #endregion

    [Serializable]
    public class Conclusion : Subscribable
    {
        [BsonElement("A")]
        public long Amount { get; set; }

        [BsonElement("MTT")]
        public MarketTimeTypes MarketTimeType { get; set; }

        [BsonElement("P")]
        public float Price { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                foreach (var property in typeof(Conclusion).GetProperties())
                {
                    if (property.Name != "Id")
                        sb.Append($"{property.Name}: {property.GetValue(this)}, ");
                }
            }
            catch { }

            return sb.ToString();
        }
        public override string ToString(bool excludeId = true)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                if (excludeId == true)
                {
                    foreach (var property in typeof(Conclusion).GetProperties())
                    {
                        if (property.Name != "Id")
                            sb.Append($"{property.Name}: {property.GetValue(this)}, ");
                    }
                }
                else
                {
                    return ToString();
                }
            }
            catch { }

            return sb.ToString();
        }
    }
}
