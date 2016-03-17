using System.Windows;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using MTree.Utility;
using MTree.Configuration;

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

        /// <summary>
        /// MainWindow 실행
        /// Service Host 등록
        /// DaishinMaster Process 실행
        /// DaishinMaster가 Publisher로 등록되면 Stock Code List 획득
        /// HistorySaver, Daishin, Ebest Process 실행
        /// Stock Mastering 수행
        /// HistorySaver 통해서 Db에 저장
        /// Daishin Process에 Stock Subscribe Code 분배
        /// </summary>
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

            if (Config.General.OfflineMode == false)
                ProcessUtility.Start(ProcessTypes.DaishinMaster);
                //ProcessUtility.Start(ProcessTypes.Kiwoom);
        }
    }
}
