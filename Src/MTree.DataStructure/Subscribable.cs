using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MTree.Configuration;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
                    typeof(IndexMaster),
                    typeof(ETFConclusion),
                    typeof(CodeMapDbObject))]
    [Serializable]
    [ProtoContract(ImplicitFields = ImplicitFields.AllFields)]
    [ProtoInclude(100, typeof(Candle))]
    [ProtoInclude(101, typeof(BiddingPrice))]
    [ProtoInclude(102, typeof(CircuitBreak))]
    [ProtoInclude(103, typeof(Conclusion))]
    [ProtoInclude(104, typeof(StockMaster))]
    [ProtoInclude(105, typeof(IndexMaster))]
    [ProtoInclude(106, typeof(ETFConclusion))]
    [ProtoInclude(107, typeof(CodeMapDbObject))]
    public class Subscribable
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("C")]
        public string Code { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        [BsonElement("T")]
        public DateTime Time { get; set; }

        [BsonIgnore]
        public DateTime ReceivedTime { get; set; }

        public override string ToString()
        {
            return ToString(string.Empty);
        }

        public virtual string ToString(params string[] excludeProperties)
        {
            return ToString(typeof(Subscribable), excludeProperties);
        }

        public string ToString(Type type, params string[] excludeProperties)
        {
            List<string> strList = new List<string>();

            try
            {
                foreach (var property in type.GetProperties())
                {
                    if (excludeProperties.Contains(property.Name) == false)
                    {
                        object value = property.GetValue(this);

                        if (value is DateTime)
                        {
                            DateTime dateTime = (DateTime)value;
                            strList.Add($"{property.Name}: {dateTime.ToString(Config.General.DateTimeFormat)}");
                        }
                        else
                        {
                            strList.Add($"{property.Name}: {value}");
                        }
                    }
                }

                return string.Join(", ", strList.ToArray());
            }
            catch
            {
            }
            finally
            {
                strList.Clear();
            }

            return string.Empty;
        }
    }
}
