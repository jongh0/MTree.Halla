using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace MTree.Dashboard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Dashboard Consumer { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            Consumer = new Dashboard();
            //TestData();

            this.DataContext = Consumer;
        }

        private void TestData()
        {
            Consumer.StockItems.Add("000020", new DashboardItem() { Code = "000020", Name = "삼양1", Price = 2000, Volume = 60, BasisPrice = 2000, PreviousVolume = 50 });
            Consumer.StockItems.Add("000030", new DashboardItem() { Code = "000030", Name = "삼양2", Price = (float)102.20, Volume = 243, BasisPrice = (float)101.20, PreviousVolume = 176750 });
            Consumer.StockItems.Add("000040", new DashboardItem() { Code = "000040", Name = "삼양3", Price = 55, Volume = 5435, BasisPrice = 50, PreviousVolume = 7576, CircuitBreakType = DataStructure.CircuitBreakTypes.StaticInvoke });
            Consumer.StockItems.Add("000050", new DashboardItem() { Code = "000050", Name = "삼양4", Price = 243400, Volume = 423, BasisPrice = 244400, PreviousVolume = 10656 });
            Consumer.StockItems.Add("000060", new DashboardItem() { Code = "000060", Name = "삼양5", Price = 12200, Volume = 510, BasisPrice = 11200, PreviousVolume = 234, CircuitBreakType = DataStructure.CircuitBreakTypes.DynamicInvoke });

            Consumer.IndexItems.Add("000040", new DashboardItem() { Code = "000040", Name = "삼양3", Price = 50, Volume = 5435, BasisPrice = 46, PreviousVolume = 7576 });
            Consumer.IndexItems.Add("000050", new DashboardItem() { Code = "000050", Name = "삼양4", Price = 243400, Volume = 423, BasisPrice = 242400, PreviousVolume = 10656 });
            Consumer.IndexItems.Add("000060", new DashboardItem() { Code = "000060", Name = "삼양5", Price = 12200, Volume = 510, BasisPrice = 12300, PreviousVolume = 234 });

            Task.Run(() =>
            {
                Thread.Sleep(4000);
                Consumer.StockItems["000020"].Name = "aaaaaa";

                Thread.Sleep(2000);
                Consumer.StockItems.Add("000070", new DashboardItem() { Code = "000070", Name = "삼양6", Price = 12200, Volume = 510, BasisPrice = 12300, PreviousVolume = 234 });
            });
        }
    }
}
