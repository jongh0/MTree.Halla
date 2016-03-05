﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel.Channels;
using System.ServiceModel;
using MTree.DataStructure;

namespace MTree.RealTimeProvider
{
    public partial class RealTimeProviderClient : DuplexClientBase<IRealTimeProvider>, IRealTimeProvider
    {
        public RealTimeProviderClient(InstanceContext callbackInstance, string endpointConfigurationName) : base(callbackInstance, endpointConfigurationName) { }

        public void KeepConnection()
        {
            base.Channel.KeepConnection();
        }

        public void NotifyBiddingPrice(BiddingPrice biddingPrice)
        {
            base.Channel.NotifyBiddingPrice(biddingPrice);
        }

        public void NotifyIndexConclusion(IndexConclusion conclusion)
        {
            base.Channel.NotifyIndexConclusion(conclusion);
        }

        public void NotifyStockConclusion(StockConclusion conclusion)
        {
            base.Channel.NotifyStockConclusion(conclusion);
        }
    }
}
