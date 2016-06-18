using GalaSoft.MvvmLight.Command;
using MongoDB.Driver;
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

        private DbAgent source { get; set; } = DbAgent.Instance;
        private DbAgent destination { get; set; } = DbAgent.RemoteInstance;

        private const string logBasePath = "Logs";
        private const string compareResultPath = "CompareResult";
        private const string codeCompareResultFile = "CodeCompare.html";
        private const string masterCompareResultPath = "Master";
        private const string stockConclusionCompareResultPath = "StockConclusion";
        private const string indexConclusionCompareResultPath = "IndexConclusion";
        private const string circuitbreakCompareResultPath = "CircuitBreak";

        public DataValidator()
        {
        }

        public DataValidator(DbAgent src, DbAgent dest)
        {
            source = src;
            destination = dest;
        }

        public bool ValidateCodeList()
        {
            logger.Info("Code List Validation Start");
            List<string> srcCodes = new List<string>();
            srcCodes.AddRange(source.GetCollectionList(DbTypes.IndexMaster).OrderBy(s => s));
            srcCodes.AddRange(source.GetCollectionList(DbTypes.StockMaster).OrderBy(s => s));
            
            List<string> destCodes = new List<string>();
            destCodes.AddRange(destination.GetCollectionList(DbTypes.IndexMaster).OrderBy(s => s));
            destCodes.AddRange(destination.GetCollectionList(DbTypes.StockMaster).OrderBy(s => s));
            
            bool result = comparator.DoCompareItem(srcCodes, destCodes, false);
            comparator.MakeReport(srcCodes, destCodes, Path.Combine(logBasePath, Config.General.DateNow, compareResultPath, codeCompareResultFile));

            if (result == true)
                logger.Info($"Code List Validation Success");
            else
                logger.Error($"Code List Validation Fail");

            return result;
        }

        public bool ValidateMaster(DateTime targetDate, string code, bool makeReport = true)
        {
            if (code.Length == 3)
            {
                return ValidateIndexMaster(targetDate, code, makeReport);
            }
            else
            {
                return ValidateStockMaster(targetDate, code, makeReport);
            }
        }

        public bool ValidateIndexMaster(DateTime targetDate, string code, bool makeReport = true)
        {
            //logger.Info("Index Master Validation Start");

            Stopwatch sw = new Stopwatch();
            sw.Start();

            var filter = FilterFactory.Instance.BuildIndexMasterFilter(targetDate);

            IndexMaster srcMaster = source.Find(code, filter).FirstOrDefault();
            //logger.Info($"Get Index Master for {code} from source done.");

            IndexMaster destMaster = destination.Find(code, filter).FirstOrDefault();
            //logger.Info($"Get Index Master for {code} from destination done.");

            bool result = comparator.DoCompareItem(srcMaster, destMaster, false);
            if (result == false)
            {
                //logger.Error($"Index Master for {code} of {targetDate.ToString("yyyy-MM-dd")} Validation Fail");
                if (makeReport == true)
                    comparator.MakeReport(srcMaster, destMaster, Path.Combine(logBasePath, Config.General.DateNow, compareResultPath, masterCompareResultPath, code + ".html"));
            }
            else
            {
                //logger.Info($"Index Master for {code} of {targetDate.ToString("yyyy-MM-dd")} Validation Success");
            }

            sw.Stop();
            //logger.Info($"Index Master Validation Done. Elapsed:{sw.Elapsed}");
            
            return result;
        }

        public bool ValidateStockMaster(DateTime targetDate, string code, bool makeReport = true)
        {
            //logger.Info("Stock Master Validation Start");

            Stopwatch sw = new Stopwatch();
            sw.Start();

            var filter = FilterFactory.Instance.BuildStockMasterFilter(targetDate);

            StockMaster srcMaster = source.Find(code, filter).FirstOrDefault();
            //logger.Info($"Get Stock Master for {code} from source done.");

            StockMaster destMaster = destination.Find(code, filter).FirstOrDefault();
            //logger.Info($"Get Stock Master for {code} from destination done.");

            bool result = comparator.DoCompareItem(srcMaster, destMaster, false);
            if (result == false )
            {
                //logger.Error($"Stock Master for {code} of {targetDate.ToString("yyyy-MM-dd")} Validation Fail");
                if (makeReport == true)
                    comparator.MakeReport(srcMaster, destMaster, Path.Combine(logBasePath, Config.General.DateNow, compareResultPath, masterCompareResultPath, code + ".html"));
            }
            else
            {
                //logger.Info($"Stock Master for {code} of {targetDate.ToString("yyyy-MM-dd")} Validation Success");
            }

            sw.Stop();
            //logger.Info($"Stock Master Validation Done. Elapsed:{sw.Elapsed}");

            return result;
        }

        public bool ValidateMasters(DateTime targetDate, bool makeReport = true)
        {
            logger.Info("Stock Masters Validation Start");

            Stopwatch sw = new Stopwatch();
            sw.Start();

            List<string> indexCodes = new List<string>();
            List<string> stockCodes = new List<string>();

            List<string> srcMasters = new List<string>();
            List<string> destMasters = new List<string>();

            indexCodes.AddRange(source.GetCollectionList(DbTypes.IndexMaster).OrderBy(o => o));
            stockCodes.AddRange(source.GetCollectionList(DbTypes.StockMaster).OrderBy(o => o));

            var indexFilter = FilterFactory.Instance.BuildIndexMasterFilter(targetDate);
            foreach (string code in indexCodes)
            {
                IndexMaster srcMaster = source.Find(code, indexFilter).FirstOrDefault();
                logger.Info($"Get Stock Master for {code} of {targetDate.ToString("yyyy-MM-dd")} from source done.");
                if (srcMaster != null)
                    srcMasters.Add(srcMaster.ToString(nameof(srcMaster.Id)));

                IndexMaster destMaster = destination.Find(code, indexFilter).FirstOrDefault();
                logger.Info($"Get Stock Master for {code} of {targetDate.ToString("yyyy-MM-dd")} from destination done.");
                if (destMaster != null)
                    destMasters.Add(destMaster.ToString(nameof(destMaster.Id)));
            }

            var stockFilter = FilterFactory.Instance.BuildStockMasterFilter(targetDate);
            foreach (string code in stockCodes)
            {
                StockMaster srcMaster = source.Find(code, stockFilter).FirstOrDefault();
                logger.Info($"Get Stock Master for {code} of {targetDate.ToString("yyyy-MM-dd")} from source done.");
                if (srcMaster != null)
                    srcMasters.Add(srcMaster.ToString(nameof(srcMaster.Id)));

                StockMaster destMaster = destination.Find(code, stockFilter).FirstOrDefault();
                logger.Info($"Get Stock Master for {code} of {targetDate.ToString("yyyy-MM-dd")} from destination done.");
                if (destMaster != null)
                    destMasters.Add(destMaster.ToString(nameof(destMaster.Id)));
            }
            
            bool result = comparator.DoCompareItem(srcMasters, destMasters, false);
            if (result == false)
            {
                logger.Error($"Stock Masters Validation Fail");
                if (makeReport == true)
                    comparator.MakeReport(srcMasters, destMasters, Path.Combine(logBasePath, Config.General.DateNow, compareResultPath, masterCompareResultPath));
            }
            else
            {
                logger.Info($"Stock Masters Validation Success");
            }

            sw.Stop();
            logger.Info($"Stock Master Validation Done. Elapsed:{sw.Elapsed}");

            return result;
        }
        
        public bool ValidateStockConclusions(DateTime targetDate, bool makeReport = true)
        {
            //logger.Info("Stock Conclusion Validation Start");
            Stopwatch sw = new Stopwatch();
            sw.Start();
            int cnt = 0;
            bool result = true;

            List<string> codeList = new List<string>();
            codeList.AddRange(source.GetCollectionList(DbTypes.StockMaster).OrderBy(o => o));

            var filter = FilterFactory.Instance.BuildStockConclusionFilter(targetDate);

            Parallel.ForEach(codeList, new ParallelOptions { MaxDegreeOfParallelism = Config.Validator.ThreadLimit }, code =>
            {
                var tasks = new List<Task>();

                List<Subscribable> sourceList = new List<Subscribable>();
                List<Subscribable> destinationList = new List<Subscribable>();

                tasks.Add(Task.Run(() => { sourceList.AddRange(source.Find(code, filter, o => o.Id).ToList()); }));
                tasks.Add(Task.Run(() => { destinationList.AddRange(destination.Find(code, filter, o => o.Id).ToList()); }));

                Task.WaitAll(tasks.ToArray());

                if (sourceList.Count != 0 || destinationList.Count != 0)
                {
                    if (sourceList.Count == 0)
                    {
                        logger.Error($"Conclusion list is empty in local db.");
                        result = false;
                    }
                    if (destinationList.Count == 0)
                    {
                        logger.Error($"Conclusion list is empty in remote db.");
                        result = false;
                    }
                    
                    if (comparator.DoCompareItem(sourceList, destinationList, false) == false)
                    {
                        //logger.Error($"Stock Conclusion Validation for {code} of {targetDate.ToString("yyyy-MM-dd")} Fail. {Interlocked.Increment(ref cnt)}/{codeList.Count}");
                        if (makeReport == true)
                            comparator.MakeReport(sourceList, destinationList, Path.Combine(logBasePath, Config.General.DateNow, compareResultPath, stockConclusionCompareResultPath, code + ".html"));
                        result = false;
                    }
                    else
                    {
                        //logger.Info($"Stock Conclusion Validation for {code} of {targetDate.ToString("yyyy-MM-dd")} success. {Interlocked.Increment(ref cnt)}/{codeList.Count}");
                    }
                }
                else
                {
                    logger.Info($"Stock Conclusion for {code} of {targetDate.ToString("yyyy-MM-dd")} is Empty. {Interlocked.Increment(ref cnt)}/{codeList.Count}");
                }

                sourceList.Clear();
                destinationList.Clear();

                GC.Collect();
                GC.WaitForPendingFinalizers();
            });

            codeList.Clear();

            sw.Stop();
            logger.Info($"Stock Conclusion Validation Done. Elapsed:{sw.Elapsed}");

            return result;
        }

        public bool ValidateStockConclusion(DateTime targetDate, string code, bool makeReport = true)
        {
            if (code == null)
            {
                logger.Warn("Code is empty");
                return false;
            }

            //logger.Info($"Stock Conclusion Validation for {code} Start");
            
            var tasks = new List<Task>();

            List<Subscribable> sourceList = new List<Subscribable>();
            List<Subscribable> destinationList = new List<Subscribable>();
            
            var filter = FilterFactory.Instance.BuildStockConclusionFilter(targetDate);
            
            tasks.Add(Task.Run(() => { sourceList.AddRange(source.Find(code, filter, o => o.Id).ToList()); }));
            tasks.Add(Task.Run(() => { destinationList.AddRange(destination.Find(code, filter, o => o.Id).ToList()); }));

            Task.WaitAll(tasks.ToArray());

            if (sourceList.Count != 0 || destinationList.Count != 0)
            {
                if (sourceList.Count == 0)
                    logger.Error($"Conclusion list is empty in local db.");
                if (destinationList.Count == 0)
                    logger.Error($"Conclusion list is empty in remote db.");

                if (comparator.DoCompareItem(sourceList, destinationList, false) == false)
                {
                    //logger.Error($"Stock Conclusion Validation for {code} of {targetDate.ToString("yyyy-MM-dd")} Fail.");
                    if (makeReport == true)
                        comparator.MakeReport(sourceList, destinationList, Path.Combine(logBasePath, Config.General.DateNow, compareResultPath, stockConclusionCompareResultPath, code + ".html"));
                    return false;
                }
                else
                {
                    //logger.Info($"Stock Conclusion Validation for {code} of {targetDate.ToString("yyyy-MM-dd")} success.");
                }
            }
            else
            {
                logger.Info($"Stock Conclusion for {code} is Empty.");
            }

            sourceList.Clear();
            destinationList.Clear();

            GC.Collect();
            GC.WaitForPendingFinalizers();

            return true;
        }

        public void ValidateStockConclusionWithDaishin(DateTime targetDate)
        {
            logger.Info("Stock Conclusion Validation with Daishin Start.");
            int cnt = 0;

            List<string> codeList = new List<string>();
            codeList.AddRange(source.GetCollectionList(DbTypes.StockMaster).OrderBy(o => o));

            DaishinCollector daishin = new DaishinCollector();

            foreach (string code in codeList)
            {
                var tasks = new List<Task>();

                List<Subscribable> sourceList = new List<Subscribable>();
                List<Subscribable> destinationList = new List<Subscribable>();

                var filter = FilterFactory.Instance.BuildStockConclusionFilter(targetDate, true);

                tasks.Add(Task.Run(() => { sourceList.AddRange(source.Find(code, filter, o => o.Id).ToList()); }));
                tasks.Add(Task.Run(() => { destinationList.AddRange(daishin.GetStockConclusions(targetDate, code, true)); }));

                Task.WaitAll(tasks.ToArray());
                
                if (sourceList.Count != 0 || destinationList.Count != 0)
                {
                    if (sourceList.Count == 0)
                        logger.Error($"Conclusion list is empty in local db.");
                    if (destinationList.Count == 0)
                        logger.Error($"Conclusion list is empty in Daishin.");

                    if (comparator.DoCompareItem(sourceList, destinationList, false) == false)
                    {
                        logger.Error($"Stock Conclusion Validation for {code} Fail. {cnt}/{codeList.Count}");
                        comparator.MakeReport(sourceList, destinationList, Path.Combine(logBasePath, Config.General.DateNow, compareResultPath, stockConclusionCompareResultPath, code + ".html"));
                    }
                    else
                    {
                        logger.Info($"Stock Conclusion Validation for {code} success. {cnt}/{codeList.Count}");
                    }
                }

                cnt++;

                sourceList.Clear();
                destinationList.Clear();

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            codeList.Clear();

            logger.Info("Stock Conclusion Validation with Daishin Done.");
        }

        public void ValidateSourceConclusionWithDaishin(string code)
        {
            ValidateConclusionWithDaishin(source, code);
        }

        public void ValidateDestinationConclusionWithDaishin(string code)
        {
            ValidateConclusionWithDaishin(destination, code);
        }

        public void ValidateConclusionWithDaishin(DbAgent dbCollector, string code)
        {
            logger.Info($"Stock Conclusion Validation for {code} with Daishin Start.");

            DaishinCollector daishin = new DaishinCollector();

            var tasks = new List<Task>();

            List<Subscribable> sourceList = new List<Subscribable>();
            List<Subscribable> destinationList = new List<Subscribable>();

            DateTime targetDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            var filter = FilterFactory.Instance.BuildStockConclusionFilter(targetDate, true);

            tasks.Add(Task.Run(() => { sourceList.AddRange(source.Find(code, filter, o => o.Id).ToList()); }));
            tasks.Add(Task.Run(() => { destinationList.AddRange(daishin.GetStockConclusions(targetDate, code, true)); }));

            Task.WaitAll(tasks.ToArray());

            if (sourceList.Count != 0 || destinationList.Count != 0)
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

            sourceList.Clear();
            destinationList.Clear();

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public bool ValidateIndexConclusions(DateTime targetDate, bool makeReport = true)
        {
            //logger.Info("Index Conclusion Validation Start.");

            bool result = true;

            Stopwatch sw = new Stopwatch();
            sw.Start();

            List<string> codeList = new List<string>();
            codeList.AddRange(source.GetCollectionList(DbTypes.IndexMaster).OrderBy(s => s));

            Parallel.ForEach(codeList, new ParallelOptions { MaxDegreeOfParallelism = Config.Validator.ThreadLimit }, code =>
            {
                var tasks = new List<Task>();

                List<Subscribable> sourceList = new List<Subscribable>();
                List<Subscribable> destinationList = new List<Subscribable>();

                var filter = FilterFactory.Instance.BuildIndexConclusionFilter(targetDate);

                tasks.Add(Task.Run(() => { sourceList.AddRange(source.Find(code, filter, o => o.Id).ToList()); }));
                tasks.Add(Task.Run(() => { destinationList.AddRange(destination.Find(code, filter, o => o.Id).ToList()); }));

                Task.WaitAll(tasks.ToArray());

                if (comparator.DoCompareItem(sourceList, destinationList, false) == false)
                {
                    //logger.Error($"Index Conclusion Validation for {code} of {targetDate.ToString("yyyy-MM-dd")} Fail.");
                    if (makeReport == true)
                        comparator.MakeReport(sourceList, destinationList, Path.Combine(logBasePath, Config.General.DateNow, compareResultPath, indexConclusionCompareResultPath, code + ".html"));
                    result = false;
                }

                sourceList.Clear();
                destinationList.Clear();

                GC.Collect();
                GC.WaitForPendingFinalizers();
            });

            codeList.Clear();

            sw.Stop();
            //logger.Info($"Index Conclusion Validation Done. Elapsed:{sw.Elapsed}");

            return result;
        }

        public bool ValidateIndexConclusion(DateTime targetDate, string code, bool makeReport = true)
        {
            if (code == null)
            {
                logger.Warn("Code is empty");
                return false;
            }

            //logger.Info($"Index Conclusion Validation for {code} Start.");

            var tasks = new List<Task>();

            List<Subscribable> sourceList = new List<Subscribable>();
            List<Subscribable> destinationList = new List<Subscribable>();

            try
            {
                var filter = FilterFactory.Instance.BuildIndexConclusionFilter(targetDate);
                tasks.Add(Task.Run(() => { sourceList.AddRange(source.Find(code, filter, o => o.Id).ToList()); }));
                tasks.Add(Task.Run(() => { destinationList.AddRange(destination.Find(code, filter, o => o.Id).ToList()); }));

                Task.WaitAll(tasks.ToArray());

                if (comparator.DoCompareItem(sourceList, destinationList, false) == false)
                {
                    //logger.Error($"Index Conclusion Validation for {code} of {targetDate.ToString("yyyy-MM-dd")} Fail.");
                    if (makeReport == true)
                        comparator.MakeReport(sourceList, destinationList, Path.Combine(logBasePath, Config.General.DateNow, compareResultPath, indexConclusionCompareResultPath, code + ".html"));
                    return false;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                sourceList.Clear();
                destinationList.Clear();

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            
            //logger.Info($"Index Conclusion Validation for {code} of {targetDate.ToString("yyyy-MM-dd")} Done.");
            return true;
        }

        public bool ValidateCircuitBreak(DateTime targetDate, string code, bool makeReport = true)
        {
            //logger.Info("Circuit Break Validation Start.");
            Stopwatch sw = new Stopwatch();
            sw.Start();
            
            var tasks = new List<Task>();

            List<Subscribable> sourceList = new List<Subscribable>();
            List<Subscribable> destinationList = new List<Subscribable>();

            try
            {
                var filter = FilterFactory.Instance.BuildCircuitBreakFilter(targetDate);

                tasks.Add(Task.Run(() => { sourceList.AddRange(source.Find(code, filter, o => o.Id).ToList()); }));
                tasks.Add(Task.Run(() => { destinationList.AddRange(destination.Find(code, filter, o => o.Id).ToList()); }));

                Task.WaitAll(tasks.ToArray());

                if (comparator.DoCompareItem(sourceList, destinationList, false) == false)
                {
                    //logger.Error($"Circuit Break Validation for {code} of {targetDate.ToString("yyyy-MM-dd")} Fail.");
                    if (makeReport == true)
                        comparator.MakeReport(sourceList, destinationList, Path.Combine(logBasePath, Config.General.DateNow, compareResultPath, circuitbreakCompareResultPath, code + ".html"));
                    return false;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                sourceList.Clear();
                destinationList.Clear();

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            sw.Stop();
            //logger.Info($"Circuit Break Validation for {code} of {targetDate.ToString("yyyy-MM-dd")}. Elapsed:{sw.Elapsed}");

            return true;
        }
        public bool ValidateCircuitBreaks(DateTime targetDate, bool makeReport = true)
        {
            //logger.Info("Circuit Break Validation Start.");
            Stopwatch sw = new Stopwatch();
            sw.Start();

            List<string> codeList = new List<string>();
            codeList.AddRange(source.GetCollectionList(DbTypes.IndexMaster).OrderBy(s => s));

            var tasks = new List<Task>();

            List<Subscribable> sourceList = new List<Subscribable>();
            List<Subscribable> destinationList = new List<Subscribable>();

            try
            {
	            var filter = FilterFactory.Instance.BuildCircuitBreakFilter(targetDate);

                foreach (string code in codeList)
                {
                    tasks.Add(Task.Run(() => { sourceList.AddRange(source.Find(code, filter, o => o.Id).ToList()); }));
                    tasks.Add(Task.Run(() => { destinationList.AddRange(destination.Find(code, filter, o => o.Id).ToList()); }));

                    Task.WaitAll(tasks.ToArray());
                }

                if (comparator.DoCompareItem(sourceList, destinationList, false) == false)
                {
                    //logger.Error("Circuit Break Validation Fail.");
                    if (makeReport == true)
                        comparator.MakeReport(sourceList, destinationList, Path.Combine(logBasePath, Config.General.DateNow, compareResultPath, circuitbreakCompareResultPath));
                    return false;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                sourceList.Clear();
                destinationList.Clear();

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            sw.Stop();
            //logger.Info($"Circuit Break Validation Done. Elapsed:{sw.Elapsed}");

            return true;
        }
    }
}
