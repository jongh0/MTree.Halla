using System;
using System.Collections.Generic;
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
#if true // Test data
            Consumer.DashboardItems.Add(new DashboardItem() { Code = "000020", Name = "삼양", Price = 100, Volume = 50, PreviousClosedPrice = 30430, PreviousVolume = 5665767 });
            Consumer.DashboardItems.Add(new DashboardItem() { Code = "000030", Name = "삼양", Price = 10220, Volume = 243, PreviousClosedPrice = 343500, PreviousVolume = 176750 });
            Consumer.DashboardItems.Add(new DashboardItem() { Code = "000040", Name = "삼양", Price = 45, Volume = 5435, PreviousClosedPrice = 305450, PreviousVolume = 7576 });
            Consumer.DashboardItems.Add(new DashboardItem() { Code = "000050", Name = "삼양", Price = 143400, Volume = 423, PreviousClosedPrice = 32500, PreviousVolume = 10656 });
            Consumer.DashboardItems.Add(new DashboardItem() { Code = "000060", Name = "삼양", Price = 12200, Volume = 510, PreviousClosedPrice = 3052540, PreviousVolume = 234 });
#endif

            this.DataContext = Consumer;
        }
    }
}
