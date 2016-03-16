using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.DataStructure
{
    public enum CandleTypes
    {
        A,
        B,
    }

    [Serializable]
    public class Candle
    {
        [BsonElement("CT")]
        public CandleTypes CandleType { get; set; }

        [BsonElement("O")]
        public float Open { get; set; }

        [BsonElement("C")]
        public float Close { get; set; }

        [BsonElement("L")]
        public float Low { get; set; }

        [BsonElement("H")]
        public float High { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        [BsonElement("ST")]
        public DateTime StartTime { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        [BsonElement("ET")]
        public DateTime EndTime { get; set; }

        [BsonElement("TVa")]
        public long TradeValue { get; set; }

        [BsonElement("TVo")]
        public long TradeVolume { get; set; }
    }
}
