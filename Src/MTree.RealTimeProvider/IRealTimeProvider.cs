using System;
using System.ServiceModel;
using MTree.DataStructure;

namespace MTree.RealTimeProvider
{
    [ServiceContract(CallbackContract = typeof(IRealTimeProviderCallback))]
    public interface IRealTimeProvider
    {
        [OperationContract(IsOneWay = true)]
        void KeepConnection();
    }

    public interface IRealTimeProviderCallback
    {
        [OperationContract(IsOneWay = true)]
        void ConsumeConclusion(StockConclusion conclusion);
    }
}
