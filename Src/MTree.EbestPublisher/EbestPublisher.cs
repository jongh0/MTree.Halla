using System;
using System.Threading;
using MTree.DataStructure;
using MTree.Configuration;
using MTree.Publisher;
using XA_SESSIONLib;
using XA_DATASETLib;
using System.ServiceModel;
using MTree.RealTimeProvider;
using MTree.Utility;
using System.Timers;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MTree.EbestPublisher
{
    enum WarningTypes1
    {
        AdministrativeIssue = 1,    // 관리
        UnfairAnnouncement = 2,     // 불성실공시
        InvestAttention = 3,        // 투자유의
        CallingAttention = 4,       // 투자환기
    }
    enum WarningTypes2
    { 
        InvestWarning = 1,          // 경고
        TradingHalt = 2,            // 매매정지
        CleaningTrade = 3,          // 정리매매
        InvestCaution = 4,          // 주의
        InvestmentRisk =5 ,         // 위험
        InvestmentRiskNoticed = 6,  // 위험예고
        Overheated = 7,             // 단기과열
        OverheatNoticed = 8,        // 단기과열지정예고
    }

    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class EbestPublisher : BrokerageFirmBase
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private const int maxSubscribeCount = 200;
        private int subscribeCount = 0;

        private readonly string resFilePath = "\\Res";

        #region Ebest Specific
        private XASessionClass sessionObj;
        
        private XARealClass indexSubscribingObj;
        private XARealClass viSubscribingObj;
        private XARealClass dviSubscribingObj;

        private XAQueryClass indexListObj;
        private XAQueryClass codeListObj;
        private XAQueryClass indexQuotingObj;
        private XAQueryClass stockQuotingObj;

        private XAQueryClass warningObj1;
        private XAQueryClass warningObj2;
        #endregion

        private Dictionary<string, string> industryDictionary = new Dictionary<string, string>();
        private Dictionary<string, string> stockDictionary = new Dictionary<string, string>();

        private Dictionary<string, List<string>> warningListDic;
        private string currentUpdatingWarningType;

        private ManualResetEvent WarningListUpdatedEvent { get; } = new ManualResetEvent(false);
        private int WarningListUpdateTimeout { get; } = 1000 * 30;

        public EbestPublisher() : base()
        {
            try
            {
                #region XASession
                sessionObj = new XASessionClass();
                sessionObj.Disconnect += sessionObj_Disconnect;
                sessionObj._IXASessionEvents_Event_Login += sessionObj_Event_Login;
                sessionObj._IXASessionEvents_Event_Logout += sessionObj_Event_Logout;
                #endregion

                #region XAReal
                indexSubscribingObj = new XARealClass();
                indexSubscribingObj.ReceiveRealData += realObj_ReceiveRealData;
                indexSubscribingObj.ResFileName = resFilePath + "\\IJ_.res";

                viSubscribingObj = new XARealClass();
                viSubscribingObj.ReceiveRealData += realObj_ReceiveRealData;
                viSubscribingObj.ResFileName = resFilePath + "\\VI_.res";

                dviSubscribingObj = new XARealClass();
                dviSubscribingObj.ReceiveRealData += realObj_ReceiveRealData;
                dviSubscribingObj.ResFileName = resFilePath + "\\DVI.res";
                #endregion

                #region XAQuery
                indexListObj = new XAQueryClass();
                indexListObj.ResFileName = resFilePath + "\\t8424.res";
                indexListObj.ReceiveChartRealData += queryObj_ReceiveChartRealData;
                indexListObj.ReceiveData += queryObj_ReceiveData;
                indexListObj.ReceiveMessage += queryObj_ReceiveMessage;

                codeListObj = new XAQueryClass();
                codeListObj.ResFileName = resFilePath + "\\t8430.res";
                codeListObj.ReceiveChartRealData += queryObj_ReceiveChartRealData;
                codeListObj.ReceiveData += queryObj_ReceiveData;
                codeListObj.ReceiveMessage += queryObj_ReceiveMessage;

                indexQuotingObj = new XAQueryClass();
                indexQuotingObj.ResFileName = resFilePath + "\\t1511.res";
                indexQuotingObj.ReceiveChartRealData += queryObj_ReceiveChartRealData;
                indexQuotingObj.ReceiveData += queryObj_ReceiveData;
                indexQuotingObj.ReceiveMessage += queryObj_ReceiveMessage;

                stockQuotingObj = new XAQueryClass();
                stockQuotingObj.ResFileName = resFilePath + "\\t1102.res";
                stockQuotingObj.ReceiveChartRealData += queryObj_ReceiveChartRealData;
                stockQuotingObj.ReceiveData += queryObj_ReceiveData;
                stockQuotingObj.ReceiveMessage += queryObj_ReceiveMessage;

                warningObj1 = new XAQueryClass();
                warningObj1.ResFileName = resFilePath + "\\t1404.res";
                warningObj1.ReceiveChartRealData += queryObj_ReceiveChartRealData;
                warningObj1.ReceiveData += queryObj_ReceiveData;
                warningObj1.ReceiveMessage += queryObj_ReceiveMessage;

                warningObj2 = new XAQueryClass();
                warningObj2.ResFileName = resFilePath + "\\t1405.res";
                warningObj2.ReceiveChartRealData += queryObj_ReceiveChartRealData;
                warningObj2.ReceiveData += queryObj_ReceiveData;
                warningObj2.ReceiveMessage += queryObj_ReceiveMessage;
                #endregion

                #region Login
                LoginInstance.UserId = Config.Instance.Ebest.UserId;
                LoginInstance.UserPw = Config.Instance.Ebest.UserPw;
                LoginInstance.CertPw = Config.Instance.Ebest.CertPw;
                LoginInstance.AccountPw = Config.Instance.Ebest.AccountPw;
                LoginInstance.ServerType = Config.Instance.Ebest.ServerType;
                LoginInstance.ServerAddress = Config.Instance.Ebest.ServerAddress;
                LoginInstance.ServerPort = Config.Instance.Ebest.ServerPort;

                if (string.IsNullOrEmpty(LoginInstance.UserId) == false &&
                    string.IsNullOrEmpty(LoginInstance.UserPw) == false &&
                    string.IsNullOrEmpty(LoginInstance.CertPw) == false)
                {
                    Login();
                }
                else
                {
                    logger.Error("Check Ebest configuration");
                    return;
                }
                #endregion

                //StartIndexConclusionQueueTask();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            #region Warning List Update
            var tast = Task.Run(() =>
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                foreach (WarningTypes1 warningType in Enum.GetValues(typeof(WarningTypes1)))
                {
                    GetWarningList(warningType);
                }

                foreach (WarningTypes2 warningType in Enum.GetValues(typeof(WarningTypes2)))
                {
                    GetWarningList(warningType);
                }
                SetWarninglistUpdated();
                sw.Stop();
                logger.Trace(sw.Elapsed);
            });
            #endregion
        }

        #region XAQuery
        private void queryObj_ReceiveMessage(bool bIsSystemError, string nMessageCode, string szMessage)
        {
            LastFirmCommunicateTick = Environment.TickCount;

            if (bIsSystemError == true)
                logger.Error($"bIsSystemError: {bIsSystemError}, nMessageCode: {nMessageCode}, szMessage: {szMessage}");
        }

        private void queryObj_ReceiveData(string szTrCode)
        {
            LastFirmCommunicateTick = Environment.TickCount;
            logger.Info($"szTrCode: {szTrCode}");

            if (szTrCode == "t1102")
                StockMasterReceived();
            else if (szTrCode == "t1511")
                IndexMasterReceived();
            else if (szTrCode == "t8424")
                IndexListReceived();
            else if (szTrCode == "t8430")
                StockListReceived();
            else if (szTrCode == "t1404")
                WarningType1ListReceived();
            else if (szTrCode == "t1405")
                WarningType2ListReceived();
        }

        private void queryObj_ReceiveChartRealData(string szTrCode)
        {
            LastFirmCommunicateTick = Environment.TickCount;
            logger.Info($"szTrCode: {szTrCode}");
        }
        #endregion

        #region XAReal
        private void realObj_ReceiveRealData(string szTrCode)
        {
            LastFirmCommunicateTick = Environment.TickCount;

            if (szTrCode == "IJ_")
                IndexConclusionReceived(szTrCode);
            else if (szTrCode == "VI_")
                VolatilityInterruptionReceived(szTrCode);
            else if (szTrCode == "DVI")
                AfterVolatilityInterruptionReceived(szTrCode);
        }
        #endregion

        #region XASession
        private void sessionObj_Event_Logout()
        {
            LastFirmCommunicateTick = Environment.TickCount;

            LoginInstance.State = LoginStates.Logout;
            logger.Info(LoginInstance.ToString());
        }

        private void sessionObj_Event_Login(string szCode, string szMsg)
        {
            LastFirmCommunicateTick = Environment.TickCount;

            QuoteInterval = 1000 / stockQuotingObj.GetTRCountPerSec("t1102");

            LoginInstance.State = LoginStates.Login;
            SetLogin();

            logger.Info($"{LoginInstance.ToString()}, nszCode: {szCode}, szMsg: {szMsg}");
        }

        private void sessionObj_Disconnect()
        {
            LoginInstance.State = LoginStates.Disconnect;
            logger.Info(LoginInstance.ToString());
        }
        #endregion

        #region Login / Logout
        public bool Login()
        {
            bool ret = false;

            try
            {
                if (sessionObj.ConnectServer(LoginInstance.ServerAddress, LoginInstance.ServerPort) == true)
                {
                    logger.Info("Server connected");

                    if (LoginInstance.ServerType == ServerTypes.Real)
                        ret = sessionObj.Login(LoginInstance.UserId, LoginInstance.UserPw, LoginInstance.CertPw, (int)XA_SERVER_TYPE.XA_REAL_SERVER, true);
                    else
                        ret = sessionObj.Login(LoginInstance.UserId, LoginInstance.UserPw, LoginInstance.CertPw, (int)XA_SERVER_TYPE.XA_SIMUL_SERVER, true);

                    if (ret == true)
                        logger.Info($"Try login with id:{LoginInstance.UserId}");
                    else
                        logger.Error("Login error");
                }
                else
                {
                    logger.Error("Server connection fail");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return ret;
        }

        public bool Logout()
        {
            try
            {
                if (sessionObj.IsConnected() == false)
                    return false;

                if (LoginInstance.State != LoginStates.Login)
                    return false;

                sessionObj.Logout();
                sessionObj.DisconnectServer();

                logger.Info("Logout success");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return false;
            }

            return true;
        }
        #endregion

        public override bool SubscribeIndex(string code)
        {
            if (WaitLogin() == false)
            {
                logger.Error("Not login state");
                return false;
            }

            try
            {
                indexSubscribingObj.SetFieldData("InBlock", "upcode", code);
                indexSubscribingObj.AdviseRealData();

                subscribeCount++;
                logger.Info($"Subscribe index success, Code: {code}, subscribeCount: {subscribeCount}");
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            logger.Error($"Subscribe index fail, Code: {code}");
            return false;
        }

        public override bool UnsubscribeIndex(string code)
        {
            if (WaitLogin() == false)
            {
                logger.Error("Not login state");
                return false;
            }

            try
            {
                indexSubscribingObj.SetFieldData("InBlock", "upcode", code);
                indexSubscribingObj.UnadviseRealData();

                subscribeCount--;
                logger.Info($"Unsubscribe index success, Code: {code}, subscribeCount: {subscribeCount}");
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            logger.Error($"Unsubscribe index fail, Code: {code}");
            return false;
        }

        private void IndexConclusionReceived(string szTrCode)
        {
            try
            {
                var now = DateTime.Now;
                IndexConclusion conclusion = new IndexConclusion();

                string temp = indexSubscribingObj.GetFieldData("OutBlock", "upcode");
                conclusion.Code = temp;

                temp = indexSubscribingObj.GetFieldData("OutBlock", "time");
                uint time;
                if (uint.TryParse(temp, out time) == true)
                    conclusion.Time = new DateTime(now.Year, now.Month, now.Day, (int)(time / 10000), (int)((time / 100) % 100), (int)time % 100, now.Millisecond);
                else
                    conclusion.Time = now;

                temp = indexSubscribingObj.GetFieldData("OutBlock", "jisu");
                double index = 0;
                if (double.TryParse(temp, out index) == false)
                    logger.Error($"Index conclusion index error, {temp}");
                conclusion.Price = (float)index;

                temp = indexSubscribingObj.GetFieldData("OutBlock", "volume");
                long volume = 0;
                if (long.TryParse(temp, out volume) == false)
                    logger.Error($"Index conclusion index error, {temp}");
                conclusion.Amount = volume * 1000;

                temp = indexSubscribingObj.GetFieldData("OutBlock", "value");
                long value;
                if (long.TryParse(temp, out value) == false)
                    logger.Error($"Index conclusion value error, {temp}");
                conclusion.MarketCapitalization = value;

                temp = indexSubscribingObj.GetFieldData("OutBlock", "upjo");
                int upperLimitCount;
                if (int.TryParse(temp, out upperLimitCount) == false)
                    logger.Error($"Index conclusion upper limit count error, {temp}");
                conclusion.UpperLimitedItemCount = upperLimitCount;

                temp = indexSubscribingObj.GetFieldData("OutBlock", "highjo");
                int increasingCount;
                if (int.TryParse(temp, out increasingCount) == false)
                    logger.Error($"Index conclusion increasing count error, {temp}");
                conclusion.IncreasingItemCount = increasingCount;

                temp = indexSubscribingObj.GetFieldData("OutBlock", "unchgjo");
                int steadyCount;
                if (int.TryParse(temp, out steadyCount) == false)
                    logger.Error($"Index conclusion steady count error, {temp}");
                conclusion.SteadyItemCount = steadyCount;

                temp = indexSubscribingObj.GetFieldData("OutBlock", "lowjo");
                int decreasingCount;
                if (int.TryParse(temp, out decreasingCount) == false)
                    logger.Error($"Index conclusion decreasing count error, {temp}");
                conclusion.DecreasingItemCount = decreasingCount;

                temp = indexSubscribingObj.GetFieldData("OutBlock", "downjo");
                int lowerLimitedCount;
                if (int.TryParse(temp, out lowerLimitedCount) == false)
                    logger.Error($"Index conclusion lower limited count error, {temp}");
                conclusion.LowerLimitedItemCount = lowerLimitedCount;

                if (PrevIndexConclusions.ContainsKey(conclusion.Code) == false)
                    PrevIndexConclusions.TryAdd(conclusion.Code, new IndexConclusion());

                if (PrevIndexConclusions[conclusion.Code].MarketCapitalization == conclusion.MarketCapitalization && 
                    PrevIndexConclusions[conclusion.Code].Amount == conclusion.Amount)
                    return;

                long newReceived;
                newReceived = conclusion.MarketCapitalization;
                conclusion.MarketCapitalization = conclusion.MarketCapitalization - PrevIndexConclusions[conclusion.Code].MarketCapitalization;
                PrevIndexConclusions[conclusion.Code].MarketCapitalization = newReceived;

                newReceived = conclusion.Amount;
                conclusion.Amount = conclusion.Amount - PrevIndexConclusions[conclusion.Code].Amount;
                PrevIndexConclusions[conclusion.Code].Amount = newReceived;

                IndexConclusionQueue.Enqueue(conclusion);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
        
        
        public override bool SubscribeCircuitBreak(string code)
        {
            if (WaitLogin() == false)
            {
                logger.Error("Not loggedin state");
                return false;
            }

            try
            {
                viSubscribingObj.SetFieldData("InBlock", "shcode", code);
                viSubscribingObj.AdviseRealData();
                subscribeCount++;

                dviSubscribingObj.SetFieldData("InBlock", "shcode", code);
                dviSubscribingObj.AdviseRealData();
                subscribeCount++;

                logger.Info($"Subscribe circuit break success, Code: {code}");
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            logger.Error($"Subscribe circuit break fail, Code: {code}");
            return false;
        }

        public override bool UnsubscribeCircuitBreak(string code)
        {

            if (WaitLogin() == false)
            {
                logger.Error("Not loggedin state");
                return false;
            }

            try
            {
                viSubscribingObj.SetFieldData("InBlock", "shcode", code);
                viSubscribingObj.UnadviseRealData();
                subscribeCount++;

                dviSubscribingObj.SetFieldData("InBlock", "shcode", code);
                dviSubscribingObj.UnadviseRealData();
                subscribeCount++;

                logger.Info($"Subscribe circuit break success, Code: {code}");
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            logger.Error($"Subscribe circuit break fail, Code: {code}");
            return false;
        }

        private void VolatilityInterruptionReceived(string szTrCode)
        {
            try
            {
                CircuitBreak circuitBreak = new CircuitBreak();
                circuitBreak.Time = DateTime.Now;
                circuitBreak.Code = viSubscribingObj.GetFieldData("OutBlock", "shcode");
                
                int cbType = Convert.ToInt32(viSubscribingObj.GetFieldData("OutBlock", "vi_gubun"));
                switch (cbType)
                {
                    case 0:
                        circuitBreak.CircuitBreakType = CircuitBreakTypes.Clear;
                        break;
                    case 1:
                        circuitBreak.CircuitBreakType = CircuitBreakTypes.StaticInvoke;
                        break;
                    case 2:
                        circuitBreak.CircuitBreakType = CircuitBreakTypes.DynamicInvoke;
                        break;
                    default:
                        logger.Error($"Invalid circuit break type. cbtype:{cbType}/{viSubscribingObj.GetFieldData("OutBlock", "vi_gubun")}");
                        return;
                }

                if (circuitBreak.CircuitBreakType == CircuitBreakTypes.StaticInvoke)
                    circuitBreak.BasePrice = Convert.ToSingle(viSubscribingObj.GetFieldData("OutBlock", "svi_recprice"));
                else if (circuitBreak.CircuitBreakType == CircuitBreakTypes.DynamicInvoke)
                    circuitBreak.BasePrice = Convert.ToSingle(viSubscribingObj.GetFieldData("OutBlock", "dvi_recprice"));
                else
                    circuitBreak.BasePrice = 0;

                circuitBreak.InvokePrice = Convert.ToSingle(viSubscribingObj.GetFieldData("OutBlock", "vi_trgprice"));

                ServiceClient.PublishCircuitBreak(circuitBreak);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private void AfterVolatilityInterruptionReceived(string szTrCode)
        {
            try
            {
                CircuitBreak circuitBreak = new CircuitBreak();
                circuitBreak.Time = DateTime.Now;
                circuitBreak.Code = dviSubscribingObj.GetFieldData("OutBlock", "shcode");
                
                int cbType = Convert.ToInt32(dviSubscribingObj.GetFieldData("OutBlock", "vi_gubun"));
                switch (cbType)
                {
                    case 0:
                        circuitBreak.CircuitBreakType = CircuitBreakTypes.Clear;
                        break;
                    case 1:
                        circuitBreak.CircuitBreakType = CircuitBreakTypes.StaticInvoke;
                        break;
                    case 2:
                        circuitBreak.CircuitBreakType = CircuitBreakTypes.DynamicInvoke;
                        break;
                    default:
                        logger.Error($"Invalid circuit break type. cbtype:{cbType}/{dviSubscribingObj.GetFieldData("OutBlock", "vi_gubun")}");
                        return;
                }

                if (circuitBreak.CircuitBreakType == CircuitBreakTypes.StaticInvoke)
                    circuitBreak.BasePrice = Convert.ToSingle(dviSubscribingObj.GetFieldData("OutBlock", "svi_recprice"));
                else if (circuitBreak.CircuitBreakType == CircuitBreakTypes.DynamicInvoke)
                    circuitBreak.BasePrice = Convert.ToSingle(dviSubscribingObj.GetFieldData("OutBlock", "dvi_recprice"));
                else
                    circuitBreak.BasePrice = 0;

                circuitBreak.InvokePrice = Convert.ToSingle(dviSubscribingObj.GetFieldData("OutBlock", "vi_trgprice"));
                
                ServiceClient.PublishCircuitBreak(circuitBreak);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public bool GetQuote(string code, ref StockMaster stockMaster)
        {
            if(WaitWarninglistUpdated() == false)
            {
                logger.Error($"Quoting failed, Code: {code}, Warning list update is not done yet.");
                return false;
            }

            if (Monitor.TryEnter(QuoteLock, QuoteLockTimeout) == false)
            {
                logger.Error($"Quoting failed, Code: {code}, Can't obtaion lock object");
                return false;
            }

            QuoteInterval = 1000 / stockQuotingObj.GetTRCountPerSec("t1102");

            int ret = -1;

            try
            {
                if (WaitLogin() == false)
                {
                    logger.Error($"Quoting failed, Code: {code}, Not loggedin state");
                    return false;
                }

                WaitQuoteInterval();
                
                logger.Info($"Start quoting, Code: {code}");
                QuotingStockMaster = stockMaster;
                QuotingStockMaster.Code = code;

                stockQuotingObj.SetFieldData("t1102InBlock", "shcode", 0, code);
                ret = stockQuotingObj.Request(false);

                if (ret > 0)
                {
                    if (WaitQuoting() == true)
                    {
                        if (QuotingStockMaster.Code != string.Empty)
                        {
                            logger.Info($"Quoting done, Code: {code}");
                            return true;
                        }

                        logger.Error($"Quoting fail, Code: {code}");
                    }

                    logger.Error($"Quoting timeout, Code: {code}");
                }
                else
                {
                    logger.Error($"Quoting request fail, Code: {code}, result: {ret}");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                QuotingStockMaster = null;
                Monitor.Exit(QuoteLock);
            }

            return false;
        }

        public bool GetQuote(string code, ref IndexMaster indexMaster)
        {
            if (Monitor.TryEnter(QuoteLock, QuoteLockTimeout) == false)
            {
                logger.Error($"Quoting failed, Code: {code}, Can't obtaion lock object");
                return false;
            }

            QuoteInterval = 1000 / stockQuotingObj.GetTRCountPerSec("t1511");

            int ret = -1;

            try
            {
                if (WaitLogin() == false)
                {
                    logger.Error($"Quoting failed, Code: {code}, Not loggedin state");
                    return false;
                }

                WaitQuoteInterval();

                logger.Info($"Start quoting, Code: {code}");
                QuotingIndexMaster = indexMaster;

                indexQuotingObj.SetFieldData("t1511InBlock", "upcode", 0, code);
                ret = indexQuotingObj.Request(false);

                if (ret > 0)
                {
                    if (WaitQuoting() == true)
                    {
                        if (QuotingIndexMaster.Code != string.Empty)
                        {
                            logger.Info($"Quoting done, Code: {code}");
                            return true;
                        }

                        logger.Error($"Quoting fail, Code: {code}");
                    }

                    logger.Error($"Quoting timeout, Code: {code}");
                }
                else
                {
                    logger.Error($"Quoting request fail, Code: {code}, Quoting result: {ret}");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                QuotingIndexMaster = null;
                Monitor.Exit(QuoteLock);
            }

            return false;
        }

        private void StockMasterReceived()
        {
            try
            {
                if (QuotingStockMaster == null)
                    return;

                string cvStr = stockQuotingObj.GetFieldData("t1102OutBlock", "abscnt", 0);
                long cv = 0;
                if (long.TryParse(cvStr, out cv) == false)
                    logger.Error($"Stock master circulating volume error, {cvStr}");

                QuotingStockMaster.CirculatingVolume = cv * 1000;  //유통주식수

                string listDateStr = stockQuotingObj.GetFieldData("t1102OutBlock", "listdate", 0); // 상장일
                int listDate = 0;
                if (int.TryParse(listDateStr, out listDate) == true)
                {
                    QuotingStockMaster.ListedDate = new DateTime(listDate / 10000, listDate / 100 % 100, listDate % 100);
                }

                string valueAltered = stockQuotingObj.GetFieldData("t1102OutBlock", "info1", 0);
                if (valueAltered == "권배락")         QuotingStockMaster.ValueAlteredType = ValueAlteredTypes.ExRightDividend;
                else if (valueAltered == "권리락")    QuotingStockMaster.ValueAlteredType = ValueAlteredTypes.ExRight;
                else if (valueAltered == "배당락")    QuotingStockMaster.ValueAlteredType = ValueAlteredTypes.ExDividend;
                else if (valueAltered == "액면분할")  QuotingStockMaster.ValueAlteredType = ValueAlteredTypes.SplitFaceValue;
                else if (valueAltered == "액면병합")  QuotingStockMaster.ValueAlteredType = ValueAlteredTypes.MergeFaceValue;
                else if (valueAltered == "주식병합")  QuotingStockMaster.ValueAlteredType = ValueAlteredTypes.Consolidation;
                else if (valueAltered == "기업분할")  QuotingStockMaster.ValueAlteredType = ValueAlteredTypes.Divestiture;
                else if (valueAltered == "감자")      QuotingStockMaster.ValueAlteredType = ValueAlteredTypes.CapitalReduction;
                else                                  QuotingStockMaster.ValueAlteredType = ValueAlteredTypes.None;

                string suspended = stockQuotingObj.GetFieldData("t1102OutBlock", "info3", 0);
                if (suspended == "suspended") QuotingStockMaster.TradingSuspend = true;
                else QuotingStockMaster.TradingSuspend = false;

                // 관리
                if (warningListDic["AdministrativeIssue"].Contains(QuotingStockMaster.Code))
                    QuotingStockMaster.AdministrativeIssue = true;
                else
                    QuotingStockMaster.AdministrativeIssue = false;

                // 불성실공시
                if (warningListDic["UnfairAnnouncement"].Contains(QuotingStockMaster.Code))
                    QuotingStockMaster.UnfairAnnouncement = true;
                else
                    QuotingStockMaster.UnfairAnnouncement = false;

                // 투자유의
                if (warningListDic["InvestAttention"].Contains(QuotingStockMaster.Code))
                    QuotingStockMaster.InvestAttention = true;
                else
                    QuotingStockMaster.InvestAttention = false;

                // 투자환기
                if (warningListDic["CallingAttention"].Contains(QuotingStockMaster.Code))
                    QuotingStockMaster.CallingAttention = true;
                else
                    QuotingStockMaster.CallingAttention = false;

                // 경고
                if (warningListDic["InvestWarning"].Contains(QuotingStockMaster.Code))
                    QuotingStockMaster.InvestWarning = true;
                else
                    QuotingStockMaster.InvestWarning = false;

                // 매매정지
                if (warningListDic["TradingHalt"].Contains(QuotingStockMaster.Code))
                    QuotingStockMaster.TradingHalt = true;
                else
                    QuotingStockMaster.TradingHalt = false;

                // 정리매매
                if (warningListDic["CleaningTrade"].Contains(QuotingStockMaster.Code))
                    QuotingStockMaster.CleaningTrade = true;
                else
                    QuotingStockMaster.CleaningTrade = false;

                // 주의
                if (warningListDic["InvestCaution"].Contains(QuotingStockMaster.Code))
                    QuotingStockMaster.InvestCaution = true;
                else
                    QuotingStockMaster.InvestCaution = false;

                // 위험
                if (warningListDic["InvestmentRisk"].Contains(QuotingStockMaster.Code))
                    QuotingStockMaster.InvestmentRisk = true;
                else
                    QuotingStockMaster.InvestmentRisk = false;

                // 위험예고
                if (warningListDic["InvestmentRiskNoticed"].Contains(QuotingStockMaster.Code))
                    QuotingStockMaster.InvestmentRiskNoticed = true;
                else
                    QuotingStockMaster.InvestmentRiskNoticed = false;

                // 단기과열
                if (warningListDic["Overheated"].Contains(QuotingStockMaster.Code))
                    QuotingStockMaster.Overheated = true;
                else
                    QuotingStockMaster.Overheated = false;

                // 단기과열지정예고
                if (warningListDic["OverheatNoticed"].Contains(QuotingStockMaster.Code))
                    QuotingStockMaster.OverheatNoticed = true;
                else
                    QuotingStockMaster.OverheatNoticed = false;

            }
            catch (Exception ex)
            {
                QuotingStockMaster.Code = string.Empty;
                logger.Error(ex);
            }
            finally
            {
                SetQuoting();
            }
        }

        private void IndexMasterReceived()
        {
            try
            {
                if (QuotingIndexMaster == null)
                    return;

                string temp = indexQuotingObj.GetFieldData("t1511OutBlock", "jniljisu", 0);
                if (temp == "") temp = "0";
                QuotingIndexMaster.PreviousClosedPrice = Convert.ToDouble(temp); // 현재가

                temp = indexQuotingObj.GetFieldData("t1511OutBlock", "jnilvolume", 0);
                if (temp == "") temp = "0";
                QuotingIndexMaster.PreviousVolume = Convert.ToInt64(temp); //전일거래량

                temp = indexQuotingObj.GetFieldData("t1511OutBlock", "jnilvalue", 0);
                if (temp == "") temp = "0";
                QuotingIndexMaster.PreviousTradeCost = Convert.ToInt64(temp);  //전일거래대금
            }
            catch (Exception ex)
            {
                QuotingIndexMaster.Code = string.Empty;
                logger.Error(ex);
            }
            finally
            {
                SetQuoting();
            }
        }

        public override StockMaster GetStockMaster(string code)
        {
            var stockMaster = new StockMaster();
            stockMaster.Code = code;

            if (GetQuote(code, ref stockMaster) == false)
                stockMaster.Code = string.Empty;

            return stockMaster;
        }

        public  Dictionary<string, string> GetIndexCodeList()
        {
            int ret = -1;
            try
            {
                if (WaitLogin() == false)
                {
                    logger.Error($"Get industry list fail. Not loggedin state");
                    return null;
                }

                WaitQuoteInterval();

                indexListObj.SetFieldData("t8424InBlock", "gubun1", 0, "");
                ret = indexListObj.Request(false);

                if (ret > 0)
                {
                    if (WaitQuoting() == true)
                    {
                        logger.Error("Quoting industry list success");
                    }

                    logger.Error("Quoting industry list fail");
                }
                else
                {
                    logger.Error($"Quoting industry list fail. Quoting result: {ret}");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return industryDictionary;
        }

        public Dictionary<string, string> GetStockCodeList()
        {
            int ret = -1;
            try
            {
                if (WaitLogin() == false)
                {
                    logger.Error($"Get stock list fail. Not loggedin state");
                    return null;
                }

                WaitQuoteInterval();

                codeListObj.SetFieldData("t8430InBlock", "gubun", 0, "0");
                ret = codeListObj.Request(false);

                if (ret > 0)
                {
                    if (WaitQuoting() == true)
                    {
                        logger.Error("Quoting stock list success");
                    }

                    logger.Error("Quoting stock list fail");
                }
                else
                {
                    logger.Error($"Quoting stock list fail. Quoting result: {ret}");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return stockDictionary;
        }

        public override Dictionary<string, CodeEntity> GetCodeList()
        {
            var codeList = new Dictionary<string, CodeEntity>();

            try
            {
                foreach (KeyValuePair<string, string> code in GetIndexCodeList())
                {
                    var codeEntity = new CodeEntity();
                    codeEntity.Code = code.Key;
                    codeEntity.Name = code.Value;
                    codeEntity.MarketType = MarketTypes.INDEX;
                    if (codeList.ContainsKey(codeEntity.Code) == false)
                        codeList.Add(codeEntity.Code, codeEntity);
                    else
                        logger.Error($"{codeEntity.Code} code already exists");

                }

                foreach (KeyValuePair<string, string> code in GetStockCodeList())
                {
                    var codeEntity = new CodeEntity();
                    codeEntity.Code = code.Key;
                    codeEntity.Name = code.Value;
                    codeEntity.MarketType = MarketTypes.Unknown;
                    if (codeList.ContainsKey(codeEntity.Code) == false)
                        codeList.Add(codeEntity.Code, codeEntity);
                    else
                        logger.Error($"{codeEntity.Code} code already exists");
                }

                logger.Info($"Code list query done, Count: {codeList.Count}");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return codeList;
        }

        private void IndexListReceived()
        {
            int cnt = indexListObj.GetBlockCount("t8424OutBlock");
            for (int i = 0; i < cnt; i++)
            {
                industryDictionary.Add(indexListObj.GetFieldData("t8424OutBlock", "upcode", i), indexListObj.GetFieldData("t8424OutBlock", "hname", i));
            }
            SetQuoting();
        }

        private void StockListReceived()
        {
            int cnt = codeListObj.GetBlockCount("t8430OutBlock");
           
            for (int i = 0; i < cnt; i++)
            {
                stockDictionary.Add(codeListObj.GetFieldData("t8430OutBlock", "shcode", i), codeListObj.GetFieldData("t8430OutBlock", "hname", i));
            }
            SetQuoting();
        }

        private bool GetWarningList(WarningTypes1 warningType)
        {
            if (Monitor.TryEnter(QuoteLock, QuoteLockTimeout) == false)
            {
                logger.Error($"Updating {warningType.ToString()} list fail. Can't obtaion lock object");
                return false;
            }

            QuoteInterval = 1000 / stockQuotingObj.GetTRCountPerSec("t1404");

            try
            {
                if (WaitLogin() == false)
                {
                    logger.Error($"Updating {warningType.ToString()} list fail, Not loggedin state");
                    return false;
                }

                WaitQuoteInterval();

                if (warningListDic == null)
                {
                    warningListDic = new Dictionary<string, List<string>>();
                }
                if (!warningListDic.ContainsKey(warningType.ToString()))
                {
                    warningListDic.Add(warningType.ToString(), new List<string>());
                }
                currentUpdatingWarningType = warningType.ToString();
                warningObj1.SetFieldData("t1404InBlock", "gubun", 0, "0");
                warningObj1.SetFieldData("t1404InBlock", "jongchk", 0, ((int)warningType).ToString());
                int ret = warningObj1.Request(false);

                if (ret > 0)
                {
                    if (WaitQuoting() == true)
                    {
                        logger.Info($"Updating {warningType.ToString()} list done. count:{warningListDic[warningType.ToString()].Count}");
                    }
                    else
                    {
                        logger.Error($"Updating {warningType.ToString()} list timeout.");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                currentUpdatingWarningType = "";
                Monitor.Exit(QuoteLock);
            }
            return true;
        }

        private bool GetWarningList(WarningTypes2 warningType)
        {
            if (Monitor.TryEnter(QuoteLock, QuoteLockTimeout) == false)
            {
                logger.Error($"Updating {warningType.ToString()} list fail. Can't obtaion lock object");
                return false;
            }

            QuoteInterval = 1000 / stockQuotingObj.GetTRCountPerSec("t1405");

            try
            {
                if (WaitLogin() == false)
                {
                    logger.Error($"Updating {warningType.ToString()} list fail, Not loggedin state");
                    return false;
                }

                WaitQuoteInterval();

                if (warningListDic == null)
                {
                    warningListDic = new Dictionary<string, List<string>>();
                }
                if (!warningListDic.ContainsKey(warningType.ToString()))
                {
                    warningListDic.Add(warningType.ToString(), new List<string>());
                }
                currentUpdatingWarningType = warningType.ToString();
                warningObj2.SetFieldData("t1405InBlock", "gubun", 0, "0");
                warningObj2.SetFieldData("t1405InBlock", "jongchk", 0, ((int)warningType).ToString());
                int ret = warningObj2.Request(false);

                if (ret > 0)
                {
                    if (WaitQuoting() == true)
                    {
                        logger.Info($"Updating {warningType.ToString()} list done. count:{warningListDic[warningType.ToString()].Count}");
                    }
                    else
                    {
                        logger.Error($"Updating {warningType.ToString()} list timeout.");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                currentUpdatingWarningType = "";
                Monitor.Exit(QuoteLock);
            }
            return true;
        }

        private void WarningType1ListReceived()
        {
            int cnt = warningObj1.GetBlockCount("t1404OutBlock1");
            for (int i = 0; i < cnt; i++)
            {
                warningListDic[currentUpdatingWarningType].Add(warningObj1.GetFieldData("t1404OutBlock1", "shcode", i));
            }
            string continueCode = warningObj1.GetFieldData("t1404OutBlock", "cts_shcode", 0);
            if (continueCode != "")
            {
                warningObj1.SetFieldData("t1404InBlock", "cts_shcode", 0, continueCode);
                warningObj1.Request(true);
            }
            else
            {
                SetQuoting();
            }
        }

        private void WarningType2ListReceived()
        {
            int cnt = warningObj2.GetBlockCount("t1405OutBlock1");
            for (int i = 0; i < cnt; i++)
            {
                warningListDic[currentUpdatingWarningType].Add(warningObj2.GetFieldData("t1405OutBlock1", "shcode", i));
            }
            string continueCode = warningObj2.GetFieldData("t1405OutBlock", "cts_shcode", 0);
            if (continueCode != "")
            {
                warningObj2.SetFieldData("t1405InBlock", "cts_shcode", 0, continueCode);
                warningObj2.Request(true);
            }
            else
            {
                SetQuoting();
            }
        }

        protected bool WaitWarninglistUpdated()
        {
            return WarningListUpdatedEvent.WaitOne(WarningListUpdateTimeout);
        }

        protected void SetWarninglistUpdated()
        {
            WarningListUpdatedEvent.Set();
        }
        
        protected override void ServiceClient_Opened(object sender, EventArgs e)
        {
            // Login이 완료된 후에 Publisher contract 등록
            WaitLogin();
            WaitWarninglistUpdated();
            base.ServiceClient_Opened(sender, e);
        }

        public override void NotifyMessage(MessageTypes type, string message)
        {
            if (type == MessageTypes.CloseClient)
                Logout();

            base.NotifyMessage(type, message);
        }

        public override bool IsSubscribable()
        {
            return subscribeCount < maxSubscribeCount;
        }
    }
}
