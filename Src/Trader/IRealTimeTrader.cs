using RealTimeProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Trader
{
    [ServiceContract(CallbackContract = typeof(IRealTimeTraderCallback))]
    public interface IRealTimeTrader
    {
        [OperationContract]
        void NotifyMessage(MessageTypes type, string message);

        [OperationContract]
        void RegisterTraderContract(Guid clientId, TraderContract contract);

        [OperationContract]
        void UnregisterTraderContract(Guid clientId);

        [OperationContract]
        List<string> GetAccountList();

        [OperationContract]
        long GetDeposit(string accNum, string accPw);

        [OperationContract]
        List<HoldingStock> GetHoldingList(string accNum);

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
