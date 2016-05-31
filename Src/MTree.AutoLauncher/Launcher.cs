﻿using GalaSoft.MvvmLight.Command;
using MTree.Configuration;
using MTree.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MTree.AutoLauncher
{
    public class Launcher : INotifyPropertyChanged
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public bool KillIfExists { get; set; } = false;
        
        #region Launch Time
        private TimeSpan LaunchInterval { get; } = TimeSpan.FromDays(1); // 1 Day interval

        public DateTime Time { get; set; }

        private System.Timers.Timer LaunchTimer { get; set; }
        #endregion

        #region Process
        public ProcessTypes LaunchProcess { get; } = ProcessTypes.Unknown;
        public string LaunchArguments { get; set; }

        public List<ProcessTypes> KillProcesses { get; set; } = new List<ProcessTypes>(); 
        #endregion

        public Launcher(ProcessTypes type, string arguments = null)
        {
            try
            {
                LaunchProcess = type;
                LaunchArguments = arguments;

                LaunchTimer = new System.Timers.Timer();
                LaunchTimer.AutoReset = false;
                LaunchTimer.Elapsed += LaunchTimer_Elapsed;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public void Start()
        {
            try
            {
                if (Time == null)
                {
                    logger.Error($"{LaunchProcess} time not set");
                    return;
                }

                var now = DateTime.Now;

                Time = new DateTime(now.Year, now.Month, now.Day, Time.Hour, Time.Minute, Time.Second);

                while (Time <= now || Time.DayOfWeek == DayOfWeek.Saturday || Time.DayOfWeek == DayOfWeek.Sunday)
                    Time = Time.Add(LaunchInterval);

                LaunchTimer.Stop();
                LaunchTimer.Interval = (Time - now).TotalMilliseconds;
                LaunchTimer.Start();

            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public void Stop()
        {
            if (LaunchTimer?.Enabled == true)
            {
                LaunchTimer.Stop();
                logger.Info($"{LaunchProcess} launcher stopped");
            }
        }

        private void Launch()
        {
            try
            {
                if (ProcessUtility.Exists(LaunchProcess) == true)
                {
                    if (KillIfExists == true)
                    {
                        logger.Info($"{LaunchProcess} exists and will be killed");

                        ProcessUtility.Kill(LaunchProcess);
                        Thread.Sleep(1000 * 5);
                    }
                    else
                    {
                        logger.Error($"{LaunchProcess} already exists, launcher stopped");
                        return;
                    }
                }

                foreach (var processType in KillProcesses)
                {
                    ProcessUtility.Kill(processType);
                }

                ProcessUtility.Start(LaunchProcess);

                var msg = $"{LaunchProcess} launched";
                logger.Info(msg);
                PushUtility.NotifyMessage(msg);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
        
        private void LaunchTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                logger.Info($"{LaunchProcess} launcher timer elapsed");
                Launch();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                Start();
            }
        }

        #region Command
        RelayCommand _LaunchNowCommand;
        public ICommand LaunchNowCommand
        {
            get
            {
                if (_LaunchNowCommand == null)
                    _LaunchNowCommand = new RelayCommand(() => ExecuteLaunchNow());

                return _LaunchNowCommand;
            }
        }

        public void ExecuteLaunchNow()
        {
            logger.Info("Execute launch now");
            Launch();
        }
        #endregion

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}
