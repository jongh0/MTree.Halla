﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.Trader
{
    [Serializable]
    public class OrderResult
    {
        public string OrderNumber { get; set; }
        
        public string Code { get; set; }

        public OrderTypes OrderType { get; set; }

        public int ConcludedQuantity { get; set; }

        public int ConcludedPrice { get; set; }

        public int OrderedQuantity { get; set; }

        public int OrderedPrice { get; set; }


    }
}
