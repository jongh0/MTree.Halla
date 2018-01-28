using RealTimeProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Trader.Account;

namespace Trader
{
    [ServiceContract(CallbackContract = typeof(IRealTimeTraderCallback))]
    public interface IRealTimeTrader
    {
        [OperationContract]
        void SendMessage(MessageTypes type, string message);

        [OperationContract]
        void RegisterTraderContract(Guid clientId, TraderContract contract);

        [OperationContract]
        void UnregisterTraderContract(Guid clientId);

        [OperationContract]
        List<AccountInformation> GetAccountInformations();

        [OperationContract]
        bool MakeOrder(Order order);
    }

    public interface IRealTimeTraderCallback
    {
        [OperationContract]
        void NotifyMessage(MessageTypes type, string message);

        [OperationContract]
        void NotifyOrderResult(OrderResult result);
    }
}
