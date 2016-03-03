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
        public string Name { get; set; }

        [BsonElement("PCP")]
        public double PreviousClosedPrice { get; set; }

        [BsonElement("PV")]
        public long PreviousVolume { get; set; }

        [BsonElement("PTC")]
        public long PreviousTradeCost { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{nameof(Name)}: {Name}");
            sb.AppendLine($"{nameof(PreviousClosedPrice)}: {PreviousClosedPrice}");
            sb.AppendLine($"{nameof(PreviousVolume)}: {PreviousVolume}");
            sb.AppendLine($"{nameof(PreviousTradeCost)}: {PreviousTradeCost}");

            return sb.ToString();
        }
    }
}
