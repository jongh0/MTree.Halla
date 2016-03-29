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
using System.Threading;
using System.Windows;

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
        private StockMstClass indexMstObj;
        private StockCurClass stockCurObj;
        private StockOutCurClass stockOutCurObj;
        private StockJpbidClass stockJpbidObj;
        private StockChartClass stockChartObj;
        private CpSvr8091SClass memberTrendObj;

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

                indexMstObj = new StockMstClass();
                indexMstObj.Received += IndexMasterReceived;

                stockCurObj = new StockCurClass();
                stockCurObj.Received += stockCurObj_Received;

                stockOutCurObj = new StockOutCurClass();
                stockOutCurObj.Received += StockOutCurObj_Received;

                stockJpbidObj = new StockJpbidClass();
                stockJpbidObj.Received += stockJpbidObj_Received;

                stockChartObj = new StockChartClass();
                stockChartObj.Received += stockChartObj_Received;

                memberTrendObj = new CpSvr8091SClass();
                memberTrendObj.Received += MemberTrendObj_Received;

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
#if false // Member Subscribing test
                if (Environment.GetCommandLineArgs()[1] == "DaishinMaster")
                {
                    memberTrendObj.SetInputValue(0, "*");
                    memberTrendObj.SetInputValue(1, "*");
                    memberTrendObj.Subscribe();
                }
#endif
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
        
        int cnt = 0;
        private void MemberTrendObj_Received()
        {
            cnt++;
            string str = $"Time:{memberTrendObj.GetHeaderValue(0)},Member:{memberTrendObj.GetHeaderValue(1)},Code:{memberTrendObj.GetHeaderValue(2)},Name:{memberTrendObj.GetHeaderValue(3)},Sell/Buy:{memberTrendObj.GetHeaderValue(4)},Amount: { memberTrendObj.GetHeaderValue(5)},Accum Amount:{ memberTrendObj.GetHeaderValue(6)},Sign: { memberTrendObj.GetHeaderValue(7)},Forien: { memberTrendObj.GetHeaderValue(8)}";
            if (cnt % 100 == 0)
            {
                logger.Info(str);
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

        private void sessionObj_OnDisconnect()
        {
            logger.Error("Disconnected");
        }

        private void stockMstObj_Received()
        {
            StockMasterReceived();
        }

        private void stockCurObj_Received()
        {
            string fullcode = stockCurObj.GetHeaderValue(0).ToString();

            if (fullcode[0] == 'U')
                IndexConclusionReceived();
            else
                StockConclusionReceived();
        }

        private void stockJpbidObj_Received()
        {
            BiddingPriceReceived();
        }

        private void stockChartObj_Received()
        {
            StockChartReceived();
        }

        public override bool IsSubscribable()
        {
            int remainCount = sessionObj.GetLimitRemainCount(LIMIT_TYPE.LT_SUBSCRIBE);
            return remainCount > 0;
        }

        public override void NotifyMessage(MessageTypes type, string message)
        {
            if (type == MessageTypes.CloseClient)
            {
                Task.Run(() =>
                {
                    logger.Info("Process will be closed");
                    Thread.Sleep(1000 * 10);
                    //Application.Current.Shutdown();
                    Environment.Exit(0);
                });
            }

            base.NotifyMessage(type, message);
        }

        public override string GetMarketInfo(MarketInfoTypes type)
        {
            try
            {
                if (type == MarketInfoTypes.WorkDate)
                {
                    var date = codeMgrObj.GetWorkDate();
                    return date;
                }
                else if (type == MarketInfoTypes.StartTime)
                {
                    var time = codeMgrObj.GetMarketStartTime();
                    return time.ToString();
                }
                else if (type == MarketInfoTypes.EndTime)
                {
                    var time = codeMgrObj.GetMarketEndTime();
                    return time.ToString();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return base.GetMarketInfo(type);
        }
    }
}
