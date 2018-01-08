using Configuration;
using RealTimeProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Consumer
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class RealTimeConsumer : ConsumerBase
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        protected Guid ClientId { get; } = Guid.NewGuid();

        protected InstanceContext CallbackInstance { get; set; }
        protected ConsumerClient ServiceClient { get; set; }

        public RealTimeConsumer()
        {
            try
            {
                NotifyMessageEvent += HandleNotifyMessage;

                CallbackInstance = new InstanceContext(this);
                OpenChannel();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }
        
        protected void OpenChannel()
        {
            try
            {
                _logger.Info($"[{GetType().Name}] Open channel");

                ServiceClient = new ConsumerClient(CallbackInstance, "RealTimeConsumerConfig");
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

                    ServiceClient.UnregisterContractAll(ClientId);
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
        }

        protected virtual void ServiceClient_Closed(object sender, EventArgs e)
        {
            _logger.Info($"[{GetType().Name}] Channel closed");
        }

        protected virtual void ServiceClient_Faulted(object sender, EventArgs e)
        {
            _logger.Error($"[{GetType().Name}] Channel faulted");
        }

        public void HandleNotifyMessage(MessageTypes type, string message)
        {
            _logger.Info($"[{GetType().Name}] NotifyMessage, type: {type.ToString()}, message: {message}");
            
            try
            {
                if (type == MessageTypes.CloseClient)
                {
                    StopQueueTask();
                    CloseChannel();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        public void RegisterContract(SubscribeContract contract)
        {
            try
            {
                ServiceClient.RegisterContract(ClientId, contract);
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }
    }
}
