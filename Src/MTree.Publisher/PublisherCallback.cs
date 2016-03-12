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
        public void NoOperation()
        {
        }

        public virtual void CloseClient()
        {
        }

        public virtual Dictionary<string, string> GetStockCodeList()
        {
            return new Dictionary<string, string>();
        }

        public virtual StockMaster GetStockMaster(string code)
        {
            return new StockMaster();
        }

        public virtual bool IsSubscribable()
        {
            return true;
        }

        public virtual bool SubscribeStock(string code)
        {
            return true;
        }

        public virtual bool UnsubscribeStock(string code)
        {
            return true;
        }

        public virtual bool SubscribeIndex(string code)
        {
            return true;
        }

        public virtual bool UnsubscribeIndex(string code)
        {
            return true;
        }
    }
}
