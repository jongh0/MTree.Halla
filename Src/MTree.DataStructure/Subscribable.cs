using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.DataStructure
{
    public enum MasteringStateType
    {
        Ready,
        Running,
        Finished,
    }

    public enum MarketType
    {
        Unknown,
        INDEX,
        KOSPI,
        KOSDAQ,
        ETF,
        ETN,
        ELW,
    }

    [BsonDiscriminator(RootClass = true)]
    [BsonKnownTypes(typeof(BiddingPrice), 
                    typeof(CircuitBreak), 
                    typeof(IndexConclusion), 
                    typeof(StockConclusion), 
                    typeof(StockMaster),
                    typeof(IndexMaster))]
    [Serializable]
    public class Subscribable
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("C")]
        public string Code { get; set; }

        [BsonElement("M")]
        public MarketType Market { get; set; } = MarketType.Unknown;

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Time { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{nameof(Id)}: {Id}");
            sb.AppendLine($"{nameof(Code)}: {Code}");
            sb.AppendLine($"{nameof(Market)}: {Market}");
            sb.AppendLine($"{nameof(Time)}: {Time}");

            return sb.ToString();
        }
    }
}
