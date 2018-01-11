using MongoDB.Driver;
using DataStructure;
using DbProvider;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.ServiceModel;
using RealTimeProvider;

namespace Consumer
{
    public interface ISimulation
    {
        bool StartSimulation(string[] codes, DateTime targetDate);
        void StopSimulation();
    }

    public class HistoryConsumer : IConsumer, ISimulation
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private DataLoader dataLoader = new DataLoader();

        #region Event
        public event Action<MessageTypes, string> MessageNotified;
        public event Action<BiddingPrice> BiddingPriceConsumed;
        public event Action<CircuitBreak> CircuitBreakConsumed;
        public event Action<IndexConclusion> IndexConclusionConsumed;
        public event Action<StockConclusion> StockConclusionConsumed;
        public event Action<ETFConclusion> ETFConclusionConsumed;
        public event Action<List<StockMaster>> StockMasterConsumed;
        public event Action<List<IndexMaster>> IndexMasterConsumed;
        public event Action<Dictionary<string, object>> CodeMapConsumed;
        public event Action<List<Candle>> ChartConsumed; 
        #endregion

        public bool StartSimulation(string[] codes, DateTime targetDate)
        {
            List<StockMaster> masters = new List<StockMaster>();
            Parallel.ForEach(codes, code =>
            {
                masters.AddRange(dataLoader.Load<StockMaster>(code, targetDate, targetDate));
            });

            if (masters.Count == 0)
            {
                MessageNotified?.Invoke(MessageTypes.SubscribingDone, null);
                return false;
            }

            StockMasterConsumed?.Invoke(masters);

            object conclusionLock = new object();
            List<StockConclusion> conclusions = new List<StockConclusion>();
            
            Stopwatch sw = new Stopwatch();
            _logger.Info("Start to load history from db");
            sw.Start();

            Parallel.ForEach(codes, code =>
            {
                List<StockConclusion> tempConclusions = dataLoader.Load<StockConclusion>(code, targetDate, targetDate);
                lock (conclusionLock)
                {
                    conclusions.AddRange(tempConclusions);
                    conclusions = conclusions.OrderBy(conclusion => conclusion.Time).ToList();
                }
            });

            sw.Stop();
            _logger.Info($"Loading done. Elapsed:{sw.Elapsed}");
            
            _logger.Info("Start consuming");
            sw.Start();

            foreach (StockConclusion conclusion in conclusions)
            {
                StockConclusionConsumed?.Invoke(conclusion);
            }

            MessageNotified?.Invoke(MessageTypes.SubscribingDone, null);

            sw.Stop();
            _logger.Info($"Consuming done. Elapsed:{sw.Elapsed}");

            return true;
        }

        public void StopSimulation()
        {

        }
    }
}
