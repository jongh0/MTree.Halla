using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.Consumer
{
    public interface INotifyMessageReceived
    {
        event MessageReceivedEventHandler MessageReceived;
    }
}
