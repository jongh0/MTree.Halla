﻿using MongoDB.Driver;
using MTree.Configuration;
using MTree.DataStructure;
using MTree.DbProvider;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace MTree.DataValidator
{
    public class DataRecoverer
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        
        public DbAgent From { get; set; } = DbAgent.Instance;

        public DbAgent To { get; set; } = DbAgent.RemoteInstance;

        public DataRecoverer()
        {
        }

        public DataRecoverer(DbAgent from, DbAgent to)
        {
            From = from;
            To = to;
        }

        public void RecoverMaster(DateTime targetDate, string code, bool needConfirm = true)
        {
            if (code.Length == 3)
            {
                RecoverIndexMaster(targetDate, code, needConfirm);
            }
            else
            {
                RecoverStockMaster(targetDate, code, needConfirm);
            }
        }

        private void RecoverIndexMaster(DateTime targetDate, string code, bool needConfirm = true)
        {
            logger.Info($"Index Master Recovery for {code} Started");
            var filter = FilterFactory.Instance.BuildIndexMasterFilter(targetDate);

            IndexMaster master = To.Find(code, filter).FirstOrDefault();
            if (master != null && needConfirm == true)
            {
                MessageBoxResult answer = MessageBox.Show($"Master for {code} is already existing. Force to remove and recover?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (answer != MessageBoxResult.Yes)
                {
                    logger.Info($"Stock Master Recovery for {code} Canceled.");
                    return;
                }
            }

            To.Delete(code, filter);
            To.Insert(From.Find(code, filter).FirstOrDefault());
            logger.Info($"Index Master Recovery for {code} Done");
        }

        private void RecoverStockMaster(DateTime targetDate, string code, bool needConfirm = true)
        {
            logger.Info($"Stock Master Recovery for {code} Started");
            var filter = FilterFactory.Instance.BuildStockMasterFilter(targetDate);

            StockMaster master = To.Find(code, filter).FirstOrDefault();
            if (master != null && needConfirm == true)
            {
                MessageBoxResult answer = MessageBox.Show($"Master for {code} is already existing. Force to remove and recover?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (answer != MessageBoxResult.Yes)
                {
                    logger.Info($"Stock Master Recovery for {code} Canceled.");
                    return;
                }
            }

            To.Delete(code, filter);
            To.Insert(From.Find(code, filter).FirstOrDefault());
            logger.Info($"Stock Master Recovery for {code} Done");
        }

        public void RecoverMasters(DateTime targetDate, bool needConfirm = true)
        {
            logger.Info($"Index Masters Recovery Started");
            Stopwatch sw = new Stopwatch();
            sw.Start();

            RecoverIndexMasters(targetDate, needConfirm);
            RecoverStockMasters(targetDate, needConfirm);

            sw.Stop();
            logger.Info($"Stock Masters Recovery Done. Elapsed:{sw.Elapsed}");
        }

        private void RecoverIndexMasters(DateTime targetDate, bool needConfirm = true)
        {
            int cnt = 0;

            List<string> codeList = new List<string>();
            codeList.AddRange(From.GetCollectionList(DbTypes.IndexMaster).OrderBy(s => s));
            var filter = FilterFactory.Instance.BuildIndexMasterFilter(targetDate);
            
            Parallel.ForEach(codeList, new ParallelOptions { MaxDegreeOfParallelism = Config.Validator.ThreadLimit }, code =>
            {
                IndexMaster master = To.Find(code, filter).FirstOrDefault();
                if (master != null && needConfirm == true)
                {
                    MessageBoxResult answer = MessageBox.Show($"Master for {code} is already existing. Force to remove and recover?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (answer != MessageBoxResult.Yes)
                    {
                        logger.Info($"Stock Master Recovery for {code} Canceled.");
                        return;
                    }
                }
                To.Delete(code, filter);
                To.Insert(From.Find(code, filter).FirstOrDefault());
                logger.Info($"Stock Master Recovery for {code} Done. {Interlocked.Increment(ref cnt)}/{codeList.Count}");
            });
        }

        private void RecoverStockMasters(DateTime targetDate, bool needConfirm = true)
        {
            int cnt = 0;

            List<string> codeList = new List<string>();
            codeList.AddRange(From.GetCollectionList(DbTypes.StockMaster).OrderBy(s => s));
            var filter = FilterFactory.Instance.BuildStockMasterFilter(targetDate);
            
            Parallel.ForEach(codeList, new ParallelOptions { MaxDegreeOfParallelism = Config.Validator.ThreadLimit }, code =>
            {
                if (needConfirm == true)
                {
                    StockMaster master = To.Find(code, filter).FirstOrDefault();
                    if (master != null)
                    {
                        MessageBoxResult answer = MessageBox.Show($"Master for {code} is already existing. Force to remove and recover?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                        if (answer != MessageBoxResult.Yes)
                        {
                            logger.Info($"Stock Master Recovery for {code} Canceled.");
                            return;
                        }
                    }
                }
                To.Delete(code, filter);
                To.Insert(From.Find(code, filter).FirstOrDefault());
                logger.Info($"Stock Master Recovery for {code} Done. {Interlocked.Increment(ref cnt)}/{codeList.Count}");
            });
        }

        public void RecoverStockConclusion(DateTime targetDate, string code, bool needConfirm = true)
        {
            logger.Info($"Stock Conclusion Recovery for {code} Started");
            var filter = FilterFactory.Instance.BuildStockConclusionFilter(targetDate);
            List<StockConclusion> conclusions = To.Find(code, filter).ToList();
            if (conclusions.Count > 0 && needConfirm == true)
            {
                MessageBoxResult answer = MessageBox.Show($"Master for {code} is already existing. Force to remove and recover?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (answer != MessageBoxResult.Yes)
                {
                    logger.Info($"Stock Master Recovery for {code} Canceled.");
                    return;
                }
            }
            To.Delete(code, filter);
            conclusions = From.Find(code, filter).ToList();
            foreach (StockConclusion conclusion in conclusions)
            {
                To.Insert(conclusion);
            }
            logger.Info($"Stock Conclusion Recovery for {code} Done");
        }

        public void RecoverStockConclusions(DateTime targetDate, bool needConfirm = true)
        {
            logger.Info($"Stock Conclusion Recovery Started");
            int cnt = 0;
            List<string> codeList = new List<string>();
            codeList.AddRange(From.GetCollectionList(DbTypes.StockMaster).OrderBy(s => s));
            var filter = FilterFactory.Instance.BuildStockConclusionFilter(targetDate);
            Parallel.ForEach(codeList, new ParallelOptions { MaxDegreeOfParallelism = Config.Validator.ThreadLimit }, code =>
            {
                List<StockConclusion> conclusions = To.Find(code, filter).ToList();
                if (conclusions.Count > 0 && needConfirm == true)
                {
                    MessageBoxResult answer = MessageBox.Show($"Stock Conclusion for {code} is already existing. Force to remove and recover?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (answer != MessageBoxResult.Yes)
                    {
                        logger.Info($"Stock Conclusion Recovery for {code} Canceled.");
                        return;
                    }
                }
                To.Delete(code, filter);
                conclusions = From.Find(code, filter).ToList();
                foreach (StockConclusion conclusion in conclusions)
                {
                    To.Insert(conclusion);
                }
                logger.Info($"Stock Conclusion Recovery for {code} Done. {Interlocked.Increment(ref cnt)}/{codeList.Count}");
            });
            logger.Info($"Stock Conclusion Recovery Done");
        }


        public void RecoverIndexConclusion(DateTime targetDate, string code, bool needConfirm = true)
        {
            logger.Info($"Index Conclusion Recovery for {code} Started");
            var filter = FilterFactory.Instance.BuildIndexConclusionFilter(targetDate);
            List<IndexConclusion> conclusions = To.Find(code, filter).ToList();
            if (conclusions.Count > 0 && needConfirm == true)
            {
                MessageBoxResult answer = MessageBox.Show($"Index Conclusion for {code} is already existing. Force to remove and recover?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (answer != MessageBoxResult.Yes)
                {
                    logger.Info($"Index Conclusion Recovery for {code} Canceled.");
                    return;
                }
            }
            To.Delete(code, filter);
            conclusions = From.Find(code, filter).ToList();
            foreach (IndexConclusion conclusion in conclusions)
            {
                To.Insert(conclusion);
            }
            logger.Info($"Index Conclusion Recovery for {code} Done");
        }

        public void RecoverIndexConclusions(DateTime targetDate, bool needConfirm = true)
        {
            logger.Info($"Index Conclusion Recovery Started");
            int cnt = 0;
            List<string> codeList = new List<string>();
            codeList.AddRange(From.GetCollectionList(DbTypes.IndexMaster).OrderBy(s => s));
            var filter = FilterFactory.Instance.BuildIndexConclusionFilter(targetDate);
            Parallel.ForEach(codeList, new ParallelOptions { MaxDegreeOfParallelism = Config.Validator.ThreadLimit }, code =>
            {
                List<IndexConclusion> conclusions = To.Find(code, filter).ToList();
                if (conclusions.Count > 0 && needConfirm == true)
                {
                    MessageBoxResult answer = MessageBox.Show($"Index Conclusion for {code} is already existing. Force to remove and recover?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (answer != MessageBoxResult.Yes)
                    {
                        logger.Info($"Index Conclusion Recovery for {code} Canceled.");
                        return;
                    }
                }
                To.Delete(code, filter);
                conclusions = From.Find(code, filter).ToList();
                foreach (IndexConclusion conclusion in conclusions)
                {
                    To.Insert(conclusion);
                }
                logger.Info($"Index Conclusion Recovery for {code} Done. {Interlocked.Increment(ref cnt)}/{codeList.Count}");
            });
            logger.Info($"Index Conclusion Recovery Done");
        }
        
        public void RecoverCircuitBreak(DateTime targetDate, string code, bool needConfirm = true)
        {
            logger.Info($"Circuit Break Recovery for {code} Started");
            var filter = FilterFactory.Instance.BuildCircuitBreakFilter(targetDate);
            List<CircuitBreak> cbs = To.Find(code, filter).ToList();
            if (cbs.Count > 0 && needConfirm == true)
            {
                MessageBoxResult answer = MessageBox.Show($"Circuit Break for {code} is already existing. Force to remove and recover?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (answer != MessageBoxResult.Yes)
                {
                    logger.Info($"Circuit Break Recovery for {code} Canceled.");
                    return;
                }
            }
            To.Delete(code, filter);
            cbs = From.Find(code, filter).ToList();
            foreach (CircuitBreak cb in cbs)
            {
                To.Insert(cb);
            }
            logger.Info($"Circuit Break Recovery for {code} Done");
        }

        public void RecoverCircuitBreaks(DateTime targetDate, bool needConfirm = true)
        {
            logger.Info($"Circuit Break Recovery Started");
            int cnt = 0;
            List<string> codeList = new List<string>();
            codeList.AddRange(From.GetCollectionList(DbTypes.StockMaster).OrderBy(s => s));
            var filter = FilterFactory.Instance.BuildCircuitBreakFilter(targetDate);
            Parallel.ForEach(codeList, new ParallelOptions { MaxDegreeOfParallelism = Config.Validator.ThreadLimit }, code =>
            {
                List<CircuitBreak> cbs = To.Find(code, filter).ToList();
                if (cbs.Count > 0 && needConfirm == true)
                {
                    MessageBoxResult answer = MessageBox.Show($"Circuit Break for {code} is already existing. Force to remove and recover?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                    if (answer != MessageBoxResult.Yes)
                    {
                        logger.Info($"Circuit Break Recovery for {code} Canceled.");
                        return;
                    }
                }
                To.Delete(code, filter);
                cbs = From.Find(code, filter).ToList();
                foreach (CircuitBreak cb in cbs)
                {
                    To.Insert(cb);
                }
                logger.Info($"Circuit Break Recovery for {code} Done. {Interlocked.Increment(ref cnt)}/{codeList.Count}");
            });
            logger.Info($"Circuit Break Recovery Done");
        }
    }
}
