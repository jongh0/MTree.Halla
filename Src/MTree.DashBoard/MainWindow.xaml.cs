using MTree.Consumer;
using MTree.DataStructure;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace MTree.Dashboard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Dashboard dashboard { get; set; }
        private ConsumerBase consumer { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            
            if (Environment.GetCommandLineArgs().Length > 1)
            {
                if (Environment.GetCommandLineArgs()[1] == "/Simul")
                {
                    consumer = new HistoryConsumer();
                }
            }
            else
            {
                consumer = new RealTimeConsumer();
            }

            dashboard = new Dashboard(consumer);
            this.DataContext = dashboard;

            //TestData();
        }

        private void TestData()
        {
            dashboard.StockItems.Add("000020", new DashboardItem() { Code = "000020", Name = "삼양1", Price = 2000, Volume = 60, BasisPrice = 2000, PreviousVolume = 50 });
            dashboard.StockItems.Add("000030", new DashboardItem() { Code = "000030", Name = "삼양2", Price = (float)102.20, Volume = 243, BasisPrice = (float)101.20, PreviousVolume = 176750 });
            dashboard.StockItems.Add("000040", new DashboardItem() { Code = "000040", Name = "삼양3", Price = 55, Volume = 5435, BasisPrice = 50, PreviousVolume = 7576, CircuitBreakType = DataStructure.CircuitBreakTypes.StaticInvoke });
            dashboard.StockItems.Add("000050", new DashboardItem() { Code = "000050", Name = "삼양4", Price = 243400, Volume = 423, BasisPrice = 244400, PreviousVolume = 10656 });
            dashboard.StockItems.Add("000060", new DashboardItem() { Code = "000060", Name = "삼양5", Price = 12200, Volume = 510, BasisPrice = 11200, PreviousVolume = 234, CircuitBreakType = DataStructure.CircuitBreakTypes.DynamicInvoke });

            dashboard.IndexItems.Add("000040", new DashboardItem() { Code = "000040", Name = "삼양3", Price = 50, Volume = 5435, BasisPrice = 46, PreviousVolume = 7576 });
            dashboard.IndexItems.Add("000050", new DashboardItem() { Code = "000050", Name = "삼양4", Price = 243400, Volume = 423, BasisPrice = 242400, PreviousVolume = 10656 });
            dashboard.IndexItems.Add("000060", new DashboardItem() { Code = "000060", Name = "삼양5", Price = 12200, Volume = 510, BasisPrice = 12300, PreviousVolume = 234 });

            Task.Run(() =>
            {
                Thread.Sleep(1000);
                dashboard.StockItems["000020"].Price = 2100;
                dashboard.StockItems["000020"].Price = 2110;
                dashboard.StockItems["000020"].Price = 2111;
                dashboard.StockItems["000020"].Price = 2112;
                dashboard.StockItems["000020"].Price = 2113;

                Thread.Sleep(1000);
                dashboard.StockItems["000030"].Price = (float)102.21;

                Thread.Sleep(1000);
                dashboard.StockItems["000060"].Price = 12201;

                Thread.Sleep(1000);
                dashboard.StockItems["000020"].Price = 2101;

                Thread.Sleep(1000);
                dashboard.StockItems["000030"].Price = (float)102.22;

                Thread.Sleep(1000);
                dashboard.StockItems["000060"].Price = 12202;

                Thread.Sleep(1000);
                dashboard.StockItems["000020"].Price = 2103;

                Thread.Sleep(1000);
                dashboard.StockItems["000030"].Price = (float)102.23;

                Thread.Sleep(1000);
                dashboard.StockItems["000060"].Price = 12203;
            });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                if (consumer is ISimulation)
                {
                    string[] codes = { "000020", "000030" };
                    ((ISimulation)consumer).StartSimulation(codes, new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day));
                }
            });
        }
    }
}
