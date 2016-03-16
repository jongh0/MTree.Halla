﻿using MTree.RealTimeProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTree.Consumer
{
    public class ConsumerBase : ConsumerCallback
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        protected Guid ClientId { get; } = Guid.NewGuid();

        protected InstanceContext CallbackInstance { get; set; }
        protected ConsumerClient ServiceClient { get; set; }

        public ConsumerBase()
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

                ServiceClient = new ConsumerClient(CallbackInstance, "RealTimeConsumerConfig");
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
                    logger.Info($"[{GetType().Name}] Close channel");

                    ServiceClient.UnregisterContractAll(ClientId);
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
            CommunicateTimer.Start();
        }

        protected virtual void ServiceClient_Closed(object sender, EventArgs e)
        {
            logger.Info($"[{GetType().Name}] Channel closed");
            CommunicateTimer.Stop();
        }

        public override void CloseClient()
        {
            try
            {
                logger.Info($"[{GetType().Name}] Process will be closed");

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
                        
                        logger.Trace($"[{GetType().Name}] Keep wcf connection");
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
