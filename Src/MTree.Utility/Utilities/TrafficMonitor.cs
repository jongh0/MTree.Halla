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

        private DataCounter PrevCounter;
        private DataCounter Counter;

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

        public TimeSpan averageLatency = new TimeSpan(0);
        public TimeSpan AverageLatency
        {
            get { return averageLatency; }
            set
            {
                averageLatency = value;
                NotifyPropertyChanged(nameof(AverageLatency));
            }
        }
        
        public float StockConclusionThroughput { get; set; }
        public float BiddingPriceThroughput { get; set; }

        private List<TimeSpan> LatencyList = new List<TimeSpan>();

        private DateTime lastRefreshed;

        public TrafficMonitor(DataCounter Counter)
        {
            this.Counter = Counter;
            PrevCounter = new DataCounter(Counter.Type);
            lastRefreshed = DateTime.Now;
        }

        public void CheckLatency(Subscribable subscribable)
        {
            Latency = DateTime.Now - subscribable.Time;
            if (Latency.TotalMilliseconds > 1000)
                logger.Debug($"[{GetType().Name}] {subscribable.GetType().Name} data transfer delayed. Latency: {Latency.TotalMilliseconds}");
            LatencyList.Add(Latency);
        }

        public void NotifyPropertyAll()
        {
            int StockConclusionCountHandled = Counter.StockConclusionCount - PrevCounter.StockConclusionCount;
            int BiddingPriceCountHandled = Counter.BiddingPriceCount - PrevCounter.BiddingPriceCount;

            float timeUnit = (float)((lastRefreshed - DateTime.Now).TotalMilliseconds / 1000);
            lastRefreshed = DateTime.Now;

            StockConclusionThroughput = (float)(StockConclusionCountHandled / timeUnit);
            BiddingPriceThroughput = (float)(BiddingPriceCountHandled / timeUnit);

            PrevCounter.StockConclusionCount = Counter.StockConclusionCount;
            PrevCounter.BiddingPriceCount = Counter.BiddingPriceCount;

            AverageLatency = new TimeSpan(Convert.ToInt64(LatencyList.Average(timeSpan => timeSpan.TotalMilliseconds)));
            LatencyList.Clear();

            NotifyPropertyChanged(nameof(AverageLatency));
            NotifyPropertyChanged(nameof(StockConclusionThroughput));
            NotifyPropertyChanged(nameof(BiddingPriceThroughput));
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
