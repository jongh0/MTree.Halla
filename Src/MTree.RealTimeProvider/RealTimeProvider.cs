using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using MTree.DataStructure;
using MTree.Utility;
using System.Threading;
using MTree.Configuration;
using System.IO;

namespace MTree.RealTimeProvider
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public partial class RealTimeProvider : RealTimeBase, IRealTimePublisher, IRealTimeConsumer
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public DateTime MarketStartTime { get; set; }
        public DateTime MarketEndTime { get; set; }

        private System.Timers.Timer MarketEndTimer { get; set; }

        private Dictionary<string, CodeEntity> StockCodeList { get; set; } = new Dictionary<string, CodeEntity>();
        private Dictionary<string, CodeEntity> IndexCodeList { get; set; } = new Dictionary<string, CodeEntity>();

        private List<StockMastering> StockMasteringList { get; } = new List<StockMastering>();
        private List<IndexMastering> IndexMasteringList { get; } = new List<IndexMastering>();

        public RealTimeProvider()
        {
            TaskUtility.Run("RealTimeProvider.BiddingPriceQueue", QueueTaskCancelToken, ProcessBiddingPriceQueue);
            TaskUtility.Run("RealTimeProvider.CircuitBreakQueue", QueueTaskCancelToken, ProcessCircuitBreakQueue);
            TaskUtility.Run("RealTimeProvider.StockConclusionQueue", QueueTaskCancelToken, ProcessStockConclusionQueue);
            TaskUtility.Run("RealTimeProvider.IndexConclusionQueue", QueueTaskCancelToken, ProcessIndexConclusionQueue);
        }

        public void NotifyMessage(MessageTypes type, string message)
        {
            try
            {
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private void MarketEndTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            logger.Info("Market end timer elapsed");

            // Publisher 종료
            foreach (var contract in PublishContracts)
            {
                try
                {
                    logger.Info($"Close publisher client, {contract.ToString()}");
                    contract.Value.Callback.NotifyMessage(MessageTypes.CloseClient, string.Empty);
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }
            }

            // Consumer 종료
            foreach (var contract in ConsumerContracts)
            {
                try
                {
                    logger.Info($"Close consumer client, {contract.ToString()}");
                    contract.Value.Callback.NotifyMessage(MessageTypes.CloseClient, string.Empty);
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }
            }

            Task.Run(() =>
            {
                // CybosStarter 종료
                ProcessUtility.Kill(ProcessTypes.CybosStarter);

                // 당일 수집된 로그를 Zip해서 Email로 전송함
                LogUtility.SendLogToEmail();

                // 20초후 프로그램 종료
                var msg = "RealTimeProvider will be closed after 20sec";
                logger.Info(msg);
                PushUtility.NotifyMessage(msg);

                Thread.Sleep(1000 * 20);
                Environment.Exit(0);
            });
        }
    }
}
