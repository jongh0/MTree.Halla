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
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private Dictionary<string, object> codeMapHead;

        public CodeMapBuilder()
        {
            codeMapHead = new Dictionary<string, object>();
        }

        public Dictionary<string, object> GetCodeMap()
        {
            return codeMapHead;
        }

        public string GetCodeMapAsJsonString()
        {
            return CodeMapBuilderUtil.ConvertToJsonString(codeMapHead);
        }

        public void AddMarketTypeMap(PublisherContract contract)
        {
            try
            {
                var codeList = contract.Callback.GetCodeList();
                CodeMapBuilderUtil.BuildNode(codeMapHead, "MarketType", BuildMarketTypeMap(codeList));
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        public void AddBizTypeMap(PublisherContract contract)
        {
            try
            {
                CodeMapBuilderUtil.BuildNode(codeMapHead, "BizType", contract.Callback.GetCodeMap(CodeMapTypes.BizType));
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        public void AddCapitalScaleMap(PublisherContract contract)
        {
            try
            {
                CodeMapBuilderUtil.BuildNode(codeMapHead, "CapitalScale", contract.Callback.GetCodeMap(CodeMapTypes.CapitalScale));
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        public void AddGroupMap(PublisherContract contract)
        {
            try
            {
                CodeMapBuilderUtil.BuildNode(codeMapHead, "Group", contract.Callback.GetCodeMap(CodeMapTypes.Group));
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        public void AddThemeMap(PublisherContract contract, string publisherName)
        {
            try
            {
                var theme = CodeMapBuilderUtil.BuildNode(codeMapHead, "Theme");
                CodeMapBuilderUtil.BuildNode(theme, publisherName, contract.Callback.GetCodeMap(CodeMapTypes.Theme));
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
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
                _logger.Error(ex);
            }
            return ret;
        }
    }
}
