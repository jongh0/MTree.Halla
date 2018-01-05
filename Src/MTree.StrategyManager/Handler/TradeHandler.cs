using MTree.Trader;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MTree.StrategyManager
{
    public class TradeHandler : TraderBase, INotifyPropertyChanged
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public TradeHandler(string endpointConfigurationName) : base(endpointConfigurationName)
        {
            try
            {
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        public override void NotifyOrderResult(OrderResult result)
        {
            try
            {
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        protected override void ServiceClient_Opened(object sender, EventArgs e)
        {
            base.ServiceClient_Opened(sender, e);

            try
            {
                ServiceClient.RegisterContract(ClientId, new TraderContract());
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
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
