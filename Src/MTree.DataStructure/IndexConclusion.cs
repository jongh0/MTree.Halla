using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.DataStructure
{
    [Serializable]
    public class IndexConclusion : Conclusion
    {
        [BsonElement("MC")]
        public long MarketCapitalization { get; set; }

<<<<<<< HEAD
            return sb.ToString();
        }
        public override string ToString(bool excludeId = true, bool excludeRxTime = true)
=======
        public override string ToString(params string[] excludeProperties)
>>>>>>> origin/master
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                foreach (var property in typeof(IndexConclusion).GetProperties())
                {
<<<<<<< HEAD
                    if (property.Name != "Id")
                        sb.Append($"{property.Name}: {property.GetValue(this)}, ");
                    else if (excludeRxTime == true && property.Name != "ReceivedTime")
=======
                    if (excludeProperties.Contains(property.Name) == false)
>>>>>>> origin/master
                        sb.Append($"{property.Name}: {property.GetValue(this)}, ");
                }
            }
            catch { }

            return sb.ToString();
        }
    }
}
