using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataStructure
{
    [DataContract]
    [ProtoContract]
    public class Candle : Subscribable
    {
        [BsonElement("CT")]
        [DataMember(Name = "CT")]
        [ProtoMember(1)]
        public CandleTypes CandleType { get; set; }

        [BsonElement("Op")]
        [DataMember(Name = "Op")]
        [ProtoMember(2)]
        public float Open { get; set; }

        [BsonElement("Cl")]
        [DataMember(Name = "Cl")]
        [ProtoMember(3)]
        public float Close { get; set; }

        [BsonElement("Lo")]
        [DataMember(Name = "Lo")]
        [ProtoMember(4)]
        public float Low { get; set; }

        [BsonElement("Hi")]
        [DataMember(Name = "Hi")]
        [ProtoMember(5)]
        public float High { get; set; }

        [BsonElement("Va")]
        [DataMember(Name = "Va")]
        [ProtoMember(6)]
        public ulong Value { get; set; }

        [BsonElement("Vo")]
        [DataMember(Name = "Vo")]
        [ProtoMember(7)]
        public ulong Volume { get; set; }

        public Candle()
        {
        }

        public Candle(string code)
        {
            Code = code;
        }

        public override string ToString(params string[] excludeProperties)
        {
            return ToString(typeof(Candle), excludeProperties);
        }

        public static char ConvertToCharType(CandleTypes type)
        {
            switch (type)
            {
                case CandleTypes.Min: return 'm';
                case CandleTypes.Day: return 'D';
                case CandleTypes.Week: return 'W';
                case CandleTypes.Month: return 'M';
                default: return 'T';
            }
        }

        public static CandleTypes ConvertToCandleType(char type)
        {
            switch (type)
            {
                case 'm': return CandleTypes.Min;
                case 'D': return CandleTypes.Day;
                case 'W': return CandleTypes.Week;
                case 'M': return CandleTypes.Month;
                default: return CandleTypes.Tick;
            }
        }
    }
}
