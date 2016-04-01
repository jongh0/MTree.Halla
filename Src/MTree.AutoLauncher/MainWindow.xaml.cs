using MTree.Utility;
using System;
using System.ComponentModel;
using System.Windows;

namespace MTree.AutoLauncher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private Launcher launcher;

        public MainWindow()
        {
            InitializeComponent();

            try
            {
                var now = DateTime.Now;

                launcher = new Launcher(ProcessTypes.RealTimeProvider);
                launcher.Time = new DateTime(now.Year, now.Month, now.Day, 7, 10, 0);

                launcher.KillProcesses.Add(ProcessTypes.CybosStarter);
                launcher.KillProcesses.Add(ProcessTypes.PopupStopper);
                launcher.KillProcesses.Add(ProcessTypes.DaishinSessionManager);
                launcher.KillProcesses.Add(ProcessTypes.Ebest);
                launcher.KillProcesses.Add(ProcessTypes.Kiwoom);
                launcher.KillProcesses.Add(ProcessTypes.Daishin);
                launcher.KillProcesses.Add(ProcessTypes.HistorySaver);
                launcher.KillProcesses.Add(ProcessTypes.Dashboard);

                this.DataContext = launcher;
                launcher.Start();

                var msg = "AutoLauncher started";
                logger.Info(msg);
                PushUtility.NotifyMessage(msg);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
    }
}
