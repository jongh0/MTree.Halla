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

namespace MTree.KiwoomTrader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        KiwoomTrader Trader { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.Integration.WindowsFormsHost host = new System.Windows.Forms.Integration.WindowsFormsHost();
            AxKHOpenAPILib.AxKHOpenAPI axKHOpenAPI = new AxKHOpenAPILib.AxKHOpenAPI();
            host.Child = axKHOpenAPI;
            this.mainGrid.Children.Add(host);

            Trader = new KiwoomTrader(axKHOpenAPI);
        }
    }
}
