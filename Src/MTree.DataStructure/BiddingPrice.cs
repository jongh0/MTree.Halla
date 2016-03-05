using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.DataStructure
{
    [Serializable]
    public class BiddingPrice : Subscribable
    {
        public List<BiddingPriceEntity> Bids { get; set; }

        public List<BiddingPriceEntity> Offers { get; set; }

        public BiddingPrice()
        {
            // TODO : 생성해주는게 맞나?
            //Bids = new List<BiddingPriceEntity>();
            //Offers = new List<BiddingPriceEntity>();
        }

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
