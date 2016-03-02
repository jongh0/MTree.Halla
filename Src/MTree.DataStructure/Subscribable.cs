using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.DataStructure
{
    [BsonDiscriminator(RootClass = true)]
    [Serializable]
    public class Subscribable
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public string Code { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Time { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{nameof(Id)}: {Id}");
            sb.AppendLine($"{nameof(Code)}: {Code}");
            sb.AppendLine($"{nameof(Time)}: {Time}");

            return sb.ToString();
        }
    }
}
