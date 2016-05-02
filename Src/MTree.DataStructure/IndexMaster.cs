using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.DataStructure
{
    [Serializable]
    public class IndexMaster : Subscribable
    {
        [BsonElement("N")]
        public string Name { get; set; }

        [BsonElement("BP")]
        public float BasisPrice { get; set; }

        public override string ToString(params string[] excludeProperties)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                foreach (var property in typeof(IndexMaster).GetProperties())
                {
                    if (excludeProperties.Contains(property.Name) == false)
                        sb.Append($"{property.Name}: {property.GetValue(this)}, ");
                }
            }
            catch { }

            return sb.ToString();
        }
    }
}
