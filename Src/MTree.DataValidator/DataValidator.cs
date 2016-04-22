﻿using MTree.DataStructure;
using MTree.DbProvider;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.DataValidator
{
    public class DataValidator
    {
        private BeyondCompare comparator { get; set; } = new BeyondCompare();

        private IDataCollector source = new DbCollector(DbAgent.Instance);
        private IDataCollector destination = new DbCollector(DbAgent.RemoteInstance);

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
            List<string> srcCodes = new List<string>();
            srcCodes.AddRange(source.GetStockCodeList());
            srcCodes.AddRange(source.GetIndexCodeList());

            List<string> destCodes = new List<string>();
            destCodes.AddRange(destination.GetStockCodeList());
            destCodes.AddRange(destination.GetIndexCodeList());

            bool result = comparator.DoCompareItem(srcCodes, destCodes, false);
            comparator.MakeReport(srcCodes, destCodes, Path.Combine(compareResultPath, $"{DateTime.Now.Year}{DateTime.Now.Month}{DateTime.Now.Day}", codeCompareResultFile));
            return result;
        }

        public void ValidateMasterCompare(DateTime target)
        {
            List<string> srcMasters = new List<string>();
            List<string> destMasters = new List<string>();

            foreach (string code in source.GetStockCodeList())
            {
                StockMaster srcMaster = source.GetMaster(code, target);
                if(srcMaster != null)
                    srcMasters.Add(srcMaster.ToString());

                StockMaster destMaster = destination.GetMaster(code, target);
                if (destMaster != null)
                    destMasters.Add(destMaster.ToString());
            }

            if (comparator.DoCompareItem(srcMasters, destMasters, false) == false)
            {
                comparator.MakeReport(srcMasters, destMasters, Path.Combine(compareResultPath, $"{DateTime.Now.Year}{DateTime.Now.Month}{DateTime.Now.Day}", masterCompareResultFile));
            }
        }
        
        public void ValidateStockConclusionCompare(DateTime target)
        {
            foreach (string code in source.GetStockCodeList())
            {
                var tasks = new List<Task>();

                List<Subscribable> sourceList = new List<Subscribable>();
                List<Subscribable> destinationList = new List<Subscribable>();

                tasks.Add(Task.Run(() => { sourceList = source.GetStockConclusions(code, target, false); }));
                tasks.Add(Task.Run(() => { destinationList = destination.GetStockConclusions(code, target, false); }));

                Task.WaitAll(tasks.ToArray());

                if (sourceList.Count != 0 && destinationList.Count != 0)
                {
                    bool result = comparator.DoCompareItem(sourceList, destinationList, false);

                    Console.WriteLine($"Code:{code}, Result:{result}");
                    if (result == false)
                    {
                        comparator.MakeReport(sourceList, destinationList, Path.Combine(compareResultPath, $"{DateTime.Now.Year}{DateTime.Now.Month}{DateTime.Now.Day}", stockConclusionCompareResultPath, code + ".html"));
                    }
                }
            }
        }

        public void ValidateStockConclusionCompareWithDaishin(DateTime target)
        {
            int cnt = 0;
            IDataCollector daishin = new DaishinCollector();

            foreach (string code in source.GetStockCodeList())
            {
                var tasks = new List<Task>();

                List<Subscribable> sourceList = new List<Subscribable>();
                List<Subscribable> destinationList = new List<Subscribable>();

                tasks.Add(Task.Run(() => { sourceList = source.GetStockConclusions(code, target, true); }));
                tasks.Add(Task.Run(() => { destinationList = daishin.GetStockConclusions(code, target, true); }));

                Task.WaitAll(tasks.ToArray());

                if (sourceList.Count != 0 && destinationList.Count != 0)
                {
                    bool result = comparator.DoCompareItem(sourceList, destinationList, false);

                    Console.WriteLine($"{cnt}/{source.GetStockCodeList().Count} Code:{code}, Result:{result}");
                    if (result == false)
                    {
                        comparator.MakeReport(sourceList, destinationList, Path.Combine(compareResultPath, $"{DateTime.Now.Year}{DateTime.Now.Month}{DateTime.Now.Day}", stockConclusionCompareResultPath, code + ".html"));
                    }
                }

                cnt++;
            }
        }

        public void ValidateIndexConclusionCompare(DateTime target)
        {
            foreach (string code in source.GetIndexCodeList())
            {
                var tasks = new List<Task>();

                List<Subscribable> sourceList = new List<Subscribable>();
                List<Subscribable> destinationList = new List<Subscribable>();

                tasks.Add(Task.Run(() => { sourceList = source.GetIndexConclusions(code, target, false); }));
                tasks.Add(Task.Run(() => { destinationList = destination.GetIndexConclusions(code, target, false); }));

                Task.WaitAll(tasks.ToArray());

                bool result = comparator.DoCompareItem(sourceList, destinationList, false);

                Console.WriteLine($"Code:{code}, Result:{result}");

                if (result == false)
                {
                    comparator.MakeReport(sourceList, destinationList, Path.Combine(compareResultPath, $"{DateTime.Now.Year}{DateTime.Now.Month}{DateTime.Now.Day}", indexConclusionCompareResultPath, code + ".html"));
                }
            }
        }
        public void ValidateCircuitBreakCompare(DateTime target)
        {
            var tasks = new List<Task>();

            List<Subscribable> sourceList = new List<Subscribable>();
            List<Subscribable> destinationList = new List<Subscribable>();

            foreach (string code in source.GetStockCodeList())
            {
                tasks.Add(Task.Run(() => { sourceList.AddRange(source.GetCircuitBreaks(code, target)); }));
                tasks.Add(Task.Run(() => { destinationList.AddRange(destination.GetCircuitBreaks(code, target)); }));
                
                Task.WaitAll(tasks.ToArray());
            }

            bool result = comparator.DoCompareItem(sourceList, destinationList, true);

            Console.WriteLine($"Result:{result}");

            if (result == false)
            {
                comparator.MakeReport(sourceList, destinationList, Path.Combine(compareResultPath, $"{DateTime.Now.Year}{DateTime.Now.Month}{DateTime.Now.Day}", circuitbreakCompareResultFile));
            }
        }
    }
}