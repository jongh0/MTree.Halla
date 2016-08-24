using MongoDB.Bson;
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
        Year,
    }

    [Serializable]
    public class Candle : Subscribable
    {
        [BsonElement("CT")]
        public CandleTypes CandleType { get; set; }

        [BsonElement("Op")]
        public float Open { get; set; }

        [BsonElement("Cl")]
        public float Close { get; set; }

        [BsonElement("Lo")]
        public float Low { get; set; }

        [BsonElement("Hi")]
        public float High { get; set; }

        [BsonElement("Va")]
        public ulong Value { get; set; }

        [BsonElement("Vo")]
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
