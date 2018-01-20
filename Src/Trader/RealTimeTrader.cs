﻿using RealTimeProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Trader
{
    public class RealTimeTrader : TraderCallback
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        protected Guid ClientId { get; } = Guid.NewGuid();

        protected InstanceContext CallbackInstance { get; set; }
        protected TraderClient ServiceClient { get; set; }

        #region Event
        public event Action<MessageTypes, string> MessageNotified;

        public event Action<RealTimeTrader> ChannelOpened;
        public event Action<RealTimeTrader> ChannelClosed;
        public event Action<RealTimeTrader> ChannelFaulted;
        #endregion

        public RealTimeTrader(string endpointConfigurationName)
        {
            try
            {
                CallbackInstance = new InstanceContext(this);

                Task.Run(() =>
                {
                    Thread.Sleep(1000);
                    OpenChannel(endpointConfigurationName);
                });
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        protected void OpenChannel(string endpointConfigurationName)
        {
            try
            {
                _logger.Info($"[{GetType().Name}] Open channel");

                ServiceClient = new TraderClient(CallbackInstance, endpointConfigurationName);
                ServiceClient.InnerChannel.Opened += ServiceClient_Opened;
                ServiceClient.InnerChannel.Closed += ServiceClient_Closed;
                ServiceClient.InnerChannel.Faulted += ServiceClient_Faulted;
                ServiceClient.Open();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        protected void CloseChannel()
        {
            try
            {
                if (ServiceClient != null)
                {
                    _logger.Info($"[{GetType().Name}] Close channel");
                    ServiceClient.Close();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        protected virtual void ServiceClient_Opened(object sender, EventArgs e)
        {
            _logger.Info($"[{GetType().Name}] Channel opened");
            ChannelOpened?.BeginInvoke(this, null, null);
        }

        protected virtual void ServiceClient_Closed(object sender, EventArgs e)
        {
            _logger.Info($"[{GetType().Name}] Channel closed");
            ChannelClosed?.BeginInvoke(this, null, null);
        }

        protected virtual void ServiceClient_Faulted(object sender, EventArgs e)
        {
            _logger.Error($"[{GetType().Name}] Channel faulted");
            ChannelFaulted?.BeginInvoke(this, null, null);
        }
    }
}