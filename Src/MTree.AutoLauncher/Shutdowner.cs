using MTree.Configuration;
using MTree.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace MTree.AutoLauncher
{
    public class Shutdowner
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private System.Timers.Timer ShutdownTimer { get; set; }

        public bool IsWorkingDay()
        {
            switch (DateTime.Now.DayOfWeek)
            {
                case DayOfWeek.Saturday:
                case DayOfWeek.Sunday:
                    return false;

                default:
                    return true;
            }
        }

        public void Start()
        {
            var now = DateTime.Now;
            var shutdownTime = new DateTime(now.Year, now.Month, now.Day, Config.General.ShutdownHour % 24, 0, 0);
            if (shutdownTime < now)
                shutdownTime = shutdownTime.AddDays(1);

            if (ShutdownTimer == null)
            {
                ShutdownTimer = new System.Timers.Timer();
                ShutdownTimer.AutoReset = false;
                ShutdownTimer.Elapsed += ShutdownTimer_Elapsed;
            }

            ShutdownTimer.Interval = (shutdownTime - now).TotalMilliseconds;
            ShutdownTimer.Start();
        }

        private void ShutdownTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            logger.Info($"Shutdown timer elapsed");
            ProcessUtility.Shutdown();
        }
    }
}
