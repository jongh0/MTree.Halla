using MTree.Consumer;
using MTree.DataStructure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.StrategyManager
{
    public class DbHandler : INotifyPropertyChanged, INotifySubscribableReceived
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion

        #region INotifySubscribable
        public event SubscribableReceivedEventHandler BiddingPriceReceived;
        public event SubscribableReceivedEventHandler CircuitBreakReceived;
        public event SubscribableReceivedEventHandler ConclusionReceived;

        private void NotifyBiddingPrice(BiddingPrice biddingPrice)
        {
            BiddingPriceReceived?.Invoke(this, new SubscribableNotifiedEventArgs(biddingPrice));
        }

        private void NotifyCircuitBreak(CircuitBreak circuitBreak)
        {
            CircuitBreakReceived?.Invoke(this, new SubscribableNotifiedEventArgs(circuitBreak));
        }

        private void NotifyConclusion(Conclusion conclusion)
        {
            ConclusionReceived?.Invoke(this, new SubscribableNotifiedEventArgs(conclusion));
        }
        #endregion
    }
}
