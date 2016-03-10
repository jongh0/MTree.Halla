using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using MTree.DataStructure;
using MTree.Utility;
using System.Threading;

namespace MTree.RealTimeProvider
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public partial class RealTimeProvider : RealTimeBase, IRealTimePublisher, IRealTimeConsumer
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public RealTimeProvider()
        {
            GeneralTask.Run("RealTimeProvider.BiddingPriceQueue", QueueTaskCancelToken, ProcessBiddingPriceQueue);
            GeneralTask.Run("RealTimeProvider.StockConclusionQueue", QueueTaskCancelToken, ProcessStockConclusionQueue);
            GeneralTask.Run("RealTimeProvider.IndexConclusionQueue", QueueTaskCancelToken, ProcessIndexConclusionQueue);
        }

        public void NoOperation()
        {
        }
    }
}
