using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ProtoBuf;
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

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class Base0
    {
        public string Id { get; set; }

        public string Code { get; set; }

        public DateTime Time { get; set; }

        public long Amount { get; set; }

        public MarketTimeTypes MarketTimeType { get; set; }

        public float Price { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class StockConclusionTest : Base0
    {
        public ConclusionTypes ConclusionType { get; set; }
    }

    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class StockConclusion : Subscribable
    {
        [BsonElement("A")]
        public long Amount { get; set; }

        [BsonElement("MTT")]
        public MarketTimeTypes MarketTimeType { get; set; }

        [BsonElement("P")]
        public float Price { get; set; }

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
                        sb.Append($"{property.Name}: {property.GetValue(this)}, ");
                }
            }
            catch { }

            return sb.ToString();
        }
    }
}
