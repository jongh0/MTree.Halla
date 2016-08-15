using MTree.Consumer;
using MTree.DataStructure;
using MTree.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTree.DataExtractingConsumer
{
    public class DataExtractor
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        #region Queue Task
        public CancellationToken QueueTaskCancelToken { get; set; }
        #endregion

        private ConsumerBase Consumer { get; set; }

        public DataExtractor(ConsumerBase consumer)
        {
            try
            {
                Consumer = consumer;

                Consumer.ConsumeStockMasterEvent += ConsumeStockMaster;                
                TaskUtility.Run("Dashboard.StockConclusionQueue", QueueTaskCancelToken, ProcessStockConclusionQueue);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
        public void ConsumeStockMaster(List<StockMaster> stockMasters)
        {
            try
            {
                foreach (var stockMaster in stockMasters)
                {
         
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
        private void ProcessStockConclusionQueue()
        {
            try
            {
                StockConclusion conclusion;
                if (Consumer.StockConclusionQueue.TryDequeue(out conclusion) == true)
                {
                }
                else
                {
                    Thread.Sleep(10);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

    }
}
