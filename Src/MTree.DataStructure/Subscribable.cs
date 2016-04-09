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
    public enum MasteringStates
    {
        Ready,
        Running,
        Finished,
    }

    public enum MarketTypes
    {
        Unknown,
        INDEX,
        KOSPI,
        KOSDAQ,
        KONEX,
        ETF,
        ETN,
        ELW,
        FREEBOARD
    }

    [BsonDiscriminator(RootClass = true)]
    [BsonKnownTypes(typeof(Candle), 
                    typeof(BiddingPrice), 
                    typeof(CircuitBreak),
                    typeof(IndexConclusion), 
                    typeof(StockConclusion), 
                    typeof(StockMaster),
                    typeof(IndexMaster))]
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [ProtoInclude(10, typeof(StockConclusion))]
    [Serializable]
    public class Subscribable
    {
#if true
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
#else
        [BsonId]
        public ObjectId Id { get; set; }
#endif

        [BsonElement("C")]
        public string Code { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        [BsonElement("T")]
        public DateTime Time { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                foreach (var property in typeof(Subscribable).GetProperties())
                {
                    sb.Append($"{property.Name}: {property.GetValue(this)}, ");
                }
            }
            catch { }

            return sb.ToString();
        }
    }
}
