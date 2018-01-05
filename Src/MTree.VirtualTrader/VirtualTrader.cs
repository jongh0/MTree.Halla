using MTree.RealTimeProvider;
using MTree.Trader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MTree.VirtualTrader
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Single, ValidateMustUnderstand = false)]
    public partial class VirtualTrader : ITrader
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public void NotifyMessage(MessageTypes type, string message)
        {
        }

        public void RegisterContract(Guid clientId, TraderContract contract)
        {
        }

        public void UnregisterContract(Guid clientId)
        {
        }
    }
}
