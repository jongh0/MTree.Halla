﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel.Channels;
using System.ServiceModel;
using MTree.DataStructure;
using MTree.RealTimeProvider;

namespace MTree.Publisher
{
    public partial class PublisherClient : DuplexClientBase<IRealTimePublisher>, IRealTimePublisher
    {
        public PublisherClient(InstanceContext callbackInstance, string endpointConfigurationName) : base(callbackInstance, endpointConfigurationName) { }

        public void NoOperation()
        {
            base.Channel.NoOperation();
        }

        public void RegisterPublishContract(Guid clientId, PublishContract contract)
        {
            base.Channel.RegisterPublishContract(clientId, contract);
        }

        public void UnregisterPublishContract(Guid clientId)
        {
            base.Channel.UnregisterPublishContract(clientId);
        }

        public void PublishBiddingPrice(BiddingPrice biddingPrice)
        {
            base.Channel.PublishBiddingPrice(biddingPrice);
        }

        public void PublishCircuitBreak(CircuitBreak circuitBreak)
        {
            base.Channel.PublishCircuitBreak(circuitBreak);
        }

        public void PublishIndexConclusion(IndexConclusion conclusion)
        {
            base.Channel.PublishIndexConclusion(conclusion);
        }

        public void PublishStockConclusion(StockConclusion conclusion)
        {
            base.Channel.PublishStockConclusion(conclusion);
        }
    }
}
