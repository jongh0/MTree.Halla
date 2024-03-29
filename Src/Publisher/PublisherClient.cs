﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel.Channels;
using System.ServiceModel;
using DataStructure;
using RealTimeProvider;

namespace Publisher
{
    public partial class PublisherClient : DuplexClientBase<IRealTimePublisher>, IRealTimePublisher
    {
        public PublisherClient(InstanceContext callbackInstance, string endpointConfigurationName) : base(callbackInstance, endpointConfigurationName) { }

        public void SendMessage(MessageTypes type, string message)
        {
            if (State != CommunicationState.Opened) return;
            base.Channel.SendMessage(type, message);
        }

        public void RegisterPublisherContract(Guid clientId, PublisherContract contract)
        {
            if (State != CommunicationState.Opened) return;
            base.Channel.RegisterPublisherContract(clientId, contract);
        }

        public void UnregisterPublisherContract(Guid clientId)
        {
            if (State != CommunicationState.Opened) return;
            base.Channel.UnregisterPublisherContract(clientId);
        }

        public void PublishBiddingPrice(BiddingPrice biddingPrice)
        {
            if (State != CommunicationState.Opened) return;
            base.Channel.PublishBiddingPrice(biddingPrice);
        }

        public void PublishCircuitBreak(CircuitBreak circuitBreak)
        {
            if (State != CommunicationState.Opened) return;
            base.Channel.PublishCircuitBreak(circuitBreak);
        }

        public void PublishIndexConclusion(IndexConclusion conclusion)
        {
            if (State != CommunicationState.Opened) return;
            base.Channel.PublishIndexConclusion(conclusion);
        }

        public void PublishStockConclusion(StockConclusion conclusion)
        {
            if (State != CommunicationState.Opened) return;
            base.Channel.PublishStockConclusion(conclusion);
        }

        public void PublishETFConclusion(ETFConclusion conclusion)
        {
            if (State != CommunicationState.Opened) return;
            base.Channel.PublishETFConclusion(conclusion);
        }
    }
}
