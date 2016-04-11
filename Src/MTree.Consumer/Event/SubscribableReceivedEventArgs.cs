using MTree.DataStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.Consumer
{
    public class SubscribableNotifiedEventArgs : EventArgs
    {
        public virtual Subscribable Subscribable { get; private set; }

        public SubscribableNotifiedEventArgs(Subscribable item)
        {
            this.Subscribable = item;
        }
    }
}
