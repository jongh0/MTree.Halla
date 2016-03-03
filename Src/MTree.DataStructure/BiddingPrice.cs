﻿using MongoDB.Bson.Serialization.Attributes;
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

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
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

            return sb.ToString();
        }
    }
}
