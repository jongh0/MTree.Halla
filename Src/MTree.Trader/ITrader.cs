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
        List<string> GetAccounts();

        [OperationContract]
        int GetDeposit(string account);

        [OperationContract]
        List<HoldingStock> GetHoldingStocks(string account);

        [OperationContract]
        OrderResult MakeOrder(Order order);
    }

    public interface ITraderCallback
    {

    }
}
