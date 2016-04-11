using MTree.Trader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.Trader
{
    public class TraderCallback : ITraderCallback
    {
        public virtual void NotifyOrderResult(OrderResult result)
        {
        }
    }
}
