using CPUTILLib;
using DSCBO1Lib;
using CPSYSDIBLib;
using MTree.DataStructure;
using MTree.Publisher;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Diagnostics;
using MTree.RealTimeProvider;
using System.Threading.Tasks;

namespace MTree.DaishinPublisher
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public partial class DaishinPublisher : BrokerageFirmBase
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        #region Daishin Specific
        private CpCybosClass sessionObj;
        private CpCodeMgrClass codeMgrObj;
        private StockMstClass stockMstObj;
        private StockCurClass stockCurObj;
        private StockJpbidClass stockJpbidObj;
        private StockChartClass stockChartObj;
        #endregion

        public DaishinPublisher() : base()
        {
            try
            {
                QuoteInterval = 15 * 1000 / 60; // 15초당 60개

                sessionObj = new CpCybosClass();
                sessionObj.OnDisconnect += sessionObj_OnDisconnect;

                codeMgrObj = new CpCodeMgrClass();

                stockMstObj = new StockMstClass();
                stockMstObj.Received += stockMstObj_Received;

                stockCurObj = new StockCurClass();
                stockCurObj.Received += stockCurObj_Received;

                stockJpbidObj = new StockJpbidClass();
                stockJpbidObj.Received += stockJpbidObj_Received;

                stockChartObj = new StockChartClass();
                stockChartObj.Received += stockChartObj_Received;

                if (sessionObj.IsConnect != 1)
                {
                    logger.Error("Session not connected");
                    return;
                }

                logger.Info($"Server type: {sessionObj.ServerType}");

                StartBiddingPriceQueueTask();
                StartStockConclusionQueueTask();
                StartIndexConclusionQueueTask();

#if false // Chart test code
                var chart = GetChart("A000020", new DateTime(2015, 3, 4), new DateTime(2015, 3, 4), ChartTypes.Min);
                logger.Info($"Candle count: {chart.Count}");
                Debugger.Break();
#endif
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
        
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
                #endregion

                logger.Info($"Code list query done, Count: {codeList.Count}");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return codeList;
        }

        private void sessionObj_OnDisconnect()
        {
            logger.Info("Disconnected");
        }

        private void stockMstObj_Received()
        {
            LastFirmCommunicateTick = Environment.TickCount;
            StockMasterReceived();
        }

        private void stockCurObj_Received()
        {
            LastFirmCommunicateTick = Environment.TickCount;
            string fullcode = stockCurObj.GetHeaderValue(0).ToString();
            if (fullcode[0] == 'U')
            {
                IndexConclusionReceived();
            }
            else
            {
                StockConclusionReceived();
            }
        }

        private void stockJpbidObj_Received()
        {
            LastFirmCommunicateTick = Environment.TickCount;
            BiddingPriceReceived();
        }

        private void stockChartObj_Received()
        {
            LastFirmCommunicateTick = Environment.TickCount;
            StockChartReceived();
        }

        public override bool IsSubscribable()
        {
            int remainCount = sessionObj.GetLimitRemainCount(LIMIT_TYPE.LT_SUBSCRIBE);
            return remainCount > 0;
        }

        public override void NotifyMessage(MessageTypes type, string message)
        {
            if (type == MessageTypes.MarketStartTime)
            {
                Task.Run(() =>
                {
                    short time = codeMgrObj.GetMarketStartTime();
                    ServiceClient.NotifyMessage(MessageTypes.MarketStartTime, time.ToString());
                });
            }
            else if (type == MessageTypes.MarketEndTime)
            {
                Task.Run(() =>
                {
                    short time = codeMgrObj.GetMarketEndTime();
                    ServiceClient.NotifyMessage(MessageTypes.MarketEndTime, time.ToString());
                });
            }

            base.NotifyMessage(type, message);
        }
    }
}
