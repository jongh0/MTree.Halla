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
                latency = value;
                NotifyPropertyChanged(nameof(Latency));
            }
        }
        
        public void CheckLatency(Subscribable newSubscribale)
        {
            Latency = DateTime.Now - newSubscribale.Time;
            if (Latency.TotalMilliseconds > 1000)
                logger.Error($"Data transfer delayed. Latency: {Latency.TotalMilliseconds}");
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
