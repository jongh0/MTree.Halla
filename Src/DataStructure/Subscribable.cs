using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataStructure
{
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
    [DataContract]
    [KnownType(typeof(Candle))]
    [KnownType(typeof(BiddingPrice))]
    [KnownType(typeof(CircuitBreak))]
    [KnownType(typeof(IndexConclusion))]
    [KnownType(typeof(StockConclusion))]
    [KnownType(typeof(StockMaster))]
    [KnownType(typeof(IndexMaster))]
    [KnownType(typeof(ETFConclusion))]
    [KnownType(typeof(CodeMapDbObject))]
    public class Subscribable
    {
        [BsonId]
        [DataMember]
        public ObjectId Id { get; set; }

        [BsonElement("C")]
        [DataMember(Name = "C")]
        public string Code { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        [BsonElement("T")]
        [DataMember(Name = "T")]
        public DateTime Time { get; set; }

        [BsonIgnore]
        [DataMember(Name = "RT")]
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

                        if (value is DateTime dateTime)
                            strList.Add($"{property.Name}: {dateTime.ToString(Config.General.DateTimeFormat)}");
                        else
                            strList.Add($"{property.Name}: {value}");
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
