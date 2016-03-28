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

namespace MTree.HistorySaver
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private HistorySaver Consumer { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            Consumer = new HistorySaver();
            //TestData();

            this.DataContext = Consumer;
        }

        private void TestData()
        {
            Consumer.StockMasterCount = 2750;
            Consumer.BiddingPriceCount = 10004562;
            Consumer.CircuitBreakCount = 127;
            Consumer.StockConclusionCount = 2456354;
            Consumer.IndexConclusionCount = 321542;
        }
    }
}
