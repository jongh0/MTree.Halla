using MongoDB.Driver;
using MTree.DataStructure;
using MTree.DbProvider;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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
        
        public void StartSimulation(string[] codes, DateTime targetDate)
        {
            List<StockConclusion> conclusions = new List<StockConclusion>();
            var builder = Builders<StockConclusion>.Filter;
            var filter = builder.Gte(i => (i as Subscribable).Time, targetDate) & builder.Lte(i => (i as Subscribable).Time, targetDate);

            Stopwatch sw = new Stopwatch();
            logger.Info("Start to load history from db");
            sw.Start();
            foreach (string code in codes)
            {
                conclusions.Union(DbAgent.Instance.Find(code, filter).ToList()).OrderBy(conclusion => conclusion.Time);
            }
            sw.Stop();
            logger.Info($"Loading done. Elapsed:{sw.Elapsed}");
            
            logger.Info("Start consuming");
            sw.Start();
            foreach (StockConclusion conclusion in conclusions)
            {
                ConsumeStockConclusion(conclusion);
            }
            sw.Stop();
            logger.Info($"Consuming done. Elapsed:{sw.Elapsed}");
        }
        public void StopSimulation()
        {

        }
    }
}
