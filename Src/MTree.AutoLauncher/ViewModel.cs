using GalaSoft.MvvmLight.Command;
using MTree.Configuration;
using MTree.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MTree.AutoLauncher
{
    public class ViewModel : INotifyPropertyChanged
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private List<Launcher> _Launchers;
        public List<Launcher> Launchers
        {
            get
            {
                return _Launchers;
            }
            set
            {
                _Launchers = value;
                NotifyPropertyChanged(nameof(Launchers));
            }
        }

        public List<ProcessTypes> KillProcesses { get; set; } = new List<ProcessTypes>();

        RelayCommand _KillAllCommand;
        public ICommand KillAllCommand
        {
            get
            {
                if (_KillAllCommand == null)
                    _KillAllCommand = new RelayCommand(() => ExecuteKillAll());

                return _KillAllCommand;
            }
        }
        public void ExecuteKillAll()
        {
            logger.Info("Execute kill all");
            ProcessUtility.Start(ProcessTypes.KillAll);
        }

        RelayCommand _ShutdownCommand;
        public ICommand ShutdownCommand
        {
            get
            {
                if (_ShutdownCommand == null)
                    _ShutdownCommand = new RelayCommand(() => ExecuteShutdown());

                return _ShutdownCommand;
            }
        }

        public void ExecuteShutdown()
        {
            if (MessageBox.Show("Shutdown?", "Question", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                ProcessUtility.Shutdown();
        }

        public ViewModel()
        {
            try
            {
                Launchers = new List<Launcher>();
                Launchers.Add(CreateRealTimeProviderLauncher());
                if (Config.ResourceMonitor.UseResourceMonitor == true)
                {
                    Launchers.Add(CreatePerfMonStarter());
                    Launchers.Add(CreatePerfMonStopper());
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private Launcher CreateRealTimeProviderLauncher()
        {
            var now = DateTime.Now;

            Launcher launcher = new Launcher(ProcessTypes.RealTimeProvider);
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

            return launcher;
        }

        private Launcher CreatePerfMonStarter()
        {
            var now = DateTime.Now;
            
            Launcher launcher = new Launcher(ProcessTypes.ResourceMonitor, "Start");
            launcher.Time = new DateTime(now.Year, now.Month, now.Day, 7, 9, 0);
            return launcher;
        }

        private Launcher CreatePerfMonStopper()
        {
            var now = DateTime.Now;
            
            Launcher launcher = new Launcher(ProcessTypes.ResourceMonitor, "Stop");
            launcher.Time = new DateTime(now.Year, now.Month, now.Day, 19, 0, 0);
            return launcher;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}
