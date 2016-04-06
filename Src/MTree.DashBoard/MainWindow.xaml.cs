#define VERIFY_LATENCY

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
        private Dashboard Consumer { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            Consumer = new Dashboard();
            //TestData();
#if VERIFY_LATENCY
            //TestLatency();  
#endif

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
                Thread.Sleep(1000);
                Consumer.StockItems["000020"].Price = 2100;
                Consumer.StockItems["000020"].Price = 2110;
                Consumer.StockItems["000020"].Price = 2111;
                Consumer.StockItems["000020"].Price = 2112;
                Consumer.StockItems["000020"].Price = 2113;

                Thread.Sleep(1000);
                Consumer.StockItems["000030"].Price = (float)102.21;

                Thread.Sleep(1000);
                Consumer.StockItems["000060"].Price = 12201;

                Thread.Sleep(1000);
                Consumer.StockItems["000020"].Price = 2101;

                Thread.Sleep(1000);
                Consumer.StockItems["000030"].Price = (float)102.22;

                Thread.Sleep(1000);
                Consumer.StockItems["000060"].Price = 12202;

                Thread.Sleep(1000);
                Consumer.StockItems["000020"].Price = 2103;

                Thread.Sleep(1000);
                Consumer.StockItems["000030"].Price = (float)102.23;

                Thread.Sleep(1000);
                Consumer.StockItems["000060"].Price = 12203;
            });
        }

#if VERIFY_LATENCY
        private void TestLatency()
        {
            Task.Run(() =>
            {
                Thread.Sleep(100);
                Random rand = new Random(DateTime.Now.Millisecond);
                while (true)
                {
                    BiddingPrice biddingPrice = new BiddingPrice();
                    biddingPrice.Time = DateTime.Now;
                    Thread.Sleep(rand.Next(10, 1000));
                    Consumer.ConsumeBiddingPrice(biddingPrice);
                }
            });
        } 
#endif
    }
}
