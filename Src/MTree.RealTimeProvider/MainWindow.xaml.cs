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

            RealTimeHost = new ServiceHost(typeof(RealTimeProvider));
            RealTimeHost.Opened += RealTimeHost_Opened;
            RealTimeHost.Closed += RealTimeHost_Closed;
            RealTimeHost.Faulted += RealTimeHost_Faulted;
            RealTimeHost.Open();
        }

        private void RealTimeHost_Faulted(object sender, System.EventArgs e)
        {
            logger.Info("Service faulted");
        }

        private void RealTimeHost_Closed(object sender, System.EventArgs e)
        {
            logger.Info("Service closed");
        }

        private void RealTimeHost_Opened(object sender, System.EventArgs e)
        {
            logger.Info("Service opened");

#if false // TODO : Launch를 먼저할지 요청이 있을 때 Launch를 할지 정해야 함
            RealTimeProviderInstance.LaunchPublisher(PublishType.DaisinStockMaster);
            RealTimeProviderInstance.LaunchPublisher(PublishType.DaishinStockConclusion);

            RealTimeProviderInstance.LaunchPublisher(PublishType.EbestStockMaster);
            RealTimeProviderInstance.LaunchPublisher(PublishType.EbestIndexMaster);
            RealTimeProviderInstance.LaunchPublisher(PublishType.EbestIndexConclusion);

            RealTimeProviderInstance.LaunchPublisher(PublishType.KrxStockMaster);
#endif
        }
    }
}
