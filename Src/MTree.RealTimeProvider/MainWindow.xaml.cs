using System.Windows;
using System.ServiceModel;

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

#if false // TODO : Launch를 먼저할지 요청이 있을 때 Launch를 할지 정해야 함
            RealTimeProviderInstance.LaunchPublisher(PublisherType.Daishin);
            RealTimeProviderInstance.LaunchPublisher(PublisherType.Ebest);
            //RealTimeProviderInstance.LaunchPublisher(PublishType.Krx);
#endif
        }
    }
}
