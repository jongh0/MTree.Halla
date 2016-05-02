using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
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
    [Serializable]
    public class Subscribable
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("C")]
        public string Code { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        [BsonElement("T")]
        public DateTime Time { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        [BsonElement("RT")]
        public DateTime ReceivedTime { get; set; }

        public override string ToString()
        {
            return ToString(string.Empty);
        }

        public virtual string ToString(params string[] excludeProperties)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                foreach (var property in typeof(Subscribable).GetProperties())
                {
<<<<<<< HEAD
                    if (property.Name != "Id" && property.Name != "ReceivedTime")
=======
                    if (excludeProperties.Contains(property.Name) == false)
>>>>>>> origin/master
                        sb.Append($"{property.Name}: {property.GetValue(this)}, ");
                }
            }
            catch { }

            return sb.ToString();
        }
<<<<<<< HEAD
        public virtual string ToString(bool excludeId, bool excludeRxTime)
        {
            return ToString();
        }
=======
>>>>>>> origin/master
    }
}
