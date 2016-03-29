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

        [BsonElement("PCP")]
        public double PreviousClosedPrice { get; set; }

        [BsonElement("PV")]
        public long PreviousVolume { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{nameof(Name)}: {Name}");
            sb.AppendLine($"{nameof(BasisPrice)}: {BasisPrice}");
            sb.AppendLine($"{nameof(PreviousClosedPrice)}: {PreviousClosedPrice}");
            sb.AppendLine($"{nameof(PreviousVolume)}: {PreviousVolume}");

            return sb.ToString();
        }
    }
}
