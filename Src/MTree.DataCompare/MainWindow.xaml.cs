using MongoDB.Driver;
using MTree.DataStructure;
using MTree.DbProvider;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MTree.DataCompare
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            DateTime target = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 12);
            DataValidator validator = new DataValidator();
            validator.ValidateCodeList();

            //DateTime target = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 12);
            //TestMasterCompare(target);
            //TestDbToDbStockConclusionCompare("123440",target);
            //TestDbToDaishinStockConclusionCompare("123440", target);
            //TestDbToDbStockConclusionCompare(target);
            //TestCodeListCompare();

            //string code = "005930";
            //DateTime target = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 12);
            //TestDbToDbStockConclusionCompare(code, target);
        }

        private void TestCodeListCompare()
        {
            BeyondCompare compare = new BeyondCompare();

            IDataCollector source = new DbCollector(DbAgent.Instance);
            IDataCollector destination = new DbCollector(DbAgent.RemoteInstance);
            
            List<string> srcCodes = source.GetCodeList();
            List<string> destCodes = destination.GetCodeList();
            bool result = compare.DoCompareItem(srcCodes, destCodes, true);
        }
        private void TestMasterCompare(string code, DateTime target)
        {
            BeyondCompare compare = new BeyondCompare();

            IDataCollector source = new DbCollector(DbAgent.Instance);
            IDataCollector destination = new DbCollector(DbAgent.RemoteInstance);

            StockMaster srcMaster = source.GetMaster(code, target);
            StockMaster destMaster = destination.GetMaster(code, target);
            bool result = compare.DoCompareItem(srcMaster, destMaster, true);
        }

        private void TestMasterCompare(DateTime target)
        {
            BeyondCompare compare = new BeyondCompare();

            IDataCollector source = new DbCollector(DbAgent.Instance);
            IDataCollector destination = new DbCollector(DbAgent.RemoteInstance);

            foreach (string code in source.GetCodeList())
            {
                StockMaster srcMaster = source.GetMaster(code, target);
                StockMaster destMaster = destination.GetMaster(code, target);
                bool result = compare.DoCompareItem(srcMaster, destMaster, false);
                Console.WriteLine($"Code:{code}, Result:{result}");
            }
        }

        private void TestDbToDbStockConclusionCompare(DateTime target)
        {
            BeyondCompare compare = new BeyondCompare();

            IDataCollector source = new DbCollector(DbAgent.Instance);
            IDataCollector destination = new DbCollector(DbAgent.RemoteInstance);
            
            foreach (string code in source.GetCodeList())
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();

                var tasks = new List<Task>();

                List<Subscribable> sourceList = new List<Subscribable>();
                List<Subscribable> destinationList = new List<Subscribable>();

                tasks.Add(Task.Run(() => { sourceList = source.GetStockConclusions(code, target, false); }));
                tasks.Add(Task.Run(() => { destinationList = destination.GetStockConclusions(code, target, false); }));

                Task.WaitAll(tasks.ToArray());

                bool result = compare.DoCompareItem(sourceList, destinationList, false);
                sw.Stop();

                Console.WriteLine($"Code:{code}, Result:{result}, Elapsed:{sw.Elapsed}");
                if (result == false)
                {
                    compare.DoCompareItem(sourceList, destinationList, true);
                }
            }
        }

        private void TestDbToDbStockConclusionCompare(string code, DateTime target)
        {
            BeyondCompare compare = new BeyondCompare();
            
            IDataCollector source = new DbCollector(DbAgent.Instance);
            IDataCollector destination = new DbCollector(DbAgent.RemoteInstance);

            Stopwatch sw = new Stopwatch();
            sw.Start();

            var tasks = new List<Task>();

            List<Subscribable> sourceList = new List<Subscribable>();
            List<Subscribable> destinationList = new List<Subscribable>();

            tasks.Add(Task.Run(() => { sourceList = source.GetStockConclusions(code, target); }));
            tasks.Add(Task.Run(() => { destinationList = destination.GetStockConclusions(code, target); }));

            Task.WaitAll(tasks.ToArray());

            bool result = compare.DoCompareItem(sourceList, destinationList, true);
            sw.Stop();

            Console.WriteLine($"Code:{code}, Result:{result}, Elapsed:{sw.Elapsed}");
        }

        private void TestDbToDaishinStockConclusionCompare(string code, DateTime target)
        {
            BeyondCompare compare = new BeyondCompare();

            IDataCollector source = new DbCollector(DbAgent.Instance);
            IDataCollector destination = new DaishinCollector();

            Stopwatch sw = new Stopwatch();
            sw.Start();

            var tasks = new List<Task>();

            List<Subscribable> sourceList = new List<Subscribable>();
            List<Subscribable> destinationList = new List<Subscribable>();

            tasks.Add(Task.Run(() => { sourceList = source.GetStockConclusions(code, target); }));
            tasks.Add(Task.Run(() => { destinationList = destination.GetStockConclusions(code, target); }));

            Task.WaitAll(tasks.ToArray());

            bool result = compare.DoCompareItem(sourceList, destinationList, true);
            sw.Stop();

            Console.WriteLine($"Code:{code}, Result:{result}, Elapsed:{sw.Elapsed}");
        }
    }
}
