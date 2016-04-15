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

        public TimeSpan Latency { get; set; }
        public TimeSpan AverageLatency { get; set; }
        
        public float StockConclusionThroughput { get; set; }
        public float BiddingPriceThroughput { get; set; }

        private List<TimeSpan> LatencyList = new List<TimeSpan>();

        private DateTime lastRefreshed;

        public TrafficMonitor()
        {
        }

        public TrafficMonitor(DataCounter Counter)
        {
            this.Counter = Counter;

            if (this.Counter != null)
            {
                PrevCounter = new DataCounter(Counter.Type);
                lastRefreshed = DateTime.Now;
            }
        }

        public void CheckLatency(Subscribable subscribable)
        {
            Latency = DateTime.Now - subscribable.Time;
            if (Latency.TotalMilliseconds > 1000)
                logger.Debug($"[{GetType().Name}] {subscribable.GetType().Name} data transfer delayed. Latency: {Latency.TotalMilliseconds}");

            if (this.Counter != null)
                LatencyList.Add(Latency);
        }

        public void NotifyPropertyAll()
        {
            if (this.Counter == null)
                return;

            int StockConclusionCountHandled = Counter.StockConclusionCount - PrevCounter.StockConclusionCount;
            int BiddingPriceCountHandled = Counter.BiddingPriceCount - PrevCounter.BiddingPriceCount;

            float timeUnit = (float)((lastRefreshed - DateTime.Now).TotalMilliseconds / 1000);
            lastRefreshed = DateTime.Now;

            StockConclusionThroughput = (float)(StockConclusionCountHandled / timeUnit);
            BiddingPriceThroughput = (float)(BiddingPriceCountHandled / timeUnit);

            PrevCounter.StockConclusionCount = Counter.StockConclusionCount;
            PrevCounter.BiddingPriceCount = Counter.BiddingPriceCount;

            if (LatencyList.Count > 0)
            {
                AverageLatency = new TimeSpan(Convert.ToInt64(LatencyList.Average(timeSpan => timeSpan.TotalMilliseconds)));
                LatencyList.Clear();
            }

            //NotifyPropertyChanged(nameof(AverageLatency));
            //NotifyPropertyChanged(nameof(StockConclusionThroughput));
            //NotifyPropertyChanged(nameof(BiddingPriceThroughput));

            logger.Debug($"[{GetType().Name}] AverageLatency: {AverageLatency}, StockConclusionThroughput: {StockConclusionThroughput}, BiddingPriceThroughput: {BiddingPriceThroughput}");
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
