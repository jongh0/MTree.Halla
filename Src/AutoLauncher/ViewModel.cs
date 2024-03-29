﻿using GalaSoft.MvvmLight.Command;
using Configuration;
using CommonLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CommonLib.Windows;
using CommonLib.Utility;

namespace AutoLauncher
{
    public class ViewModel : INotifyPropertyChanged
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public string MainTitle => $"AutoLauncher {AssemblyUtility.VersionName}";

        public Shutdowner Shutdowner { get; set; } = new Shutdowner();

        private List<Launcher> _launchers;
        public List<Launcher> Launchers
        {
            get => _launchers;
            set
            {
                _launchers = value;
                NotifyPropertyChanged(nameof(Launchers));
            }
        }

        public List<ProcessTypes> KillProcesses { get; set; } = new List<ProcessTypes>();

        RelayCommand _killAllCommand;
        public ICommand KillAllCommand
        {
            get
            {
                if (_killAllCommand == null)
                    _killAllCommand = new RelayCommand(() => ExecuteKillAll());

                return _killAllCommand;
            }
        }

        public void ExecuteKillAll()
        {
            _logger.Info("Execute kill all");

            if (MessageBox.Show("Kill all process?", "", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                ProcessUtility.Start(ProcessTypes.KillAll);
        }

        RelayCommand _shutdownCommand;
        public ICommand ShutdownCommand
        {
            get
            {
                if (_shutdownCommand == null)
                    _shutdownCommand = new RelayCommand(() => ExecuteShutdown());

                return _shutdownCommand;
            }
        }

        public void ExecuteShutdown()
        {
            _logger.Info("Execute shutdown");

            if (MessageBox.Show("Shutdown?", "", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                WindowsCommand.Shutdown();
        }

        public ViewModel()
        {
            try
            {
#if !DEBUG
                if (Config.General.UseShutdown == true)
                {
                    if (Shutdowner.IsWorkingDay() == false)
                    {
                        Task.Run(() =>
                        {
                            _logger.Info("Not working day, shutdown after 5 mins");

                            Thread.Sleep(1000 * 60 * 5);
                            WindowsCommand.Shutdown();
                        });

                        return;
                    }
                    else
                    {
                        Shutdowner.Start();
                    }
                } 
#endif

                Launchers = new List<Launcher>();
                Launchers.Add(CreateRealTimeProviderLauncher());

                if (Config.ResourceMonitor.UseResourceMonitor == true)
                {
                    Launchers.Add(CreatePerfMonStarter());
                    Launchers.Add(CreatePerfMonStopper());
                }

                foreach (Launcher launcher in Launchers)
                {
                    launcher.Start();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
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
            launcher.KillProcesses.Add(ProcessTypes.KiwoomSessionManager);
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
