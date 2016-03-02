using System;
using System.ServiceModel;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;
using MTree.DataStructure;
using MTree.RealTimeProvider;

namespace TestConsole
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    class TestConsoleCallback : IRealTimeProviderCallback
    {
        public void ConsumeConclusion(StockConclusion conclusion)
        {
        }
    }
}
