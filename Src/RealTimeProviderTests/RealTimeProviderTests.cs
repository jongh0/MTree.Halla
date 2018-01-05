using Microsoft.VisualStudio.TestTools.UnitTesting;
using CommonLib;
using System.ServiceModel;
using System.Threading;

namespace RealTimeProvider.Tests
{
    [TestClass()]
    public class RealTimeProviderTests
    {
        [TestMethod()]
        public void LaunchPublisherTest()
        {
            var instance = new RealTimeProvider_();
            ServiceHost RealTimeHost = new ServiceHost(instance);
            RealTimeHost.Open();
            
            //instance.LaunchPublisher(PublisherType.DaishinMaster);
            //for (int i = 0; i < 5; i++)
            //{
            //    instance.LaunchPublisher(PublisherType.Daishin);
            //}

            //while (true) ;
            //Assert.Fail();
        }
    }
}