using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStructure
{
    [BsonDiscriminator(RootClass = true)]
    [Serializable]
    public class ISubscribable
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public string Code { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Id: {Id}");
            sb.AppendLine($"Code: {Code}");

            return sb.ToString();
        }
    }
}
