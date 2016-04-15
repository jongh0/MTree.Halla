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
                launcher.KillProcesses.Add(ProcessTypes.Dashboard);
                launcher.KillProcesses.Add(ProcessTypes.HistorySaver);
                launcher.KillProcesses.Add(ProcessTypes.StrategyManager);
                launcher.KillProcesses.Add(ProcessTypes.DaishinPublisher);
                launcher.KillProcesses.Add(ProcessTypes.EbestPublisher);
                launcher.KillProcesses.Add(ProcessTypes.KiwoomPublisher);
                launcher.KillProcesses.Add(ProcessTypes.EbestTrader);
                launcher.KillProcesses.Add(ProcessTypes.KiwoomTrader);
                launcher.KillProcesses.Add(ProcessTypes.VirtualTrader);
                launcher.KillProcesses.Add(ProcessTypes.DaishinSessionManager);
                launcher.KillProcesses.Add(ProcessTypes.PopupStopper);

                this.DataContext = launcher;
                launcher.Start();

                logger.Info("AutoLauncher started");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
    }
}
