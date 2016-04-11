using MTree.RealTimeProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.Consumer
{
    public class MessageReceivedEventArgs : EventArgs
    {
        public virtual MessageTypes Type { get; private set; }
        public virtual string Message { get; private set; }

        public MessageReceivedEventArgs(MessageTypes type, string message)
        {
            this.Type = type;
            this.Message = message;
        }
    }
}
