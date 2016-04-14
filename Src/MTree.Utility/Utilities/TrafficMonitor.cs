using MTree.DataStructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.Utility
{
    public class TrafficMonitor : INotifyPropertyChanged
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private TimeSpan latency = new TimeSpan(0);
        public TimeSpan Latency
        {
            get { return latency; }
            set
            {
                if (latency != value)
                {
                    latency = value;
                    NotifyPropertyChanged(nameof(Latency));
                }
            }
        }
        
        public void CheckLatency(Subscribable subscribable)
        {
            Latency = DateTime.Now - subscribable.Time;
            if (Latency.TotalMilliseconds > 1000)
                logger.Debug($"[{GetType().Name}] {subscribable.GetType().Name} data transfer delayed. Latency: {Latency.TotalMilliseconds}");
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
