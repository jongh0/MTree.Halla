using MTree.RealTimeProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MTree.DataStructure;

namespace MTree.Publisher
{
    public class PublisherCallback : RealTimeBase, IRealTimePublisherCallback
    {
        public virtual void CloseClient()
        {
            LastWcfCommunicateTick = Environment.TickCount;
        }

        public virtual Dictionary<string, CodeEntity> GetStockCodeList()
        {
            LastWcfCommunicateTick = Environment.TickCount;
            return null;
        }

        public virtual StockMaster GetStockMaster(string code)
        {
            LastWcfCommunicateTick = Environment.TickCount;
            return null;
        }

        public virtual bool IsSubscribable()
        {
            LastWcfCommunicateTick = Environment.TickCount;
            return false;
        }

        public virtual bool SubscribeStock(string code)
        {
            LastWcfCommunicateTick = Environment.TickCount;
            return false;
        }

        public virtual bool UnsubscribeStock(string code)
        {
            LastWcfCommunicateTick = Environment.TickCount;
            return false;
        }

        public virtual bool SubscribeIndex(string code)
        {
            LastWcfCommunicateTick = Environment.TickCount;
            return false;
        }

        public virtual bool UnsubscribeIndex(string code)
        {
            LastWcfCommunicateTick = Environment.TickCount;
            return false;
        }

        public virtual bool SubscribeBidding(string code)
        {
            LastWcfCommunicateTick = Environment.TickCount;
            return false;
        }

        public virtual bool UnsubscribeBidding(string code)
        {
            LastWcfCommunicateTick = Environment.TickCount;
            return false;
        }
    }
}
