using MTree.DataStructure;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.RealTimeProvider
{
    public class CodeMapBuilder
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private Dictionary<string, object> codeMapHeader;

        public CodeMapBuilder()
        {
            codeMapHeader = new Dictionary<string, object>();
            //Dictionary<string, object> rebuilt = CodeMapBuilderUtil.RebuildNode(CodeMapBuilderUtil.ConvertToJsonString(codeMapHeader));
        }

        public Dictionary<string, object> GetCodeMap()
        {
            return codeMapHeader;
        }

        public string GetCodeMapAsJsonString()
        {
            return CodeMapBuilderUtil.ConvertToJsonString(codeMapHeader);
        }

        public void AddMarketTypeMap(PublisherContract contract)
        {
            try
            {
                var codeList = contract.Callback.GetCodeList();
                CodeMapBuilderUtil.BuildNode(codeMapHeader, "MarketType", BuildMarketTypeMap(codeList));
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public void AddThemeMap(PublisherContract contract, string name)
        {
            try
            {
                var theme = CodeMapBuilderUtil.BuildNode(codeMapHeader, "Theme");
                CodeMapBuilderUtil.BuildNode(theme, name, contract.Callback.GetThemeList());
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private Dictionary<string, object> BuildMarketTypeMap(Dictionary<string, CodeEntity> codeList)
        {
            Dictionary<string, object> ret = new Dictionary<string, object>();
            try
            {
                foreach (KeyValuePair<string, CodeEntity> codeEntity in codeList)
                {
                    if (codeEntity.Value.MarketType != MarketTypes.INDEX &&
                        codeEntity.Value.MarketType != MarketTypes.ELW)
                    {
                        if (ret.ContainsKey(codeEntity.Value.MarketType.ToString()) == false)
                        {
                            ret.Add(codeEntity.Value.MarketType.ToString(), new Dictionary<string, object>());
                        }
                        ((Dictionary<string, object>)ret[codeEntity.Value.MarketType.ToString()]).Add(codeEntity.Value.Code, codeEntity.Value.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return ret;
        }
    }
}
