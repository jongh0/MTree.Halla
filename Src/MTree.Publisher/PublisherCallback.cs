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
        public virtual void SendMessage(MessageTypes type, string message)
        {
        }

        public virtual Dictionary<string, CodeEntity> GetCodeList()
        {
            return null;
        }

        public virtual StockMaster GetStockMaster(string code)
        {
            return null;
        }

        public virtual List<Candle> GetChart(string code, DateTime startDate, DateTime endDate, ChartTypes chartType)
        {
            return null;
        }

        public virtual bool IsSubscribable()
        {
            return false;
        }

        public virtual bool SubscribeStock(string code)
        {
            return false;
        }

        public virtual bool UnsubscribeStock(string code)
        {
            return false;
        }

        public virtual bool SubscribeIndex(string code)
        {
            return false;
        }

        public virtual bool UnsubscribeIndex(string code)
        {
            return false;
        }

        public virtual bool SubscribeBidding(string code)
        {
            return false;
        }

        public virtual bool UnsubscribeBidding(string code)
        {
            return false;
        }
    }
}
