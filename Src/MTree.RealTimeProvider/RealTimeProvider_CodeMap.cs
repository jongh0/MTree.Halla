using MTree.DataStructure;
using MTree.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.RealTimeProvider
{
    public partial class RealTimeProvider
    {
        private CodeMapBuilder codeMapBuilder;
        private bool isMarketTypeHandling = false;
        private bool isMarketTypeHandled = false;
        private bool isDaishinThemeHandling = false;
        private bool isDaishinThemeHandled = false;
        private bool isEbestThemeHandling = false;
        private bool isEbestThemeHandled = false;
        private bool isKiwoomThemeHandling = false;
        private bool isKiwoomThemeHandled = false;

        private void ProcessCodeMapBuilding(Guid clientId, PublisherContract contract)
        {
            if (codeMapBuilder == null)
                codeMapBuilder = new CodeMapBuilder();

            if (isMarketTypeHandling == false && contract.Type == ProcessTypes.DaishinPublisher)
            {
                isMarketTypeHandling = true;
                logger.Info($"Build MarketType Map from {clientId}");
                codeMapBuilder.AddMarketTypeMap(contract);
                isMarketTypeHandled = true;
            }
            if (isDaishinThemeHandling == false && contract.Type == ProcessTypes.DaishinPublisher)
            {
                isDaishinThemeHandling = true;
                logger.Info($"Build Daishin Theme Map from {clientId}");
                codeMapBuilder.AddThemeMap(contract);
                isDaishinThemeHandled = true;
            }
            if (isEbestThemeHandling == false && contract.Type == ProcessTypes.EbestPublisher)
            {
                isEbestThemeHandling = true;
                logger.Info($"Build Ebest Theme Map from {clientId}");
                codeMapBuilder.AddThemeMap(contract);
                isEbestThemeHandled = true;
            }
            if (isKiwoomThemeHandling == false && contract.Type == ProcessTypes.KiwoomPublisher)
            {
                isKiwoomThemeHandling = true;
                logger.Info($"Build Kiwoom Theme Map from {clientId}");
                codeMapBuilder.AddThemeMap(contract);
                isKiwoomThemeHandled = true;
            }

            if (isMarketTypeHandled = true && isDaishinThemeHandled == true && isEbestThemeHandled == true && isKiwoomThemeHandled == true)
            {
                var jsonString = codeMapBuilder.GetCodeMapAsJsonString();
                Dictionary<string, object> rebuilt = CodeMapBuilderUtil.RebuildNode(jsonString);
            }
        }
    }
}
