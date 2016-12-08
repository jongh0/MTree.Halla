using MTree.RealTimeProvider;
using MTree.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTree.Publisher
{
    public class RealTimePublisher : PublisherBase
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        protected Guid ClientId { get; } = Guid.NewGuid();

        protected InstanceContext CallbackInstance { get; set; }
        protected PublisherClient ServiceClient { get; set; }

        public RealTimePublisher()
        {
            try
            {
                CallbackInstance = new InstanceContext(this);
                OpenChannel();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        protected void OpenChannel()
        {
            try
            {
                logger.Info($"[{GetType().Name}] Open channel");

                ServiceClient = new PublisherClient(CallbackInstance, "RealTimePublisherConfig");
                ServiceClient.InnerChannel.Opened += ServiceClient_Opened;
                ServiceClient.InnerChannel.Closed += ServiceClient_Closed;
                ServiceClient.InnerChannel.Faulted += ServiceClient_Faulted;
                ServiceClient.Open();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        protected void CloseChannel()
        {
            try
            {
                if (ServiceClient != null)
                {
                    logger.Info($"[{GetType().Name}] Close channel");

                    ServiceClient.UnregisterContract(ClientId);
                    ServiceClient.Close();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        protected virtual void ServiceClient_Opened(object sender, EventArgs e)
        {
            logger.Info($"[{GetType().Name}] Channel opened");
        }

        protected virtual void ServiceClient_Closed(object sender, EventArgs e)
        {
            logger.Info($"[{GetType().Name}] Channel closed");
        }

        protected virtual void ServiceClient_Faulted(object sender, EventArgs e)
        {
            logger.Error($"[{GetType().Name}] Channel faulted");
        }

        public void RegisterPublishContract(ProcessTypes type = ProcessTypes.Unknown)
        {
            try
            {
                if (type != ProcessTypes.Unknown)
                {
                    var contract = new PublisherContract() { Type = type };
                    ServiceClient.RegisterContract(ClientId, contract);
                    return;
                }

                var args = Environment.GetCommandLineArgs();
                if (args?.Length > 1)
                {
                    logger.Info($"Argument: {string.Join(" ", args)}");

                    var contract = new PublisherContract();
                    contract.Type = PublisherContract.ConvertToType(args[1]);

                    ServiceClient.RegisterContract(ClientId, contract);
                }
                else
                {
                    logger.Error($"[{GetType().Name}] Wrong argument");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public override void NotifyMessage(MessageTypes type, string message)
        {
            logger.Info($"[{GetType().Name}] NotifyMessage, type: {type.ToString()}, message: {message}");

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
                logger.Error(ex);
            }
        }
    }
}
