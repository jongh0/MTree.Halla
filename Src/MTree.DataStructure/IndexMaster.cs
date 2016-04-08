using MongoDB.Bson.Serialization.Attributes;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.DataStructure
{
    [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
    [Serializable]
    public class IndexMaster : Subscribable
    {
        [BsonElement("N")]
        public string Name { get; set; }

        [BsonElement("BP")]
        public float BasisPrice { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                foreach (var property in typeof(IndexMaster).GetProperties())
                {
                    sb.AppendLine($"{property.Name}: {property.GetValue(this)}");
                }
            }
            catch { }

            return sb.ToString();
        }
    }
}
