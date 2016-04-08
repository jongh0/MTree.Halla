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
    public class BiddingPrice : Subscribable
    {
        [BsonElement("Bs")]
        public List<BiddingPriceEntity> Bids { get; set; }

        [BsonElement("Os")]
        public List<BiddingPriceEntity> Offers { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            try
            {
                sb.Append(base.ToString());

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
