using MongoDB.Driver;
using MTree.DataStructure;
using MTree.DbProvider;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTree.Consumer
{
    public interface ISimulation
    {
        void StartSimulation(string[] codes, DateTime targetDate);
        void StopSimulation();
    }

    public class HistoryConsumer: ConsumerBase, ISimulation
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private DataLoader dataLoader = new DataLoader();
        
        public void StartSimulation(string[] codes, DateTime targetDate)
        {
            List<StockMaster> masters = new List<StockMaster>();
            Parallel.ForEach(codes, code =>
            {
                masters.AddRange(dataLoader.Load<StockMaster>(code, targetDate, targetDate));
            });

            if (masters.Count == 0)
                return;

            ConsumeStockMaster(masters);

            object conclusionLock = new object();
            List<StockConclusion> conclusions = new List<StockConclusion>();
            
            Stopwatch sw = new Stopwatch();
            logger.Info("Start to load history from db");
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
            logger.Info($"Loading done. Elapsed:{sw.Elapsed}");
            
            logger.Info("Start consuming");
            sw.Start();
            foreach (StockConclusion conclusion in conclusions)
            {
                ConsumeStockConclusion(conclusion);
            }

            NotifyMessage(RealTimeProvider.MessageTypes.SubscribingDone, null);

            sw.Stop();
            logger.Info($"Consuming done. Elapsed:{sw.Elapsed}");
        }
        public void StopSimulation()
        {

        }
    }
}
