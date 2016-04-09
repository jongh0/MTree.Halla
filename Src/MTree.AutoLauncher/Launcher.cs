using GalaSoft.MvvmLight.Command;
using MTree.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MTree.AutoLauncher
{
    public class Launcher : INotifyPropertyChanged
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public bool KillIfExists { get; set; } = false;

        public string LauncherInfo
        {
            get { return $"{LaunchProcess} | {Time.ToString()}"; }
        }

        #region Launch Time
        private TimeSpan LaunchInterval { get; } = TimeSpan.FromDays(1); // 1 Day interval

        public DateTime Time { get; set; }

        private System.Timers.Timer LaunchTimer { get; set; }
        #endregion

        #region Process
        public ProcessTypes LaunchProcess = ProcessTypes.Unknown;

        public List<ProcessTypes> KillProcesses { get; set; } = new List<ProcessTypes>(); 
        #endregion

        public Launcher(ProcessTypes type)
        {
            LaunchProcess = type;

            LaunchTimer = new System.Timers.Timer();
            LaunchTimer.AutoReset = false;
            LaunchTimer.Elapsed += LaunchTimer_Elapsed;
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

                NotifyPropertyChanged(nameof(LauncherInfo));

                logger.Info($"{LaunchProcess} next launch at {Time.ToString()}");
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
