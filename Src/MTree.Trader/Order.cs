using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.Trader
{

    public class Order
    {
        public OrderTypes OrderType
        {
            get;
            set;
        }

        public string Code
        {
            get;
            set;
        }

        public int OrderPrice
        {
            get;
            set;
        }

        public int Amount
        {
            get;
            set;
        }

    }
}
