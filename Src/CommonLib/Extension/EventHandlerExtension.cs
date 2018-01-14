using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Extension
{
    public delegate TReturn EventHandlerWithReturn<TEventArgs, TReturn>(object sender, TEventArgs e);

    public class EventArgs<TParam> : EventArgs
    {
        public TParam Param { get; private set; }

        public EventArgs(TParam param)
        {
            Param = param;
        }
    }

    public static class EventHandlerExtension
    {
        public static EventArgs<TParam> CreateArgs<TParam>(this EventHandler<EventArgs<TParam>> self, TParam argument)
        {
            return new EventArgs<TParam>(argument);
        }

        public static EventArgs<TParam> CreateArgs<TParam, TResult>(this EventHandlerWithReturn<EventArgs<TParam>, TResult> self, TParam argument)
        {
            return new EventArgs<TParam>(argument);
        }
    }
}
