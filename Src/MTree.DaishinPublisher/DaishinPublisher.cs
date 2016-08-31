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
        
        #region Daishin Specific
        private CpCybosClass sessionObj;
        private CpCodeMgrClass codeMgrObj;
        private StockMstClass stockMstObj;
        private StockMstClass indexMstObj;
        private StockCurClass stockCurObj;
        private StockCurClass indexCurObj;
        private CpSvrNew7244SClass etfCurObj;
        private StockOutCurClass stockOutCurObj;
        private StockJpbidClass stockJpbidObj;
        private StockChartClass stockChartObj;
        private CpSvr8091SClass memberTrendObj;
        private CpSvr8561Class themeListObj;
        private CpSvr8561TClass themeTypeObj;
        #endregion

        public DaishinPublisher()
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
                indexMstObj.Received += indexMstObj_Received;

                stockCurObj = new StockCurClass();
                stockCurObj.Received += stockCurObj_Received;

                indexCurObj = new StockCurClass();
                indexCurObj.Received += indexCurObj_Received;

                etfCurObj = new CpSvrNew7244SClass();
                etfCurObj.Received += etfCurObj_Received;

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

                if (Environment.GetCommandLineArgs()[1] == ProcessTypes.DaishinPublisherMaster.ToString())
                    IsMasterProcess = true;

#if false // Chart test code
                var chart = GetChart("A000020", new DateTime(2015, 3, 4), new DateTime(2015, 3, 4), ChartTypes.Min);
                logger.Info($"Candle count: {chart.Count}");
                Debugger.Break();
#endif
#if false // Member Subscribing test
                if (Environment.GetCommandLineArgs()[1] == ProcessTypes.DaishinPublisherMaster.ToString())
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
        private void memberTrendObj_Received()
        {
            cnt++;
            string str = $"Time: {memberTrendObj.GetHeaderValue(0)}, Member: {memberTrendObj.GetHeaderValue(1)}, Code: {memberTrendObj.GetHeaderValue(2)}, Name: {memberTrendObj.GetHeaderValue(3)}, Sell/Buy: {memberTrendObj.GetHeaderValue(4)}, Amount: { memberTrendObj.GetHeaderValue(5)}, Accum Amount: { memberTrendObj.GetHeaderValue(6)}, Sign: { memberTrendObj.GetHeaderValue(7)}, Forien: { memberTrendObj.GetHeaderValue(8)}";
            if (cnt % 100 == 0)
            {
                logger.Info(str);
            }
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
