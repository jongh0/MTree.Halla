using MTree.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;

namespace MTree.AutoLauncher
{
    public class Launcher : INotifyPropertyChanged
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public bool KillIfExists { get; set; } = false;

        #region Launch Time
        private TimeSpan Interval { get; } = new TimeSpan(1, 0, 0, 0); // 1 Day interval

        private DateTime _time;
        public DateTime Time
        {
            get { return _time; }
            set { _time = value; NotifyPropertyChanged(nameof(Time)); }
        }

        private System.Timers.Timer LaunchTimer { get; set; }
        #endregion

        #region Process
        public ProcessTypes LaunchProcess = ProcessTypes.None;

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
                    logger.Error($"[{LaunchProcess}] time not set");
                    return;
                }

                var now = DateTime.Now;
                Time = new DateTime(now.Year, now.Month, now.Day, Time.Hour, Time.Minute, Time.Second);

                while (Time <= now || Time.DayOfWeek == DayOfWeek.Saturday || Time.DayOfWeek == DayOfWeek.Sunday)
                    Time = Time.Add(Interval);

                LaunchTimer.Stop();
                LaunchTimer.Interval = (Time - now).TotalMilliseconds;
                LaunchTimer.Start();

                logger.Info($"[{LaunchProcess}] Launcher started, will be triggered at {Time.ToString()}");
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
                logger.Info($"[{LaunchProcess}] Launcher stopped");
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
                        logger.Info($"[{LaunchProcess}] Exists and will be killed");

                        ProcessUtility.Kill(LaunchProcess);
                        Thread.Sleep(1000 * 5);
                    }
                    else
                    {
                        logger.Error($"[{LaunchProcess}] Exists");
                        return;
                    }
                }

                foreach (var processType in KillProcesses)
                {
                    ProcessUtility.Kill(processType);
                }

                ProcessUtility.Start(LaunchProcess);

                logger.Info($"[{LaunchProcess}] Launched");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public void LaunchNow()
        {
            logger.Info($"[{LaunchProcess}] Launch now");
            Launch();
        }

        private void LaunchTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                logger.Info($"[{LaunchProcess}] Launcher elapsed");
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

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}
