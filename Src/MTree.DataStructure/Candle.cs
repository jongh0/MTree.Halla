﻿using MongoDB.Bson;
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
        Tick,
        Min,
        Day,
        Week,
        Month,
    }

    [BsonDiscriminator(RootClass = true)]
    [Serializable]
    public class Candle
    {
        [BsonId]
        public ObjectId Id { get; set; }

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
        [BsonElement("T")]
        public DateTime Time { get; set; }

        [BsonElement("Va")]
        public ulong Value { get; set; }

        [BsonElement("Vo")]
        public ulong Volume { get; set; }
    }
}
