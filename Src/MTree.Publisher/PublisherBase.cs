using MTree.RealTimeProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTree.Publisher
{
    public class PublisherBase : PublisherCallback
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        protected Guid ClientId { get; } = Guid.NewGuid();

        protected InstanceContext CallbackInstance { get; set; }
        protected PublisherClient ServiceClient { get; set; }

        public PublisherBase()
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
                logger.Info($"Open {GetType().Name} channel");

                ServiceClient = new PublisherClient(CallbackInstance, "RealTimePublisherConfig");
                ServiceClient.InnerChannel.Opened += ServiceClient_Opened;
                ServiceClient.InnerChannel.Closed += ServiceClient_Closed;
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
                    logger.Info($"Close {GetType().Name} channel");

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
            try
            {
                logger.Info($"{GetType().Name} keep connection");
                CommunicateTimer.Start();

                Task.Run(() =>
                {
                    var args = Environment.GetCommandLineArgs();
                    if (args?.Length > 1)
                    {
                        logger.Info($"Argument: {string.Join(" ", args)}");

                        var contract = new PublishContract();
                        contract.Type = PublishContract.ConvertToType(args[1]);

                        ServiceClient.RegisterContract(ClientId, contract);
                    }
                    else
                    {
                        logger.Error($"{GetType().Name} wrong argument");
                    }
                });
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        protected virtual void ServiceClient_Closed(object sender, EventArgs e)
        {
            logger.Info($"{GetType().Name} channel closed");
            CommunicateTimer.Stop();
        }

        public override void CloseClient()
        {
            try
            {
                logger.Info($"{GetType().Name} process will be closed");

                StopCommunicateTimer();
                StopQueueTask();
                CloseChannel();

                Task.Run(() =>
                {
                    Thread.Sleep(1000);
                    Environment.Exit(0);
                });
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        protected override void OnCommunicateTimer(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                if (ServiceClient?.State == CommunicationState.Opened &&
                    (Environment.TickCount - LastWcfCommunicateTick) > MaxCommunicateInterval)
                {
                    if (ServiceClient.State == CommunicationState.Opened)
                    {
                        LastWcfCommunicateTick = Environment.TickCount;
                        ServiceClient.NoOperation();
                        
                        logger.Info($"{GetType().Name} keep wcf connection");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
    }
}
