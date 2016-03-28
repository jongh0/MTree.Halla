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
            TestData();

            this.DataContext = Consumer;
        }

        private void TestData()
        {
            Consumer.DashboardItems.Add("000020", new DashboardItem() { Code = "000020", Name = "삼양1", Price = 100, Volume = 50, PreviousClosedPrice = 30430, PreviousVolume = 5665767 });
            Consumer.DashboardItems.Add("000030", new DashboardItem() { Code = "000030", Name = "삼양2", Price = (float)102.20, Volume = 243, PreviousClosedPrice = 343500, PreviousVolume = 176750 });
            Consumer.DashboardItems.Add("000040", new DashboardItem() { Code = "000040", Name = "삼양3", Price = 45, Volume = 5435, PreviousClosedPrice = 305450, PreviousVolume = 7576 });
            Consumer.DashboardItems.Add("000050", new DashboardItem() { Code = "000050", Name = "삼양4", Price = 143400, Volume = 423, PreviousClosedPrice = 32500, PreviousVolume = 10656 });
            Consumer.DashboardItems.Add("000060", new DashboardItem() { Code = "000060", Name = "삼양5", Price = 12200, Volume = 510, PreviousClosedPrice = 3052540, PreviousVolume = 234 });

            Task.Run(() =>
            {
                Thread.Sleep(4000);
                Consumer.DashboardItems["000020"].Name = "aaaaaa";

                Thread.Sleep(2000);
                Consumer.DashboardItems.Add("000070", new DashboardItem() { Code = "000070", Name = "삼양6", Price = 12200, Volume = 510, PreviousClosedPrice = 3052540, PreviousVolume = 234 });
            });
        }
    }
}
