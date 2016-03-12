using System.Windows;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using MTree.Utility;

namespace MTree.RealTimeProvider
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        ServiceHost RealTimeHost { get; set; }

        RealTimeProvider RealTimeProviderInstance { get { return (RealTimeHost.SingletonInstance as RealTimeProvider); } }

        public MainWindow()
        {
            InitializeComponent();

            var instance = new RealTimeProvider();
            RealTimeHost = new ServiceHost(instance);
            RealTimeHost.Opened += RealTimeHost_Opened;
            RealTimeHost.Closed += RealTimeHost_Closed;
            RealTimeHost.Faulted += RealTimeHost_Faulted;
            RealTimeHost.Open();
        }

        private void RealTimeHost_Faulted(object sender, System.EventArgs e)
        {
            logger.Info("RealTimeHost faulted");
        }

        private void RealTimeHost_Closed(object sender, System.EventArgs e)
        {
            logger.Info("RealTimeHost closed");
        }

        private void RealTimeHost_Opened(object sender, System.EventArgs e)
        {
            logger.Info("RealTimeHost opened");

            ProcessUtility.Start(ProcessType.DaishinMaster);
        }
    }
}
