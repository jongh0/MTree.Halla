using MTree.RealTimeProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace MTree.Trader
{
    [ServiceContract(CallbackContract = typeof(ITraderCallback))]
    public interface ITrader
    {
        [OperationContract(IsOneWay = true)]
        void NotifyMessage(MessageTypes type, string message);

        [OperationContract(IsOneWay = true)]
        void RegisterContract(Guid clientId, TraderContract contract);

        [OperationContract(IsOneWay = true)]
        void UnregisterContract(Guid clientId);

        [OperationContract]
        List<string> GetAccountList();

        [OperationContract]
        long GetDeposit(string accNum, string accPw);

        [OperationContract]
        List<HoldingStock> GetHoldingList(string accNum);

        [OperationContract]
        bool MakeOrder(Order order);
    }

    public interface ITraderCallback
    {
        [OperationContract(IsOneWay = true)]
        void NotifyMessage(MessageTypes type, string message);

        [OperationContract(IsOneWay = true)]
        void NotifyOrderResult(OrderResult result);
    }
}
