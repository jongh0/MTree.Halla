using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Configuration;

namespace DataStructure
{
    [Serializable]
    public class BiddingPrice : Subscribable
    {
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime Time { get; set; }

        public List<BiddingPriceEntity> Bids { get; set; }

        public List<BiddingPriceEntity> Offers { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(base.ToString());
            sb.AppendLine($"{nameof(Time)}: {Time}");

            if (Bids != null)
            {
                foreach (var bid in Bids)
                {
                    sb.AppendLine("Bid => " + bid.ToString());
                }
            }

            if (Offers != null)
            {
                foreach (var offer in Offers)
                {
                    sb.AppendLine("Offer => " + offer.ToString());
                }
            }

            return sb.ToString();
        }
    }
}
