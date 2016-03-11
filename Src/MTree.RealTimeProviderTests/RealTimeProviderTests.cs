using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ServiceModel;
using System.Threading;

namespace MTree.RealTimeProvider.Tests
{
    [TestClass()]
    public class RealTimeProviderTests
    {
        [TestMethod()]
        public void LaunchPublisherTest()
        {
            var instance = new RealTimeProvider();
            ServiceHost RealTimeHost = new ServiceHost(instance);
            RealTimeHost.Open();
            instance.LaunchPublisher(PublisherType.DaishinMaster);
            for (int i = 0; i < 5; i++)
            {
                instance.LaunchPublisher(PublisherType.Daishin);
                Thread.Sleep(1000);
            }

            while (true) ;
            Assert.Fail();
        }
    }
}