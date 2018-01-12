using Consumer;
using RealTimeProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using DataStructure;
using CommonLib;
using System.Threading;
using System.Diagnostics;
using Configuration;

namespace TestConsumer
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class TestConsumer : RealTimeConsumer
    {
        //int count = 0;
        double tick = 0;

        public void StopConsume()
        {
            StopQueueTask();
            CloseChannel();
        }

        protected override void ServiceClient_Opened(object sender, EventArgs e)
        {
            base.ServiceClient_Opened(sender, e);

            Task.Run(() =>
            {
                RegisterContract(new SubscribeContract(SubscribeTypes.StockConclusion));
                RegisterContract(new SubscribeContract(SubscribeTypes.IndexConclusion));
                if (Config.General.SkipBiddingPrice == false)
                    RegisterContract(new SubscribeContract(SubscribeTypes.BiddingPrice));
            });
        }

        protected override void ProcessStockConclusionQueue()
        {
            if (StockConclusionQueue.TryDequeue(out var conclusion) == true)
            {
                var interval = (DateTime.Now - conclusion.Time);
                tick += interval.TotalMilliseconds;

                //Console.WriteLine($"{interval.ToString()}, {conclusion.Price}, {conclusion.Code} consumed");
            }
            else
                Thread.Sleep(10);
        }

        protected override void ProcessIndexConclusionQueue()
        {
            if (IndexConclusionQueue.TryDequeue(out var conclusion) == true)
            {
                var interval = (DateTime.Now - conclusion.Time);
                tick += interval.TotalMilliseconds;

                //Console.WriteLine($"{interval.ToString()}, {conclusion.Price}, {conclusion.Code} consumed");
            }
            else
                Thread.Sleep(10);
        }

        protected override void ProcessBiddingPriceQueue()
        {
            if (BiddingPriceQueue.TryDequeue(out var biddingPrice) == true)
            {
                var interval = (DateTime.Now - biddingPrice.Time);
                tick += interval.TotalMilliseconds;

                //Console.WriteLine($"{interval.ToString()}, {biddingPrice.Code} consumed");
            }
            else
                Thread.Sleep(10);
        }
    }
}
