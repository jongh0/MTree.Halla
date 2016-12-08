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
    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    [ProtoInclude(10, typeof(StockConclusion))]
    [ProtoInclude(11, typeof(IndexConclusion))]
    public class Conclusion : Subscribable
    {
        [BsonElement("A")]
        public long Amount { get; set; }

        [BsonElement("MTT")]
        public MarketTimeTypes MarketTimeType { get; set; }

        [BsonElement("P")]
        public float Price { get; set; }

        public override string ToString(params string[] excludeProperties)
        {
            return ToString(typeof(Conclusion), excludeProperties);
        }
    }
}
