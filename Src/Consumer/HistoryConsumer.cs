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

        #region Event Invoke
        private void NotifyMessage(MessageTypes type, string message)
        {
            MessageNotified?.Invoke(type, message);
        }

        private void ConsumeBiddingPrice(BiddingPrice biddingPrice)
        {
            BiddingPriceConsumed?.Invoke(biddingPrice);
        }

        private void ConsumeCircuitBreak(CircuitBreak circuitBreak)
        {
            CircuitBreakConsumed?.Invoke(circuitBreak);
        }

        private void ConsumeIndexConclusion(IndexConclusion conclusion)
        {
            IndexConclusionConsumed?.Invoke(conclusion);
        }

        private void ConsumeStockConclusion(StockConclusion conclusion)
        {
            StockConclusionConsumed?.Invoke(conclusion);
        }

        private void ConsumeETFConclusion(ETFConclusion conclusion)
        {
            ETFConclusionConsumed?.Invoke(conclusion);
        }

        private void ConsumeStockMaster(List<StockMaster> stockMasters)
        {
            StockMasterConsumed?.Invoke(stockMasters);
        }

        private void ConsumeChart(List<Candle> chart)
        {
            ChartConsumed?.Invoke(chart);
        }

        private void ConsumeIndexMaster(List<IndexMaster> indexMasters)
        {
            IndexMasterConsumed?.Invoke(indexMasters);
        }

        private void ConsumeCodeMap(Dictionary<string, object> codeMap)
        {
            CodeMapConsumed?.Invoke(codeMap);
        } 
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
                NotifyMessage(MessageTypes.SubscribingDone, null);
                return false;
            }

            ConsumeStockMaster(masters);

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
                ConsumeStockConclusion(conclusion);
            }

            NotifyMessage(MessageTypes.SubscribingDone, null);

            sw.Stop();
            _logger.Info($"Consuming done. Elapsed:{sw.Elapsed}");

            return true;
        }

        public void StopSimulation()
        {

        }
    }
}
