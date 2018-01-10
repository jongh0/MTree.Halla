using Trader;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
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

namespace EbestTrader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        ServiceHost Host { get; set; }

        TraderViewModel TraderVM { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            
            var instance = new EbestTrader_();

            Host = new ServiceHost(instance);
            Host.Opened += Host_Opened;
            Host.Closed += Host_Closed;
            Host.Faulted += Host_Faulted;
            Host.Open();

            TraderVM = new TraderViewModel(instance);
            this.DataContext = TraderVM;
        }

        private void Host_Faulted(object sender, EventArgs e)
        {
            _logger.Error("Host faulted");
        }

        private void Host_Closed(object sender, EventArgs e)
        {
            _logger.Info("Host closed");
        }

        private void Host_Opened(object sender, EventArgs e)
        {
            _logger.Info("Host opened");
        }
    }
}
