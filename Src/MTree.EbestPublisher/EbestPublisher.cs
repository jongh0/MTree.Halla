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
    public partial class EbestPublisher : BrokerageFirmBase
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private const int maxSubscribeCount = 200;
        private int subscribeCount = 0;

        private readonly string resFilePath = "\\Res";

        #region Keep session
        private int MaxCommunInterval { get; } = 1000 * 60 * 30;
        private int LastCommunTick { get; set; } = Environment.TickCount;
        private System.Timers.Timer CommunTimer { get; set; } 
        #endregion

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

        #region Code list
        private Dictionary<string, string> IndexCodeList { get; set; } = new Dictionary<string, string>();
        private Dictionary<string, string> StockCodeList { get; set; } = new Dictionary<string, string>(); 
        #endregion

        #region Warning
        private Dictionary<string, List<string>> WarningList { get; set; } = new Dictionary<string, List<string>>();
        private string CurrUpdatingWarningType { get; set; }

        private ManualResetEvent WarningListUpdatedEvent { get; } = new ManualResetEvent(false);
        private int WarningListUpdateTimeout { get; } = 1000 * 30; 
        #endregion

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

                StartIndexConclusionQueueTask();
                StartCircuitBreakQueueTask();

                CommunTimer = new System.Timers.Timer(MaxCommunInterval);
                CommunTimer.Elapsed += OnCommunTimer;
                CommunTimer.AutoReset = true;
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
            LastCommunTick = Environment.TickCount;

            if (bIsSystemError == true)
                logger.Error($"bIsSystemError: {bIsSystemError}, nMessageCode: {nMessageCode}, szMessage: {szMessage}");
        }

        private void queryObj_ReceiveData(string szTrCode)
        {
            LastCommunTick = Environment.TickCount;
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
            LastCommunTick = Environment.TickCount;
            logger.Info($"szTrCode: {szTrCode}");
        }
        #endregion

        #region XAReal
        private void realObj_ReceiveRealData(string szTrCode)
        {
            LastCommunTick = Environment.TickCount;

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
            LastCommunTick = Environment.TickCount;

            LoginInstance.State = LoginStates.Logout;
            logger.Info(LoginInstance.ToString());
        }

        private void sessionObj_Event_Login(string szCode, string szMsg)
        {
            LastCommunTick = Environment.TickCount;

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
                    {
                        logger.Info($"Try login with id:{LoginInstance.UserId}");
                        CommunTimer.Start();
                    }
                    else
                    {
                        logger.Error("Login error");
                    }
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
            finally
            {
                CommunTimer.Stop();
            }

            return true;
        }
        #endregion

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
                        logger.Error("Quoting industry list success");

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

            return IndexCodeList;
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
                        logger.Error("Quoting stock list success");

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

            return StockCodeList;
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
            try
            {
                int cnt = indexListObj.GetBlockCount("t8424OutBlock");
                for (int i = 0; i < cnt; i++)
                {
                    IndexCodeList.Add(indexListObj.GetFieldData("t8424OutBlock", "upcode", i), indexListObj.GetFieldData("t8424OutBlock", "hname", i));
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                SetQuoting();
            }
        }

        private void StockListReceived()
        {
            try
            {
                int cnt = codeListObj.GetBlockCount("t8430OutBlock");

                for (int i = 0; i < cnt; i++)
                {
                    StockCodeList.Add(codeListObj.GetFieldData("t8430OutBlock", "shcode", i), codeListObj.GetFieldData("t8430OutBlock", "hname", i));
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                SetQuoting();
            }
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

                if (WarningList.ContainsKey(warningType.ToString()) == false)
                    WarningList.Add(warningType.ToString(), new List<string>());

                CurrUpdatingWarningType = warningType.ToString();
                warningObj1.SetFieldData("t1404InBlock", "gubun", 0, "0");
                warningObj1.SetFieldData("t1404InBlock", "jongchk", 0, ((int)warningType).ToString());

                int ret = warningObj1.Request(false);

                if (ret > 0)
                {
                    if (WaitQuoting() == true)
                        logger.Info($"Updating {warningType.ToString()} list done. count:{WarningList[warningType.ToString()].Count}");
                    else
                        logger.Error($"Updating {warningType.ToString()} list timeout.");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                CurrUpdatingWarningType = "";
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

                if (WarningList.ContainsKey(warningType.ToString()) == false)
                    WarningList.Add(warningType.ToString(), new List<string>());

                CurrUpdatingWarningType = warningType.ToString();
                warningObj2.SetFieldData("t1405InBlock", "gubun", 0, "0");
                warningObj2.SetFieldData("t1405InBlock", "jongchk", 0, ((int)warningType).ToString());

                int ret = warningObj2.Request(false);

                if (ret > 0)
                {
                    if (WaitQuoting() == true)
                        logger.Info($"Updating {warningType.ToString()} list done. count:{WarningList[warningType.ToString()].Count}");
                    else
                        logger.Error($"Updating {warningType.ToString()} list timeout.");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                CurrUpdatingWarningType = "";
                Monitor.Exit(QuoteLock);
            }

            return true;
        }

        private void WarningType1ListReceived()
        {
            try
            {
                int cnt = warningObj1.GetBlockCount("t1404OutBlock1");
                for (int i = 0; i < cnt; i++)
                {
                    WarningList[CurrUpdatingWarningType].Add(warningObj1.GetFieldData("t1404OutBlock1", "shcode", i));
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
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private void WarningType2ListReceived()
        {
            try
            {
                int cnt = warningObj2.GetBlockCount("t1405OutBlock1");
                for (int i = 0; i < cnt; i++)
                {
                    WarningList[CurrUpdatingWarningType].Add(warningObj2.GetFieldData("t1405OutBlock1", "shcode", i));
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
            catch (Exception ex)
            {
                logger.Error(ex);
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

        private void OnCommunTimer(object sender, ElapsedEventArgs e)
        {
            if ((Environment.TickCount - LastCommunTick) > MaxCommunInterval)
            {
                LastCommunTick = Environment.TickCount;
                logger.Info($"[{GetType().Name}] Keep firm connection");

                GetStockMaster("000020");
            }
        }
    }
}
