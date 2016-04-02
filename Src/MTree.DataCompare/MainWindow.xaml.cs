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

            BeyondCompare compare = new BeyondCompare();

            DataCollector source = new DataCollector(DbAgent.Instance);
            DataCollector destination = new DataCollector(DbAgent.RemoteInstance);

            Stopwatch swTotal = new Stopwatch();

            swTotal.Start();
            Console.WriteLine(compare.DoCompareItem(source.GetCodeList(), destination.GetCodeList()));

            DateTime target = new DateTime(2016, 03, 31);
            foreach (string code in source.GetCodeList())
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                bool result = compare.DoCompareItem(source.GetMaster(code, target), destination.GetMaster(code, target));
                sw.Stop();
                Console.WriteLine($"Code:{code}, Elapsed:{sw.Elapsed}");
                if (result == false)
                {
                    Console.WriteLine(result);
                }
            }
            swTotal.Stop();
            Console.WriteLine($"Done. Elapsed:{swTotal.Elapsed}");
        }
    }
}
