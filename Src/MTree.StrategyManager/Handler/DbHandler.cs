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
    public class DbHandler : INotifyPropertyChanged, INotifySubscribable
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
        public event SubscribableEventHandler BiddingPriceNotified;
        public event SubscribableEventHandler CircuitBreakNotified;
        public event SubscribableEventHandler ConclusionNotified;

        private void NotifyBiddingPrice(BiddingPrice biddingPrice)
        {
            BiddingPriceNotified?.Invoke(this, new SubscribableEventArgs(biddingPrice));
        }

        private void NotifyCircuitBreak(CircuitBreak circuitBreak)
        {
            CircuitBreakNotified?.Invoke(this, new SubscribableEventArgs(circuitBreak));
        }

        private void NotifyConclusion(Conclusion conclusion)
        {
            ConclusionNotified?.Invoke(this, new SubscribableEventArgs(conclusion));
        }
        #endregion
    }
}
