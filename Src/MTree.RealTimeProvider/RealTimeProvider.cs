using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using MTree.DataStructure;

namespace MTree.RealTimeProvider
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class RealTimeProvider : IRealTimePublisher, IRealTimeConsumer
    {
        public void KeepConnection()
        {
            throw new NotImplementedException();
        }

        public void PublishBiddingPrice(BiddingPrice biddingPrice)
        {
            throw new NotImplementedException();
        }

        public void PublishCircuitBreak(CircuitBreak circuitBreak)
        {
            throw new NotImplementedException();
        }

        public void PublishIndexConclusion(IndexConclusion conclusion)
        {
            throw new NotImplementedException();
        }

        public void PublishStockConclusion(StockConclusion conclusion)
        {
            throw new NotImplementedException();
        }
    }
}
