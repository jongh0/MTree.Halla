using MTree.Trader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTree.RealTimeProvider;
using System.ServiceModel;

namespace MTree.Trader
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class TraderCallback : ITraderCallback
    {
        public virtual void NotifyMessage(MessageTypes type, string message)
        {
        }

        public virtual void NotifyOrderResult(OrderResult result)
        {
        }
    }
}
