﻿using MTree.RealTimeProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MTree.Publisher
{
    public class PublisherImplement : PublisherBase
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        protected Guid ClientId { get; set; } = Guid.NewGuid();

        protected InstanceContext CallbackInstance { get; set; }
        protected PublisherClient ServiceClient { get; set; }

        public PublisherImplement() : base()
        {
            try
            {
                CallbackInstance = new InstanceContext(this);
                OpenService();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        protected void OpenService()
        {
            try
            {
                ServiceClient = new PublisherClient(CallbackInstance, "RealTimePublisherConfig");
                ServiceClient.Open();

                ServiceClient.RegisterPublisher(ClientId);

                logger.Info("ServiceClient opened");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        protected void CloseService()
        {
            try
            {
                if (ServiceClient != null)
                {
                    ServiceClient.UnregisterPublisher(ClientId);
                    ServiceClient.Close();
                    logger.Info("ServiceClient closed");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
    }
}
