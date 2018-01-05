using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataStructure
{
    [DataContract]
    public class IndexConclusion : Conclusion
    {
        [BsonElement("MC")]
        [DataMember(Name = "MC")]
        public long MarketCapitalization { get; set; }

        public override string ToString(params string[] excludeProperties)
        {
            //List<string> passing = new List<string>();
            //foreach (string param in excludeProperties)
            //{
            //    passing.Add(param);
            //}
            //passing.Add(nameof(this.Time));

            //return ToString(typeof(IndexConclusion), passing.ToArray());
            return ToString(typeof(IndexConclusion), excludeProperties);
        }
    }
}
