using MTree.DataStructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.Consumer
{
    public class SubscribableEventArgs : EventArgs
    {
        public virtual Subscribable Subscribable { get; private set; }

        public SubscribableEventArgs(Subscribable item)
        {
            this.Subscribable = item;
        }
    }
}
