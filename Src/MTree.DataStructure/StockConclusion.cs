using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
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

    [DataContract]
    public class StockConclusion : Conclusion
    {
        [BsonElement("CT")]
        [DataMember(Name = "CT")]
        public ConclusionTypes ConclusionType { get; set; }

        public override string ToString(params string[] excludeProperties)
        {
            return ToString(typeof(StockConclusion), excludeProperties);
        }
    }
}
