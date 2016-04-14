#define VERIFY_LATENCY

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
using System.ComponentModel;
using MTree.Utility;

namespace MTree.DaishinPublisher
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public partial class DaishinPublisher : BrokerageFirmBase, INotifyPropertyChanged
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private bool IsMasterProcess { get; set; } = false;

#if VERIFY_LATENCY
        public DataCounter Counter { get; set; } = new DataCounter(DataTypes.DaishinPublisher);

        public TrafficMonitor TrafficMonitor { get; set; }

        private System.Timers.Timer RefreshTimer { get; set; }
#endif

        #region Daishin Specific
        private CpCybosClass sessionObj;
        private CpCodeMgrClass codeMgrObj;
        private StockMstClass stockMstObj;
        private StockMstClass indexMstObj;
        private StockCurClass stockCurObj;
        private StockCurClass indexCurObj;
        private StockOutCurClass stockOutCurObj;
        private StockJpbidClass stockJpbidObj;
        private StockChartClass stockChartObj;
        private CpSvr8091SClass memberTrendObj;
        private CpSvr8561Class themeListObj;
        private CpSvr8561TClass themeTypeObj;
        #endregion

        public DaishinPublisher() : base()
        {
            try
            {
#if VERIFY_LATENCY
                TrafficMonitor = new TrafficMonitor(Counter);
                StartRefreshTimer();
#endif 
                QuoteInterval = 15 * 1000 / 60; // 15초당 60개

                sessionObj = new CpCybosClass();
                sessionObj.OnDisconnect += sessionObj_OnDisconnect;

                codeMgrObj = new CpCodeMgrClass();

                stockMstObj = new StockMstClass();
                stockMstObj.Received += stockMstObj_Received;

                indexMstObj = new StockMstClass();
                indexMstObj.Received += indexMstObj_Received;

                stockCurObj = new StockCurClass();
                stockCurObj.Received += stockCurObj_Received;

                indexCurObj = new StockCurClass();
                indexCurObj.Received += indexCurObj_Received;

                stockOutCurObj = new StockOutCurClass();
                stockOutCurObj.Received += stockOutCurObj_Received;

                stockJpbidObj = new StockJpbidClass();
                stockJpbidObj.Received += stockJpbidObj_Received;

                stockChartObj = new StockChartClass();
                stockChartObj.Received += stockChartObj_Received;

                memberTrendObj = new CpSvr8091SClass();
                memberTrendObj.Received += memberTrendObj_Received;

                themeListObj = new CpSvr8561Class();
                themeTypeObj = new CpSvr8561TClass();

                if (sessionObj.IsConnect != 1)
                {
                    logger.Error("Session not connected");
                    return;
                }

                logger.Info($"Server type: {sessionObj.ServerType}");

                StartBiddingPriceQueueTask();
                StartStockConclusionQueueTask();
                StartIndexConclusionQueueTask();

                if (Environment.GetCommandLineArgs()[1] == "DaishinMaster")
                    IsMasterProcess = true;

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

#if VERIFY_LATENCY
        private void StartRefreshTimer()
        {
            if (RefreshTimer == null)
            {
                RefreshTimer = new System.Timers.Timer();
                RefreshTimer.AutoReset = true;
                RefreshTimer.Interval = 10000;
                RefreshTimer.Elapsed += RefreshTimer_Elapsed;
            }

            RefreshTimer?.Start();
        }
        private void StopRefreshTimer()
        {
            RefreshTimer?.Stop();
        }

        private void RefreshTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Counter.NotifyPropertyAll();
            TrafficMonitor.NotifyPropertyAll();
            NotifyPropertyChanged(nameof(BiddingPriceQueueCount));
            NotifyPropertyChanged(nameof(StockConclusionQueueCount));
            NotifyPropertyChanged(nameof(IndexConclusionQueueCount));
        }
#endif

        int cnt = 0;
        private void memberTrendObj_Received()
        {
            cnt++;
            string str = $"Time: {memberTrendObj.GetHeaderValue(0)}, Member: {memberTrendObj.GetHeaderValue(1)}, Code: {memberTrendObj.GetHeaderValue(2)}, Name: {memberTrendObj.GetHeaderValue(3)}, Sell/Buy: {memberTrendObj.GetHeaderValue(4)}, Amount: { memberTrendObj.GetHeaderValue(5)}, Accum Amount: { memberTrendObj.GetHeaderValue(6)}, Sign: { memberTrendObj.GetHeaderValue(7)}, Forien: { memberTrendObj.GetHeaderValue(8)}";
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

        public override Dictionary<string, List<string>> GetThemeList()
        {
            var themeList = new Dictionary<string, List<string>>();

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
                        themeList.Add(themeName, new List<string>());

                        WaitQuoteInterval();
                        themeTypeObj.SetInputValue(0, themeCode);
                        ret = themeTypeObj.BlockRequest();
                        if (ret == 0)
                        {
                            short itemCount = (short)themeTypeObj.GetHeaderValue(1);
                            for (int j = 0; j < itemCount; j++)
                            {
                                string fullcode = (string)themeTypeObj.GetDataValue(0, j);
                                themeList[themeName].Add(CodeEntity.RemovePrefix(fullcode));
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

        private void sessionObj_OnDisconnect()
        {
            logger.Error("Disconnected");

            if (IsMasterProcess == true)
                ServiceClient.NotifyMessage(MessageTypes.DaishinSessionDisconnected, string.Empty);
        }

        public override bool IsSubscribable()
        {
            int remainCount = GetSubscribableCount();
            return remainCount > 0;
        }

        private int GetSubscribableCount()
        {
            return sessionObj.GetLimitRemainCount(LIMIT_TYPE.LT_SUBSCRIBE);
        }

        protected override void ServiceClient_Opened(object sender, EventArgs e)
        {
            base.ServiceClient_Opened(sender, e);

            Task.Run(() =>
            {
                // Contract 등록
                RegisterPublishContract();
            });
        }

        public override void NotifyMessage(MessageTypes type, string message)
        {
            if (type == MessageTypes.CloseClient)
            {
                Task.Run(() =>
                {
                    logger.Info("Process will be closed");
                    Thread.Sleep(1000 * 5);

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

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}
