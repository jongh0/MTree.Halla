using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.Consumer
{
    public interface INotifySubscribable
    {
        event SubscribableEventHandler BiddingPriceNotified;

        event SubscribableEventHandler CircuitBreakNotified;

        event SubscribableEventHandler ConclusionNotified;
    }
}
