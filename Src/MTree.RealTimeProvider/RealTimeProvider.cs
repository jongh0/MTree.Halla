﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using MTree.DataStructure;
using MTree.Utility;
using System.Threading;
using MTree.Configuration;

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
            TaskUtility.Run("RealTimeProvider.StockConclusionQueue", QueueTaskCancelToken, ProcessStockConclusionQueue);
            TaskUtility.Run("RealTimeProvider.IndexConclusionQueue", QueueTaskCancelToken, ProcessIndexConclusionQueue);
        }

        public void NotifyMessage(MessageTypes type, string message)
        {
            try
            {
                if (type == MessageTypes.MarketStartTime)
                {
                    var now = DateTime.Now;
                    int time = int.Parse(message);

                    MarketStartTime = new DateTime(now.Year, now.Month, now.Day, time, 0, 0);
                    logger.Info($"Market start time: {MarketStartTime.ToString(Config.General.TimeFormat)}");
                }
                else if (type == MessageTypes.MarketEndTime)
                {
                    var now = DateTime.Now;
                    int time = int.Parse(message);

                    MarketEndTime = new DateTime(now.Year, now.Month, now.Day, time, 0, 0);
                    logger.Info($"Market end time: {MarketEndTime.ToString(Config.General.TimeFormat)}");

                    if (MarketEndTimer != null)
                    {
                        MarketEndTimer.Stop();
                        MarketEndTimer.Dispose();
                        MarketEndTimer = null;
                    }

                    TimeSpan interval = (MarketEndTime - now).Add(TimeSpan.FromHours(1)); // 장종료 1시간 후 프로그램 종료

                    MarketEndTimer = new System.Timers.Timer();
                    MarketEndTimer.Interval = interval.TotalMilliseconds;
                    MarketEndTimer.Elapsed += MarketEndTimer_Elapsed;
                    MarketEndTimer.Start();

                    logger.Info($"Program will be closed after {interval.Hours}:{interval.Minutes}:{interval.Seconds}");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private void MarketEndTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            logger.Info("Market end timer elapsed");

            foreach (var contract in PublishContracts)
            {
                try
                {
                    logger.Info($"Close client. {contract.ToString()}");
                    contract.Value.Callback.NotifyMessage(MessageTypes.CloseClient, string.Empty);
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }
            }

            foreach (var contract in ConsumerContracts)
            {
                try
                {
                    logger.Info($"Close client. {contract.ToString()}");
                    contract.Value.Callback.NotifyMessage(MessageTypes.CloseClient, string.Empty);
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }
            }
        }
    }
}
