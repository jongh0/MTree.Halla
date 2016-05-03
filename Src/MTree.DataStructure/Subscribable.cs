using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MTree.Configuration;
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

        [BsonElement("Ts")]
        public long Timestamp { get; set; }

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
                    if (excludeProperties.Contains(property.Name) == false)
                    {
                        object value = property.GetValue(this);

                        if (value is DateTime)
                        {
                            DateTime dateTime = (DateTime)value;
                            sb.Append($"{property.Name}: {dateTime.ToString(Config.General.DateTimeFormat)}, ");
                        }
                        else
                        {
                            sb.Append($"{property.Name}: {value}, ");
                        }
                    }
                }
            }
            catch { }

            return sb.ToString();
        }
    }
}
