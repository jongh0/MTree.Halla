using System.Windows;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using CommonLib;
using Configuration;
using System.Diagnostics;
using System;
using CommonLib.Utility;

namespace RealTimeProvider
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

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

            try
            {
                Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.High;

                var instance = new RealTimeProvider_();
                Host = new ServiceHost(instance);
                Host.Opened += Host_Opened;
                Host.Closed += Host_Closed;
                Host.Faulted += Host_Faulted;
                Host.Open();

                this.DataContext = instance;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
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

            Task.Run(() =>
            {
                // 추가된 설정들을 파일에 저장하기 위해서 호출
                Config.Initialize();

                if (Config.General.OfflineMode == false)
                {
                    // Popup stopper 실행
                    ProcessUtility.Start(ProcessTypes.PopupStopper, ProcessWindowStyle.Minimized);

                    // Daishin CybosPlus 실행
                    if (ProcessUtility.Exists(ProcessTypes.CybosStarter) == false)
                    {
                        _logger.Info("Daishin starter not exists, run CybosStarter");
                        ProcessUtility.Start(ProcessTypes.DaishinSessionManager, ProcessWindowStyle.Minimized)?.WaitForExit();
                    }

                    // Daishin Master 실행
                    ProcessUtility.Start(ProcessTypes.DaishinPublisherMaster, ProcessWindowStyle.Minimized);
                }
                else
                {
#if true
                    Thread.Sleep(2000);

                    var clientCount = 3;

                    for (int i = 0; i < clientCount; i++)
                        ProcessUtility.Start(ProcessTypes.TestConsumer, ProcessWindowStyle.Minimized);

                    Thread.Sleep(1000);

                    for (int i = 0; i < clientCount; i++)
                        ProcessUtility.Start(ProcessTypes.TestPublisher, ProcessWindowStyle.Minimized); 
#endif
                } 
            });
        }
    }
}
