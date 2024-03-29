﻿using RealTimeProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DataStructure;
using System.ServiceModel;

namespace Publisher
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class PublisherCallback : SubscribingBase, IRealTimePublisherCallback
    {
        public virtual void NotifyMessage(MessageTypes type, string message)
        {
        }

        public virtual Dictionary<string, CodeEntity> GetCodeList()
        {
            return null;
        }

        public virtual Dictionary<string, object> GetCodeMap(CodeMapTypes codemapType)
        {
            return null;
        }

        public virtual StockMaster GetStockMaster(string code)
        {
            return null;
        }

        public virtual IndexMaster GetIndexMaster(string code)
        {
            return null;
        }

        public virtual List<Candle> GetChart(string code, DateTime startDate, DateTime endDate, CandleTypes chartType)
        {
            return null;
        }

        public virtual string GetMarketInfo(MarketInfoTypes type)
        {
            return string.Empty;
        }

        public virtual bool IsSubscribable()
        {
            return true;
        }

        public virtual bool SubscribeStock(string code)
        {
            return false;
        }

        public virtual bool UnsubscribeStock(string code)
        {
            return false;
        }

        public virtual bool SubscribeIndex(string code)
        {
            return false;
        }

        public virtual bool UnsubscribeIndex(string code)
        {
            return false;
        }

        public virtual bool SubscribeBidding(string code)
        {
            return false;
        }

        public virtual bool UnsubscribeBidding(string code)
        {
            return false;
        }

        public virtual bool SubscribeCircuitBreak(string code)
        {
            return false;
        }

        public virtual bool UnsubscribeCircuitBreak(string code)
        {
            return false;
        }

        public virtual bool SubscribeETF(string code)
        {
            return false;
        }

        public virtual bool UnsubscribeETF(string code)
        {
            return false;
        }
    }
}
