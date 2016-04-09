using System.Windows;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using MTree.Utility;
using MTree.Configuration;
using System.Diagnostics;
using System;

namespace MTree.RealTimeProvider
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        ServiceHost Host { get; set; }

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
            Host = new ServiceHost(instance);
            Host.Opened += Host_Opened;
            Host.Closed += Host_Closed;
            Host.Faulted += Host_Faulted;
            Host.Open();

            this.DataContext = instance;
        }

        private void Host_Faulted(object sender, EventArgs e)
        {
            logger.Error("Host faulted");
        }

        private void Host_Closed(object sender, EventArgs e)
        {
            logger.Info("Host closed");
        }

        private void Host_Opened(object sender, EventArgs e)
        {
            logger.Info("Host opened");

            Task.Run(() =>
            {
                // 추가된 설정들을 파일에 저장하기 위해서 호출
                Config.Initialize();

                if (Config.General.OfflineMode == false)
                {
                    // Popup stopper
                    ProcessUtility.Start(ProcessTypes.PopupStopper, ProcessWindowStyle.Minimized);

                    // Daishin CybosPlus 실행
                    if (ProcessUtility.Exists(ProcessTypes.CybosStarter) == false)
                    {
                        logger.Info("Daishin starter not exists");
                        ProcessUtility.Start(ProcessTypes.DaishinSessionManager, ProcessWindowStyle.Minimized)?.WaitForExit();
                    }

                    // Daishin Master 실행
                    ProcessUtility.Start(ProcessTypes.DaishinPublisherMaster, ProcessWindowStyle.Minimized);
                }
            });
        }
    }
}
