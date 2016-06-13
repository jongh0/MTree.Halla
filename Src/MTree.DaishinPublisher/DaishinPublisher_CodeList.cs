using CPUTILLib;
using MTree.DataStructure;
using MTree.RealTimeProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.DaishinPublisher
{
    public partial class DaishinPublisher
    {
        private Dictionary<string, object> bizTypeCodeMap = new Dictionary<string, object>();

        private Dictionary<string, object> capitalScaleCodeMap = new Dictionary<string, object>();

        public override Dictionary<string, CodeEntity> GetCodeList()
        {
            var codeList = new Dictionary<string, CodeEntity>();

            try
            {
                #region Index
                foreach (string fullCode in (object[])codeMgrObj.GetIndustryList())
                {
                    var codeEntity = new CodeEntity();
                    codeEntity.Code = CodeEntity.RemovePrefix(fullCode);
                    codeEntity.Name = codeMgrObj.GetIndustryName(fullCode);
                    codeEntity.MarketType = MarketTypes.INDEX;

                    if (codeList.ContainsKey(codeEntity.Code) == false)
                        codeList.Add(codeEntity.Code, codeEntity);
                    else
                        logger.Error($"{codeEntity.Code} code already exists");
                }

                foreach (string fullCode in (object[])codeMgrObj.GetKosdaqIndustry1List())
                {
                    var codeEntity = new CodeEntity();
                    codeEntity.Code = CodeEntity.RemovePrefix(fullCode);
                    codeEntity.Name = codeMgrObj.GetIndustryName(fullCode);
                    codeEntity.MarketType = MarketTypes.INDEX;

                    if (codeList.ContainsKey(codeEntity.Code) == false)
                        codeList.Add(codeEntity.Code, codeEntity);
                    else
                        logger.Error($"{codeEntity.Code} code already exists");
                }

                foreach (string fullCode in (object[])codeMgrObj.GetKosdaqIndustry2List())
                {
                    var codeEntity = new CodeEntity();
                    codeEntity.Code = CodeEntity.RemovePrefix(fullCode);
                    codeEntity.Name = codeMgrObj.GetIndustryName(fullCode);
                    codeEntity.MarketType = MarketTypes.INDEX;

                    if (codeList.ContainsKey(codeEntity.Code) == false)
                        codeList.Add(codeEntity.Code, codeEntity);
                    else
                        logger.Error($"{codeEntity.Code} code already exists");
                }
                #endregion

                #region KOSPI & ETF & ETN
                foreach (string fullCode in (object[])codeMgrObj.GetStockListByMarket(CPE_MARKET_KIND.CPC_MARKET_KOSPI))
                {
                    var codeEntity = new CodeEntity();
                    codeEntity.Code = CodeEntity.RemovePrefix(fullCode);
                    codeEntity.Name = codeMgrObj.CodeToName(fullCode);

                    if (codeMgrObj.GetStockSectionKind(fullCode) == CPE_KSE_SECTION_KIND.CPC_KSE_SECTION_KIND_ETF)
                        codeEntity.MarketType = MarketTypes.ETF;
                    else if (fullCode[0] == 'Q')
                        codeEntity.MarketType = MarketTypes.ETN;
                    else
                        codeEntity.MarketType = MarketTypes.KOSPI;

                    if (codeList.ContainsKey(codeEntity.Code) == false)
                        codeList.Add(codeEntity.Code, codeEntity);
                    else
                        logger.Error($"{codeEntity.Code} code already exists");
                }
                #endregion

                #region KOSDAQ
                foreach (string fullCode in (object[])codeMgrObj.GetStockListByMarket(CPE_MARKET_KIND.CPC_MARKET_KOSDAQ))
                {
                    var codeEntity = new CodeEntity();
                    codeEntity.Code = CodeEntity.RemovePrefix(fullCode);
                    codeEntity.Name = codeMgrObj.CodeToName(fullCode);
                    codeEntity.MarketType = MarketTypes.KOSDAQ;

                    if (codeList.ContainsKey(codeEntity.Code) == false)
                        codeList.Add(codeEntity.Code, codeEntity);
                    else
                        logger.Error($"{codeEntity.Code} code already exists");
                }
                #endregion

                #region KONEX
                foreach (string fullCode in (object[])codeMgrObj.GetStockListByMarket((CPE_MARKET_KIND)5))
                {
                    var codeEntity = new CodeEntity();
                    codeEntity.Code = CodeEntity.RemovePrefix(fullCode);
                    codeEntity.Name = codeMgrObj.CodeToName(fullCode);
                    codeEntity.MarketType = MarketTypes.KONEX;

                    if (codeList.ContainsKey(codeEntity.Code) == false)
                        codeList.Add(codeEntity.Code, codeEntity);
                    else
                        logger.Error($"{codeEntity.Code} code already exists");
                }
                #endregion

                #region Freeboard
                foreach (string fullCode in (object[])codeMgrObj.GetStockListByMarket(CPE_MARKET_KIND.CPC_MARKET_FREEBOARD))
                {
                    var codeEntity = new CodeEntity();
                    codeEntity.Code = CodeEntity.RemovePrefix(fullCode);
                    codeEntity.Name = codeMgrObj.CodeToName(fullCode);
                    codeEntity.MarketType = MarketTypes.FREEBOARD;

                    if (codeList.ContainsKey(codeEntity.Code) == false)
                        codeList.Add(codeEntity.Code, codeEntity);
                    else
                        logger.Error($"{codeEntity.Code} code already exists");
                }
                #endregion

                #region ELW
#if false
                var elwCodeMgr = new CpElwCodeClass();
                int cnt = elwCodeMgr.GetCount();
                for (int i = 0; i < cnt; i++)
                {
                    string fullCode = elwCodeMgr.GetData(0, (short)i).ToString();
                    if (fullCode.Length == 0)
                        continue;

                    var codeEntity = new CodeEntity();
                    codeEntity.Code = CodeEntity.RemovePrefix(fullCode);
                    codeEntity.Name = codeMgrObj.CodeToName(fullCode);
                    codeEntity.MarketType = MarketTypes.ELW;

                    if (codeList.ContainsKey(codeEntity.Code) == false)
                        codeList.Add(codeEntity.Code, codeEntity);
                    else
                        logger.Error($"{codeEntity.Code} code already exists");
                }
#endif
                #endregion

                logger.Info($"Code list query done, Count: {codeList.Count}");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return codeList;
        }

        public override Dictionary<string, object> GetCodeMap(CodeMapTypes codemapType)
        {
            if (codemapType == CodeMapTypes.Theme)
                return GetThemeCodeMap();
            else if (codemapType == CodeMapTypes.BizType)
                return bizTypeCodeMap;
            else if (codemapType == CodeMapTypes.CapitalScale)
                return capitalScaleCodeMap;
            else if (codemapType == CodeMapTypes.Group)
                return GetGroupCodeMap();
            else
                return null;
        }

        private Dictionary<string, object> GetThemeCodeMap()
        {
            var themeList = new Dictionary<string, object>();

            try
            {
                logger.Info($"Theme list query start");

                short ret = themeListObj.BlockRequest();
                if (ret == 0)
                {
                    short listCount = (short)themeListObj.GetHeaderValue(0);
                    for (int i = 0; i < listCount; i++)
                    {
                        short themeCode = (short)themeListObj.GetDataValue(0, i);
                        string themeName = (string)themeListObj.GetDataValue(2, i);
                        themeList.Add(themeName, new Dictionary<string, object>());

                        WaitQuoteInterval();
                        themeTypeObj.SetInputValue(0, themeCode);
                        ret = themeTypeObj.BlockRequest();
                        if (ret == 0)
                        {
                            short itemCount = (short)themeTypeObj.GetHeaderValue(1);
                            for (int j = 0; j < itemCount; j++)
                            {
                                string fullcode = (string)themeTypeObj.GetDataValue(0, j);
                                string name = (string)themeTypeObj.GetDataValue(1, j);
                                ((Dictionary<string, object>)themeList[themeName]).Add(CodeEntity.RemovePrefix(fullcode), name);
                            }
                        }
                    }
                    logger.Info($"Theme list query done, theme list count : {listCount}");
                }
                else
                {
                    logger.Error($"Theme list query fail");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return themeList;
        }

        private Dictionary<string, object> GetGroupCodeMap()
        {
            var groupCodemap = new Dictionary<string, object>();

            foreach (string groupCode in (object[])codeMgrObj.GetGroupList())
            {
                string groupName = codeMgrObj.GetGroupName(groupCode);
                groupCodemap.Add(groupName, new Dictionary<string, object>());
                foreach (string code in (object[])codeMgrObj.GetGroupCodeList(Convert.ToInt32(groupCode)))
                {
                    ((Dictionary<string, object>)groupCodemap[groupName]).Add(code, codeMgrObj.CodeToName(code));
                }
            }

            return groupCodemap;
        }
    }
}
