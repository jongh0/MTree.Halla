using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.DataStructure
{
    public enum ChartTypes
    {
        Tick,
        Min,
        Day,
        Week,
        Month,
        Year,
    }

    [BsonDiscriminator(RootClass = true)]
    [Serializable]
    public class Chart
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("C")]
        public string Code { get; set; }

        [BsonElement("CT")]
        public ChartTypes ChartType { get; set; }

        [BsonElement("IA")]
        public bool IsAdjusted { get; set; }

        [BsonElement("Cs")]
        public Dictionary<DateTime, Candle> Candles { get; set; }
    }
}
