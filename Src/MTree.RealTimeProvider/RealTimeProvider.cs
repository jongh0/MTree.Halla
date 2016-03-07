using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using MTree.DataStructure;

namespace MTree.RealTimeProvider
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public partial class RealTimeProvider : IRealTimePublisher, IRealTimeConsumer
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public void KeepConnection()
        {
            throw new NotImplementedException();
        }
    }
}
