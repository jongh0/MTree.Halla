using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.Consumer
{
    public interface INotifySubscribableReceived
    {
        event SubscribableReceivedEventHandler BiddingPriceReceived;

        event SubscribableReceivedEventHandler CircuitBreakReceived;

        event SubscribableReceivedEventHandler ConclusionReceived;
    }
}
