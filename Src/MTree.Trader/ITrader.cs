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
        [OperationContract]
        List<string> GetAccountList();

        [OperationContract]
        int GetDeposit(string accountCode);

        [OperationContract]
        List<HoldingStock> GetHoldingList(string accountCode);

        [OperationContract]
        OrderResult MakeOrder(Order order);
    }

    public interface ITraderCallback
    {

    }
}
