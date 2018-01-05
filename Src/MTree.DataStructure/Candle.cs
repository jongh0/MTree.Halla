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
    [DataContract]
    public class Candle : Subscribable
    {
        [BsonElement("CT")]
        [DataMember(Name = "CT")]
        public CandleTypes CandleType { get; set; }

        [BsonElement("Op")]
        [DataMember(Name = "Op")]
        public float Open { get; set; }

        [BsonElement("Cl")]
        [DataMember(Name = "Cl")]
        public float Close { get; set; }

        [BsonElement("Lo")]
        [DataMember(Name = "Lo")]
        public float Low { get; set; }

        [BsonElement("Hi")]
        [DataMember(Name = "Hi")]
        public float High { get; set; }

        [BsonElement("Va")]
        [DataMember(Name = "Va")]
        public ulong Value { get; set; }

        [BsonElement("Vo")]
        [DataMember(Name = "Vo")]
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
