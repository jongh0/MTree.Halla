using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.Trader
{
    public class OrderResult
    {
        public UInt64 OrderNum
        {
            get;
            set;
        }

        public string Code
        {
            get;
            set;
        }

        public int ConcludedPrice
        {
            get;
            set;
        }

        public int ConcludedAmount
        {
            get;
            set;
        }

        public OrderTypes OrderType
        {
            get;
            set;
        }

        public int OrderedPrice
        {
            get;
            set;
        }

        public int OrderedAmount
        {
            get;
            set;
        }

    }
}
