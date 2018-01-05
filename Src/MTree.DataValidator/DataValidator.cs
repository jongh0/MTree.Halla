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
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private BeyondCompare Comparator { get; } = new BeyondCompare();

        private DbAgent Source { get; } = DbAgent.Instance;
        private DbAgent Destination { get; } = DbAgent.RemoteInstance;

        private const string LogBasePath = "Logs";
        private const string CompareResultPath = "CompareResult";
        private const string CodeCompareResultFile = "CodeCompare.html";
        private const string MasterCompareResultPath = "Master";
        private const string StockConclusionCompareResultPath = "StockConclusion";
        private const string IndexConclusionCompareResultPath = "IndexConclusion";
        private const string CircuitbreakCompareResultPath = "CircuitBreak";

        public DataValidator()
        {
        }

        public DataValidator(DbAgent src, DbAgent dest)
        {
            Source = src;
            Destination = dest;
        }

        public bool ValidateCodeList()
        {
            _logger.Info("Code List Validation Start");
            List<string> srcCodes = new List<string>();
            srcCodes.AddRange(Source.GetCollectionList(DbTypes.IndexMaster).OrderBy(s => s));
            srcCodes.AddRange(Source.GetCollectionList(DbTypes.StockMaster).OrderBy(s => s));
            
            List<string> destCodes = new List<string>();
            destCodes.AddRange(Destination.GetCollectionList(DbTypes.IndexMaster).OrderBy(s => s));
            destCodes.AddRange(Destination.GetCollectionList(DbTypes.StockMaster).OrderBy(s => s));
            
            bool result = Comparator.DoCompareItem(srcCodes, destCodes, false);
            Comparator.MakeReport(srcCodes, destCodes, Path.Combine(LogBasePath, Config.General.DateNow, CompareResultPath, CodeCompareResultFile));

            if (result == true)
                _logger.Info($"Code List Validation Success");
            else
                _logger.Error($"Code List Validation Fail");

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
            //_logger.Info("Index Master Validation Start");

            Stopwatch sw = new Stopwatch();
            sw.Start();

            var filter = FilterFactory.Instance.BuildIndexMasterFilter(targetDate);

            IndexMaster srcMaster = Source.Find(code, filter).FirstOrDefault();
            //_logger.Info($"Get Index Master for {code} from source done.");

            IndexMaster destMaster = Destination.Find(code, filter).FirstOrDefault();
            //_logger.Info($"Get Index Master for {code} from destination done.");

            bool result = Comparator.DoCompareItem(srcMaster, destMaster, false);
            if (result == false)
            {
                //_logger.Error($"Index Master for {code} of {targetDate.ToString("yyyy-MM-dd")} Validation Fail");
                if (makeReport == true)
                    Comparator.MakeReport(srcMaster, destMaster, Path.Combine(LogBasePath, Config.General.DateNow, CompareResultPath, MasterCompareResultPath, code + ".html"));
            }
            else
            {
                //_logger.Info($"Index Master for {code} of {targetDate.ToString("yyyy-MM-dd")} Validation Success");
            }

            sw.Stop();
            //_logger.Info($"Index Master Validation Done. Elapsed:{sw.Elapsed}");
            
            return result;
        }

        public bool ValidateStockMaster(DateTime targetDate, string code, bool makeReport = true)
        {
            //_logger.Info("Stock Master Validation Start");

            Stopwatch sw = new Stopwatch();
            sw.Start();

            var filter = FilterFactory.Instance.BuildStockMasterFilter(targetDate);

            StockMaster srcMaster = Source.Find(code, filter).FirstOrDefault();
            //_logger.Info($"Get Stock Master for {code} from source done.");

            StockMaster destMaster = Destination.Find(code, filter).FirstOrDefault();
            //_logger.Info($"Get Stock Master for {code} from destination done.");

            bool result = Comparator.DoCompareItem(srcMaster, destMaster, false);
            if (result == false )
            {
                //_logger.Error($"Stock Master for {code} of {targetDate.ToString("yyyy-MM-dd")} Validation Fail");
                if (makeReport == true)
                    Comparator.MakeReport(srcMaster, destMaster, Path.Combine(LogBasePath, Config.General.DateNow, CompareResultPath, MasterCompareResultPath, code + ".html"));
            }
            else
            {
                //_logger.Info($"Stock Master for {code} of {targetDate.ToString("yyyy-MM-dd")} Validation Success");
            }

            sw.Stop();
            //_logger.Info($"Stock Master Validation Done. Elapsed:{sw.Elapsed}");

            return result;
        }

        public bool ValidateMasters(DateTime targetDate, bool makeReport = true)
        {
            _logger.Info("Stock Masters Validation Start");

            Stopwatch sw = new Stopwatch();
            sw.Start();

            List<string> indexCodes = new List<string>();
            List<string> stockCodes = new List<string>();

            List<string> srcMasters = new List<string>();
            List<string> destMasters = new List<string>();

            indexCodes.AddRange(Source.GetCollectionList(DbTypes.IndexMaster).OrderBy(o => o));
            stockCodes.AddRange(Source.GetCollectionList(DbTypes.StockMaster).OrderBy(o => o));

            var indexFilter = FilterFactory.Instance.BuildIndexMasterFilter(targetDate);
            foreach (string code in indexCodes)
            {
                IndexMaster srcMaster = Source.Find(code, indexFilter).FirstOrDefault();
                _logger.Info($"Get Stock Master for {code} of {targetDate.ToString("yyyy-MM-dd")} from source done.");
                if (srcMaster != null)
                    srcMasters.Add(srcMaster.ToString(nameof(srcMaster.Id)));

                IndexMaster destMaster = Destination.Find(code, indexFilter).FirstOrDefault();
                _logger.Info($"Get Stock Master for {code} of {targetDate.ToString("yyyy-MM-dd")} from destination done.");
                if (destMaster != null)
                    destMasters.Add(destMaster.ToString(nameof(destMaster.Id)));
            }

            var stockFilter = FilterFactory.Instance.BuildStockMasterFilter(targetDate);
            foreach (string code in stockCodes)
            {
                StockMaster srcMaster = Source.Find(code, stockFilter).FirstOrDefault();
                _logger.Info($"Get Stock Master for {code} of {targetDate.ToString("yyyy-MM-dd")} from source done.");
                if (srcMaster != null)
                    srcMasters.Add(srcMaster.ToString(nameof(srcMaster.Id)));

                StockMaster destMaster = Destination.Find(code, stockFilter).FirstOrDefault();
                _logger.Info($"Get Stock Master for {code} of {targetDate.ToString("yyyy-MM-dd")} from destination done.");
                if (destMaster != null)
                    destMasters.Add(destMaster.ToString(nameof(destMaster.Id)));
            }
            
            bool result = Comparator.DoCompareItem(srcMasters, destMasters, false);
            if (result == false)
            {
                _logger.Error($"Stock Masters Validation Fail");
                if (makeReport == true)
                    Comparator.MakeReport(srcMasters, destMasters, Path.Combine(LogBasePath, Config.General.DateNow, CompareResultPath, MasterCompareResultPath));
            }
            else
            {
                _logger.Info($"Stock Masters Validation Success");
            }

            sw.Stop();
            _logger.Info($"Stock Master Validation Done. Elapsed:{sw.Elapsed}");

            return result;
        }
        
        public bool ValidateStockConclusions(DateTime targetDate, bool makeReport = true)
        {
            //_logger.Info("Stock Conclusion Validation Start");
            Stopwatch sw = new Stopwatch();
            sw.Start();
            int cnt = 0;
            bool result = true;

            List<string> codeList = new List<string>();
            codeList.AddRange(Source.GetCollectionList(DbTypes.StockMaster).OrderBy(o => o));

            var filter = FilterFactory.Instance.BuildStockConclusionFilter(targetDate);

            Parallel.ForEach(codeList, new ParallelOptions { MaxDegreeOfParallelism = Config.Validator.ThreadLimit }, code =>
            {
                var tasks = new List<Task>();

                List<Subscribable> sourceList = new List<Subscribable>();
                List<Subscribable> destinationList = new List<Subscribable>();

                tasks.Add(Task.Run(() => { sourceList.AddRange(Source.Find(code, filter, o => o.Id).ToList()); }));
                tasks.Add(Task.Run(() => { destinationList.AddRange(Destination.Find(code, filter, o => o.Id).ToList()); }));

                Task.WaitAll(tasks.ToArray());

                if (sourceList.Count != 0 || destinationList.Count != 0)
                {
                    if (sourceList.Count == 0)
                    {
                        _logger.Error($"Conclusion list is empty in local db.");
                        result = false;
                    }
                    if (destinationList.Count == 0)
                    {
                        _logger.Error($"Conclusion list is empty in remote db.");
                        result = false;
                    }
                    
                    if (Comparator.DoCompareItem(sourceList, destinationList, false) == false)
                    {
                        //_logger.Error($"Stock Conclusion Validation for {code} of {targetDate.ToString("yyyy-MM-dd")} Fail. {Interlocked.Increment(ref cnt)}/{codeList.Count}");
                        if (makeReport == true)
                            Comparator.MakeReport(sourceList, destinationList, Path.Combine(LogBasePath, Config.General.DateNow, CompareResultPath, StockConclusionCompareResultPath, code + ".html"));
                        result = false;
                    }
                    else
                    {
                        //_logger.Info($"Stock Conclusion Validation for {code} of {targetDate.ToString("yyyy-MM-dd")} success. {Interlocked.Increment(ref cnt)}/{codeList.Count}");
                    }
                }
                else
                {
                    _logger.Info($"Stock Conclusion for {code} of {targetDate.ToString("yyyy-MM-dd")} is Empty. {Interlocked.Increment(ref cnt)}/{codeList.Count}");
                }

                sourceList.Clear();
                destinationList.Clear();

                GC.Collect();
                GC.WaitForPendingFinalizers();
            });

            codeList.Clear();

            sw.Stop();
            _logger.Info($"Stock Conclusion Validation Done. Elapsed:{sw.Elapsed}");

            return result;
        }

        public bool ValidateStockConclusion(DateTime targetDate, string code, bool makeReport = true)
        {
            if (code == null)
            {
                _logger.Warn("Code is empty");
                return false;
            }

            //_logger.Info($"Stock Conclusion Validation for {code} Start");
            
            var tasks = new List<Task>();

            List<Subscribable> sourceList = new List<Subscribable>();
            List<Subscribable> destinationList = new List<Subscribable>();
            
            var filter = FilterFactory.Instance.BuildStockConclusionFilter(targetDate);
            
            tasks.Add(Task.Run(() => { sourceList.AddRange(Source.Find(code, filter, o => o.Id).ToList()); }));
            tasks.Add(Task.Run(() => { destinationList.AddRange(Destination.Find(code, filter, o => o.Id).ToList()); }));

            Task.WaitAll(tasks.ToArray());

            if (sourceList.Count != 0 || destinationList.Count != 0)
            {
                if (sourceList.Count == 0)
                    _logger.Error($"Conclusion list is empty in local db.");
                if (destinationList.Count == 0)
                    _logger.Error($"Conclusion list is empty in remote db.");

                if (Comparator.DoCompareItem(sourceList, destinationList, false) == false)
                {
                    //_logger.Error($"Stock Conclusion Validation for {code} of {targetDate.ToString("yyyy-MM-dd")} Fail.");
                    if (makeReport == true)
                        Comparator.MakeReport(sourceList, destinationList, Path.Combine(LogBasePath, Config.General.DateNow, CompareResultPath, StockConclusionCompareResultPath, code + ".html"));
                    return false;
                }
                else
                {
                    //_logger.Info($"Stock Conclusion Validation for {code} of {targetDate.ToString("yyyy-MM-dd")} success.");
                }
            }
            else
            {
                _logger.Info($"Stock Conclusion for {code} is Empty.");
            }

            sourceList.Clear();
            destinationList.Clear();

            GC.Collect();
            GC.WaitForPendingFinalizers();

            return true;
        }

        public void ValidateStockConclusionWithDaishin(DateTime targetDate)
        {
            _logger.Info("Stock Conclusion Validation with Daishin Start.");
            int cnt = 0;

            List<string> codeList = new List<string>();
            codeList.AddRange(Source.GetCollectionList(DbTypes.StockMaster).OrderBy(o => o));

            DaishinCollector daishin = new DaishinCollector();

            foreach (string code in codeList)
            {
                var tasks = new List<Task>();

                List<Subscribable> sourceList = new List<Subscribable>();
                List<Subscribable> destinationList = new List<Subscribable>();

                var filter = FilterFactory.Instance.BuildStockConclusionFilter(targetDate, true);

                tasks.Add(Task.Run(() => { sourceList.AddRange(Source.Find(code, filter, o => o.Id).ToList()); }));
                tasks.Add(Task.Run(() => { destinationList.AddRange(daishin.GetStockConclusions(targetDate, code, true)); }));

                Task.WaitAll(tasks.ToArray());
                
                if (sourceList.Count != 0 || destinationList.Count != 0)
                {
                    if (sourceList.Count == 0)
                        _logger.Error($"Conclusion list is empty in local db.");
                    if (destinationList.Count == 0)
                        _logger.Error($"Conclusion list is empty in Daishin.");

                    if (Comparator.DoCompareItem(sourceList, destinationList, false) == false)
                    {
                        _logger.Error($"Stock Conclusion Validation for {code} Fail. {cnt}/{codeList.Count}");
                        Comparator.MakeReport(sourceList, destinationList, Path.Combine(LogBasePath, Config.General.DateNow, CompareResultPath, StockConclusionCompareResultPath, code + ".html"));
                    }
                    else
                    {
                        _logger.Info($"Stock Conclusion Validation for {code} success. {cnt}/{codeList.Count}");
                    }
                }

                cnt++;

                sourceList.Clear();
                destinationList.Clear();

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            codeList.Clear();

            _logger.Info("Stock Conclusion Validation with Daishin Done.");
        }

        public void ValidateSourceConclusionWithDaishin(string code)
        {
            ValidateConclusionWithDaishin(Source, code);
        }

        public void ValidateDestinationConclusionWithDaishin(string code)
        {
            ValidateConclusionWithDaishin(Destination, code);
        }

        public void ValidateConclusionWithDaishin(DbAgent dbCollector, string code)
        {
            _logger.Info($"Stock Conclusion Validation for {code} with Daishin Start.");

            DaishinCollector daishin = new DaishinCollector();

            var tasks = new List<Task>();

            List<Subscribable> sourceList = new List<Subscribable>();
            List<Subscribable> destinationList = new List<Subscribable>();

            DateTime targetDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            var filter = FilterFactory.Instance.BuildStockConclusionFilter(targetDate, true);

            tasks.Add(Task.Run(() => { sourceList.AddRange(Source.Find(code, filter, o => o.Id).ToList()); }));
            tasks.Add(Task.Run(() => { destinationList.AddRange(daishin.GetStockConclusions(targetDate, code, true)); }));

            Task.WaitAll(tasks.ToArray());

            if (sourceList.Count != 0 || destinationList.Count != 0)
            {
                if (sourceList.Count == 0)
                    _logger.Error($"Conclusion list is empty in local db.");
                if (destinationList.Count == 0)
                    _logger.Error($"Conclusion list is empty in Daishin.");

                if (Comparator.DoCompareItem(sourceList, destinationList, false) == false)
                {
                    _logger.Error($"Stock Conclusion Validation for {code} Fail.");
                    Comparator.MakeReport(sourceList, destinationList, Path.Combine(LogBasePath, Config.General.DateNow, CompareResultPath, StockConclusionCompareResultPath, code + ".html"));
                }
                else
                {
                    _logger.Info($"Stock Conclusion Validation for {code} success.");
                }

            }

            _logger.Info($"Stock Conclusion Validation for {code} with Daishin Done.");

            sourceList.Clear();
            destinationList.Clear();

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public bool ValidateIndexConclusions(DateTime targetDate, bool makeReport = true)
        {
            //_logger.Info("Index Conclusion Validation Start.");

            bool result = true;

            Stopwatch sw = new Stopwatch();
            sw.Start();

            List<string> codeList = new List<string>();
            codeList.AddRange(Source.GetCollectionList(DbTypes.IndexMaster).OrderBy(s => s));

            Parallel.ForEach(codeList, new ParallelOptions { MaxDegreeOfParallelism = Config.Validator.ThreadLimit }, code =>
            {
                var tasks = new List<Task>();

                List<Subscribable> sourceList = new List<Subscribable>();
                List<Subscribable> destinationList = new List<Subscribable>();

                var filter = FilterFactory.Instance.BuildIndexConclusionFilter(targetDate);

                tasks.Add(Task.Run(() => { sourceList.AddRange(Source.Find(code, filter, o => o.Id).ToList()); }));
                tasks.Add(Task.Run(() => { destinationList.AddRange(Destination.Find(code, filter, o => o.Id).ToList()); }));

                Task.WaitAll(tasks.ToArray());

                if (Comparator.DoCompareItem(sourceList, destinationList, false) == false)
                {
                    //_logger.Error($"Index Conclusion Validation for {code} of {targetDate.ToString("yyyy-MM-dd")} Fail.");
                    if (makeReport == true)
                        Comparator.MakeReport(sourceList, destinationList, Path.Combine(LogBasePath, Config.General.DateNow, CompareResultPath, IndexConclusionCompareResultPath, code + ".html"));
                    result = false;
                }

                sourceList.Clear();
                destinationList.Clear();

                GC.Collect();
                GC.WaitForPendingFinalizers();
            });

            codeList.Clear();

            sw.Stop();
            //_logger.Info($"Index Conclusion Validation Done. Elapsed:{sw.Elapsed}");

            return result;
        }

        public bool ValidateIndexConclusion(DateTime targetDate, string code, bool makeReport = true)
        {
            if (code == null)
            {
                _logger.Warn("Code is empty");
                return false;
            }

            //_logger.Info($"Index Conclusion Validation for {code} Start.");

            var tasks = new List<Task>();

            List<Subscribable> sourceList = new List<Subscribable>();
            List<Subscribable> destinationList = new List<Subscribable>();

            try
            {
                var filter = FilterFactory.Instance.BuildIndexConclusionFilter(targetDate);
                tasks.Add(Task.Run(() => { sourceList.AddRange(Source.Find(code, filter, o => o.Id).ToList()); }));
                tasks.Add(Task.Run(() => { destinationList.AddRange(Destination.Find(code, filter, o => o.Id).ToList()); }));

                Task.WaitAll(tasks.ToArray());

                if (Comparator.DoCompareItem(sourceList, destinationList, false) == false)
                {
                    //_logger.Error($"Index Conclusion Validation for {code} of {targetDate.ToString("yyyy-MM-dd")} Fail.");
                    if (makeReport == true)
                        Comparator.MakeReport(sourceList, destinationList, Path.Combine(LogBasePath, Config.General.DateNow, CompareResultPath, IndexConclusionCompareResultPath, code + ".html"));
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            finally
            {
                sourceList.Clear();
                destinationList.Clear();

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            
            //_logger.Info($"Index Conclusion Validation for {code} of {targetDate.ToString("yyyy-MM-dd")} Done.");
            return true;
        }

        public bool ValidateCircuitBreak(DateTime targetDate, string code, bool makeReport = true)
        {
            //_logger.Info("Circuit Break Validation Start.");
            Stopwatch sw = new Stopwatch();
            sw.Start();
            
            var tasks = new List<Task>();

            List<Subscribable> sourceList = new List<Subscribable>();
            List<Subscribable> destinationList = new List<Subscribable>();

            try
            {
                var filter = FilterFactory.Instance.BuildCircuitBreakFilter(targetDate);

                tasks.Add(Task.Run(() => { sourceList.AddRange(Source.Find(code, filter, o => o.Id).ToList()); }));
                tasks.Add(Task.Run(() => { destinationList.AddRange(Destination.Find(code, filter, o => o.Id).ToList()); }));

                Task.WaitAll(tasks.ToArray());

                if (Comparator.DoCompareItem(sourceList, destinationList, false) == false)
                {
                    //_logger.Error($"Circuit Break Validation for {code} of {targetDate.ToString("yyyy-MM-dd")} Fail.");
                    if (makeReport == true)
                        Comparator.MakeReport(sourceList, destinationList, Path.Combine(LogBasePath, Config.General.DateNow, CompareResultPath, CircuitbreakCompareResultPath, code + ".html"));
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            finally
            {
                sourceList.Clear();
                destinationList.Clear();

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            sw.Stop();
            //_logger.Info($"Circuit Break Validation for {code} of {targetDate.ToString("yyyy-MM-dd")}. Elapsed:{sw.Elapsed}");

            return true;
        }
        public bool ValidateCircuitBreaks(DateTime targetDate, bool makeReport = true)
        {
            //_logger.Info("Circuit Break Validation Start.");
            Stopwatch sw = new Stopwatch();
            sw.Start();

            List<string> codeList = new List<string>();
            codeList.AddRange(Source.GetCollectionList(DbTypes.IndexMaster).OrderBy(s => s));

            var tasks = new List<Task>();

            List<Subscribable> sourceList = new List<Subscribable>();
            List<Subscribable> destinationList = new List<Subscribable>();

            try
            {
	            var filter = FilterFactory.Instance.BuildCircuitBreakFilter(targetDate);

                foreach (string code in codeList)
                {
                    tasks.Add(Task.Run(() => { sourceList.AddRange(Source.Find(code, filter, o => o.Id).ToList()); }));
                    tasks.Add(Task.Run(() => { destinationList.AddRange(Destination.Find(code, filter, o => o.Id).ToList()); }));

                    Task.WaitAll(tasks.ToArray());
                }

                if (Comparator.DoCompareItem(sourceList, destinationList, false) == false)
                {
                    //_logger.Error("Circuit Break Validation Fail.");
                    if (makeReport == true)
                        Comparator.MakeReport(sourceList, destinationList, Path.Combine(LogBasePath, Config.General.DateNow, CompareResultPath, CircuitbreakCompareResultPath));
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            finally
            {
                sourceList.Clear();
                destinationList.Clear();

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            sw.Stop();
            //_logger.Info($"Circuit Break Validation Done. Elapsed:{sw.Elapsed}");

            return true;
        }
    }
}
