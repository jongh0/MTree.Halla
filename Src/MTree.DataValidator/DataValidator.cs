using GalaSoft.MvvmLight.Command;
using MTree.Configuration;
using MTree.DataStructure;
using MTree.DbProvider;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MTree.DataValidator
{
    public class DataValidator
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private BeyondCompare comparator { get; set; } = new BeyondCompare();

        private IDataCollector source = new DbCollector(DbAgent.Instance);
        private IDataCollector destination = new DbCollector(DbAgent.RemoteInstance);

        private const string logBasePath = "Logs";
        private const string compareResultPath = "CompareResult";
        private const string codeCompareResultFile = "CodeCompare.html";
        private const string masterCompareResultFile = "MasterCompare.html";
        private const string stockConclusionCompareResultPath = "StockConclusion";
        private const string indexConclusionCompareResultPath = "IndexConclusion";
        private const string circuitbreakCompareResultFile = "CircuitBreak.html";

        public DataValidator()
        {
        }

        public bool ValidateCodeList()
        {
            logger.Info("Code List Validation Start");
            List<string> srcCodes = new List<string>();
            srcCodes.AddRange(source.GetStockCodeList().OrderBy(s => s));
            srcCodes.AddRange(source.GetIndexCodeList().OrderBy(s => s));

            List<string> destCodes = new List<string>();
            destCodes.AddRange(destination.GetStockCodeList().OrderBy(s => s));
            destCodes.AddRange(destination.GetIndexCodeList().OrderBy(s => s));

            bool result = comparator.DoCompareItem(srcCodes, destCodes, false);
            comparator.MakeReport(srcCodes, destCodes, Path.Combine(logBasePath, Config.General.DateNow, compareResultPath, codeCompareResultFile));

            if (result == true)
                logger.Info($"Code List Validation Success");
            else
                logger.Error($"Code List Validation Fail");

            return result;
        }

        public void ValidateMasterCompare(DateTime target)
        {
            logger.Info("Stock Master Validation Start");

            Stopwatch sw = new Stopwatch();
            sw.Start();

            List<string> srcMasters = new List<string>();
            List<string> destMasters = new List<string>();

            foreach (string code in source.GetStockCodeList().OrderBy(c => c))
            {
                StockMaster srcMaster = source.GetMaster(code, target);
                logger.Info($"Get Stock Master for {code} from source done.");
                if (srcMaster != null)
                    srcMasters.Add(srcMaster.ToString());

                StockMaster destMaster = destination.GetMaster(code, target);
                logger.Info($"Get Stock Master for {code} from destination done.");
                if (destMaster != null)
                    destMasters.Add(destMaster.ToString());
            }

            bool result = comparator.DoCompareItem(srcMasters, destMasters, false);
            if (result == false)
            {
                comparator.MakeReport(srcMasters, destMasters, Path.Combine(logBasePath, Config.General.DateNow, compareResultPath, masterCompareResultFile));
                logger.Error($"Stock Master Validation Fail");
            }
            else
            {
                logger.Info($"Stock Master Validation Success");
            }

            sw.Stop();
            logger.Info($"Stock Master Validation Done. Elapsed:{sw.Elapsed}");
        }
        
        public void ValidateStockConclusionCompare(DateTime target)
        {
            logger.Info("Stock Conclusion Validation Start");
            Stopwatch sw = new Stopwatch();
            sw.Start();
            int cnt = 0;
            List<string> codeList = source.GetStockCodeList().OrderBy(c => c).ToList();
            

            Parallel.ForEach(codeList, new ParallelOptions { MaxDegreeOfParallelism = Config.Validator.ThreaLimit }, code =>
            {
                var tasks = new List<Task>();

                List<Subscribable> sourceList = new List<Subscribable>();
                List<Subscribable> destinationList = new List<Subscribable>();

                tasks.Add(Task.Run(() => { sourceList = source.GetStockConclusions(code, target, false); }));
                tasks.Add(Task.Run(() => { destinationList = destination.GetStockConclusions(code, target, false); }));

                Task.WaitAll(tasks.ToArray());

                if (sourceList.Count != 0 && destinationList.Count != 0)
                {
                    if (sourceList.Count == 0)
                        logger.Error($"Conclusion list is empty in local db.");
                    if (destinationList.Count == 0)
                        logger.Error($"Conclusion list is empty in remote db.");

                    if (comparator.DoCompareItem(sourceList, destinationList, false) == false)
                    {
                        logger.Error($"Stock Conclusion Validation for {code} Fail. {Interlocked.Increment(ref cnt)}/{codeList.Count}");
                        comparator.MakeReport(sourceList, destinationList, Path.Combine(logBasePath, Config.General.DateNow, compareResultPath, stockConclusionCompareResultPath, code + ".html"));
                    }
                    else
                    {
                        logger.Info($"Stock Conclusion Validation for {code} success. {Interlocked.Increment(ref cnt)}/{codeList.Count}");
                    }
                }
            });
            sw.Stop();
            logger.Info($"Stock Conclusion Validation Done. Elapsed:{sw.Elapsed}");
        }

        public void ValidateStockConclusionCompare(DateTime target, string code)
        {
            if (code == null)
            {
                logger.Warn("Code is empty");
                return;
            }

            logger.Info($"Stock Conclusion Validation for {code} Start");
            
            var tasks = new List<Task>();

            List<Subscribable> sourceList = new List<Subscribable>();
            List<Subscribable> destinationList = new List<Subscribable>();

            tasks.Add(Task.Run(() => { sourceList = source.GetStockConclusions(code, target, false); }));
            tasks.Add(Task.Run(() => { destinationList = destination.GetStockConclusions(code, target, false); }));

            Task.WaitAll(tasks.ToArray());

            if (sourceList.Count != 0 && destinationList.Count != 0)
            {
                if (sourceList.Count == 0)
                    logger.Error($"Conclusion list is empty in local db.");
                if (destinationList.Count == 0)
                    logger.Error($"Conclusion list is empty in remote db.");

                if (comparator.DoCompareItem(sourceList, destinationList, false) == false)
                {
                    logger.Error($"Stock Conclusion Validation for {code} Fail.");
                    comparator.MakeReport(sourceList, destinationList, Path.Combine(logBasePath, Config.General.DateNow, compareResultPath, stockConclusionCompareResultPath, code + ".html"));
                }
                else
                {
                    logger.Info($"Stock Conclusion Validation for {code} success.");
                }
            }

            logger.Info($"Stock Conclusion Validation for {code} Done.");
        }

        public void ValidateStockConclusionCompareWithDaishin(DateTime target)
        {
            logger.Info("Stock Conclusion Validation with Daishin Start.");
            int cnt = 0;
            IDataCollector daishin = new DaishinCollector();

            foreach (string code in source.GetStockCodeList().OrderBy(c => c))
            {
                var tasks = new List<Task>();

                List<Subscribable> sourceList = new List<Subscribable>();
                List<Subscribable> destinationList = new List<Subscribable>();

                tasks.Add(Task.Run(() => { sourceList = source.GetStockConclusions(code, target, true); }));
                tasks.Add(Task.Run(() => { destinationList = daishin.GetStockConclusions(code, target, true); }));

                Task.WaitAll(tasks.ToArray());
                
                if (sourceList.Count != 0 && destinationList.Count != 0)
                {
                    if (sourceList.Count == 0)
                        logger.Error($"Conclusion list is empty in local db.");
                    if (destinationList.Count == 0)
                        logger.Error($"Conclusion list is empty in Daishin.");

                    if (comparator.DoCompareItem(sourceList, destinationList, false) == false)
                    {
                        logger.Error($"Stock Conclusion Validation for {code} Fail. {cnt}/{source.GetStockCodeList().Count}");
                        comparator.MakeReport(sourceList, destinationList, Path.Combine(logBasePath, Config.General.DateNow, compareResultPath, stockConclusionCompareResultPath, code + ".html"));
                    }
                    else
                    {
                        logger.Info($"Stock Conclusion Validation for {code} success. {cnt}/{source.GetStockCodeList().Count}");
                    }
                }

                cnt++;
            }
            logger.Info("Stock Conclusion Validation with Daishin Done.");
        }

        public void ValidateStockConclusionCompareWithDaishin(DateTime target, string code)
        {
            logger.Info($"Stock Conclusion Validation for {code} with Daishin Start.");

            IDataCollector daishin = new DaishinCollector();

            var tasks = new List<Task>();

            List<Subscribable> sourceList = new List<Subscribable>();
            List<Subscribable> destinationList = new List<Subscribable>();

            tasks.Add(Task.Run(() => { sourceList = source.GetStockConclusions(code, target, true); }));
            tasks.Add(Task.Run(() => { destinationList = daishin.GetStockConclusions(code, target, true); }));

            Task.WaitAll(tasks.ToArray());

            if (sourceList.Count != 0 && destinationList.Count != 0)
            {
                if (sourceList.Count == 0)
                    logger.Error($"Conclusion list is empty in local db.");
                if (destinationList.Count == 0)
                    logger.Error($"Conclusion list is empty in Daishin.");

                if (comparator.DoCompareItem(sourceList, destinationList, false) == false)
                {
                    logger.Error($"Stock Conclusion Validation for {code} Fail.");
                    comparator.MakeReport(sourceList, destinationList, Path.Combine(logBasePath, Config.General.DateNow, compareResultPath, stockConclusionCompareResultPath, code + ".html"));
                }
                else
                {
                    logger.Info($"Stock Conclusion Validation for {code} success.");
                }

            }
            logger.Info($"Stock Conclusion Validation for {code} with Daishin Done.");
        }

        public void ValidateIndexConclusionCompare(DateTime target)
        {
            logger.Info("Index Conclusion Validation Start.");
            Stopwatch sw = new Stopwatch();
            sw.Start();
            List<string> codeList = source.GetIndexCodeList().OrderBy(c => c).ToList();

            Parallel.ForEach(codeList, new ParallelOptions { MaxDegreeOfParallelism = Config.Validator.ThreaLimit }, code =>
            {
                var tasks = new List<Task>();

                List<Subscribable> sourceList = new List<Subscribable>();
                List<Subscribable> destinationList = new List<Subscribable>();

                tasks.Add(Task.Run(() => { sourceList = source.GetIndexConclusions(code, target, false); }));
                tasks.Add(Task.Run(() => { destinationList = destination.GetIndexConclusions(code, target, false); }));

                Task.WaitAll(tasks.ToArray());

                if (comparator.DoCompareItem(sourceList, destinationList, false) == false)
                {
                    logger.Error($"Index Conclusion Validation for {code} Fail.");
                    comparator.MakeReport(sourceList, destinationList, Path.Combine(logBasePath, Config.General.DateNow, compareResultPath, indexConclusionCompareResultPath, code + ".html"));
                }
            });
            sw.Stop();
            logger.Info($"Index Conclusion Validation Done. Elapsed:{sw.Elapsed}");
        }

        public void ValidateIndexConclusionCompare(DateTime target, string code)
        {
            if (code == null)
            {
                logger.Warn("Code is empty");
                return;
            }

            logger.Info($"Index Conclusion Validation for {code} Start.");

            var tasks = new List<Task>();

            List<Subscribable> sourceList = new List<Subscribable>();
            List<Subscribable> destinationList = new List<Subscribable>();

            tasks.Add(Task.Run(() => { sourceList = source.GetIndexConclusions(code, target, false); }));
            tasks.Add(Task.Run(() => { destinationList = destination.GetIndexConclusions(code, target, false); }));

            Task.WaitAll(tasks.ToArray());

            if (comparator.DoCompareItem(sourceList, destinationList, false) == false)
            {
                logger.Error($"Index Conclusion Validation for {code} Fail.");
                comparator.MakeReport(sourceList, destinationList, Path.Combine(logBasePath, Config.General.DateNow, compareResultPath, indexConclusionCompareResultPath, code + ".html"));
            }

            logger.Info($"Index Conclusion Validation for {code} Done.");
        }

        public void ValidateCircuitBreakCompare(DateTime target)
        {
            logger.Info("Circuit Break Validation Start.");
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var tasks = new List<Task>();

            List<Subscribable> sourceList = new List<Subscribable>();
            List<Subscribable> destinationList = new List<Subscribable>();

            foreach (string code in source.GetStockCodeList())
            {
                tasks.Add(Task.Run(() => { sourceList.AddRange(source.GetCircuitBreaks(code, target)); }));
                tasks.Add(Task.Run(() => { destinationList.AddRange(destination.GetCircuitBreaks(code, target)); }));
                
                Task.WaitAll(tasks.ToArray());
            }
            
            if (comparator.DoCompareItem(sourceList, destinationList, false) == false)
            {
                logger.Error("Circuit Break Validation Fail.");
                comparator.MakeReport(sourceList, destinationList, Path.Combine(logBasePath, Config.General.DateNow, compareResultPath, circuitbreakCompareResultFile));
            }
            sw.Stop();
            logger.Info($"Circuit Break Validation Done. Elapsed:{sw.Elapsed}");
        }
    }
}
