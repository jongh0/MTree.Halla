using RealTimeProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Publisher
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class PublisherBase : PublisherCallback
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        protected Guid ClientId { get; } = Guid.NewGuid();

        protected InstanceContext CallbackInstance { get; set; }
        protected PublisherClient ServiceClient { get; set; }

        public PublisherBase()
        {
            try
            {
                CallbackInstance = new InstanceContext(this);

                Task.Run(() =>
                {
                    Thread.Sleep(1000);
                    OpenChannel();
                });
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

                ServiceClient = new PublisherClient(CallbackInstance, "RealTimePublisherConfig");
                ServiceClient.InnerChannel.Opened += ServiceClient_Opened;
                ServiceClient.InnerChannel.Closed += ServiceClient_Closed;
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

                    ServiceClient.UnregisterPublisherContract(ClientId);
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

        public void RegisterContract()
        {
            try
            {
                var args = Environment.GetCommandLineArgs();
                if (args?.Length > 1)
                {
                    _logger.Info($"Argument: {string.Join(" ", args)}");

                    var contract = new PublisherContract();
                    contract.Type = PublisherContract.ConvertToType(args[1]);

                    ServiceClient?.RegisterPublisherContract(ClientId, contract);
                }
                else
                {
                    _logger.Error($"[{GetType().Name}] Wrong argument");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        public override void NotifyMessage(MessageTypes type, string message)
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
    }
}
