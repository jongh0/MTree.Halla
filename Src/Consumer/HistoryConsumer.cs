﻿using MongoDB.Driver;
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

namespace Consumer
{
    public interface ISimulation
    {
        bool StartSimulation(string[] codes, DateTime targetDate);
        void StopSimulation();
    }

    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class HistoryConsumer: ConsumerBase, ISimulation
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private DataLoader dataLoader = new DataLoader();
        
        public bool StartSimulation(string[] codes, DateTime targetDate)
        {
            List<StockMaster> masters = new List<StockMaster>();
            Parallel.ForEach(codes, code =>
            {
                masters.AddRange(dataLoader.Load<StockMaster>(code, targetDate, targetDate));
            });

            if (masters.Count == 0)
            {
                NotifyMessage(RealTimeProvider.MessageTypes.SubscribingDone, null);
                return false;
            }

			//return false;

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

            NotifyMessage(RealTimeProvider.MessageTypes.SubscribingDone, null);

            sw.Stop();
            _logger.Info($"Consuming done. Elapsed:{sw.Elapsed}");

            return true;
        }

        public void StopSimulation()
        {

        }
    }
}