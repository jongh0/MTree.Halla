﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStructure
{
    [Serializable]
    public class BiddingPriceEntity
    {
        public int Index { get; set; }

        public float Price { get; set; }

        public long Amount { get; set; }

        public override string ToString()
        {
            return $"Index: {Index}, Price: {Price}, Amount: {Amount}";
        }
    }
}
