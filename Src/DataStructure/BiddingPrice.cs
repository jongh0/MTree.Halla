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
    public class BiddingPrice : Subscribable
    {
        [BsonElement("Bs")]
        [DataMember(Name = "Bs")]
        public List<BiddingPriceEntity> Bids { get; set; }

        [BsonElement("Os")]
        [DataMember(Name = "Os")]
        public List<BiddingPriceEntity> Offers { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            try
            {
                sb.Append(base.ToString());
                sb.Append(Environment.NewLine);

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
            }
            catch { }

            return sb.ToString();
        }
    }
}
