using System;
using System.Threading;
using MTree.DataStructure;
using MTree.Configuration;
using MTree.Publisher;
using XA_SESSIONLib;
using XA_DATASETLib;
using System.ServiceModel;
using MTree.RealTimeProvider;

namespace MTree.EbestPublisher
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class EbestPublisher : BrokerageFirmBase
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        protected object lockObject = new object();
        private bool requestResult = false;

        private const int maxSubscribeCount = 200;
        private int subscribeCount = 0;

        private readonly string resFilePath = "\\Res";

        private CancellationTokenSource loginCheckerCancelSource = new CancellationTokenSource();
        private CancellationToken loginCheckerCancelToken;

        private ManualResetEvent waitLoginEvent { get; } = new ManualResetEvent(false);

        private int StockQuoteInterval { get; set; } = 0;

        private bool isAnyDataReceived;

        #region Ebest Specific
        private XASessionClass sessionObj;
        private XARealClass realObj;
        private XAQueryClass indexQuotingObj;
        private XAQueryClass stockQuotingObj;
        #endregion

        public EbestPublisher() : base()
        {
            try
            {
                loginCheckerCancelToken = loginCheckerCancelSource.Token;

                #region XASession
                sessionObj = new XASessionClass();
                sessionObj.Disconnect += sessionObj_Disconnect;
                sessionObj._IXASessionEvents_Event_Login += sessionObj_Event_Login;
                sessionObj._IXASessionEvents_Event_Logout += sessionObj_Event_Logout;
                #endregion

                #region XAReal
                realObj = new XARealClass();
                realObj.ReceiveRealData += realObj_ReceiveRealData;
                realObj.RecieveLinkData += realObj_RecieveLinkData;
                realObj.ResFileName = resFilePath + "\\IJ_.res";
                #endregion

                #region XAQuery
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
                #endregion

                #region Login
                LoginInstance.UserId = Config.Ebest.UserId;
                LoginInstance.UserPw = Config.Ebest.UserPw;
                LoginInstance.CertPw = Config.Ebest.CertPw;
                LoginInstance.AccountPw = Config.Ebest.AccountPw;
                LoginInstance.Server = Config.Ebest.Server;
                LoginInstance.ServerAddress = Config.Ebest.ServerAddress;
                LoginInstance.ServerPort = Config.Ebest.ServerPort;

                Login(); 
                #endregion
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        #region XAQuery
        private void queryObj_ReceiveMessage(bool bIsSystemError, string nMessageCode, string szMessage)
        {
            if (bIsSystemError == true)
                logger.Error($"bIsSystemError: {bIsSystemError}, nMessageCode: {nMessageCode}, szMessage: {szMessage}");

            requestResult = false;
        }

        private void queryObj_ReceiveData(string szTrCode)
        {
            logger.Info($"szTrCode: {szTrCode}");

            if (szTrCode == "t1102")
                StockMasterReceived();
            else if (szTrCode == "t1511")
                IndexMasterReceived();

            isAnyDataReceived = true;
        }

        private void queryObj_ReceiveChartRealData(string szTrCode)
        {
            logger.Info($"szTrCode: {szTrCode}");
        }
        #endregion

        #region XAReal
        private void realObj_RecieveLinkData(string szLinkName, string szData, string szFiller)
        {
        }

        private void realObj_ReceiveRealData(string szTrCode)
        {
            if (szTrCode == "IJ_")
                IndexConclusionReceived(szTrCode);

            isAnyDataReceived = true;
        }
        #endregion

        #region XASession
        private void sessionObj_Event_Logout()
        {
            LoginInstance.State = StateType.Logout;
            loginCheckerCancelSource.Cancel();

            logger.Info(LoginInstance.ToString());
        }

        private void sessionObj_Event_Login(string szCode, string szMsg)
        {
            StockQuoteInterval = 1000 / stockQuotingObj.GetTRCountPerSec("t1102");

            LoginInstance.State = StateType.Login;
            waitLoginEvent.Set();

            logger.Info($"{LoginInstance.ToString()}nszCode: {szCode}, szMsg: {szMsg}");

            //Task.Run(() => { LoginStateChecker(); }, loginCheckerCancelToken);
        }

        private void sessionObj_Disconnect()
        {
            LoginInstance.State = StateType.Disconnect;
            loginCheckerCancelSource.Cancel();

            logger.Info(LoginInstance.ToString());
        }
        #endregion

        #region Login / Logout
        private void LoginStateChecker()
        {
            logger.Info("LoginStateChecker started");

            while (LoginInstance.State == StateType.Login)
            {
                try
                {
                    loginCheckerCancelToken.ThrowIfCancellationRequested();

                    if (isAnyDataReceived == false)
                    {
                        IndexMaster indexMaster = new IndexMaster();
                        GetQuote("001", ref indexMaster);
                    }

                    isAnyDataReceived = false;

                    // 10분 sleep
                    int sleepCount = 60 * 10;
                    while (sleepCount-- > 0)
                    {
                        loginCheckerCancelToken.ThrowIfCancellationRequested();
                        Thread.Sleep(1000);
                    }
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("LoginStateChecker canceled");
                    break;
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }
            }

            logger.Info("LoginStateChecker stopped");
        }

        public bool Login()
        {
            bool ret = false;

            try
            {
                if (sessionObj.ConnectServer(LoginInstance.ServerAddress, LoginInstance.ServerPort) == true)
                {
                    logger.Info("Server connected");

                    if (LoginInstance.Server == ServerType.Real)
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

        public bool WaitLogin()
        {
            return waitLoginEvent.WaitOne(5000);
        }

        public bool Logout()
        {
            try
            {
                if (sessionObj.IsConnected() == false)
                    return false;

                if (LoginInstance.State != StateType.Login)
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
            if (waitLoginEvent.WaitOne(10000) == false)
            {
                logger.Error("Not loggedin state");
                return false;
            }

            try
            {
                realObj.SetFieldData("InBlock", "upcode", code);
                realObj.AdviseRealData();

                subscribeCount++;
                logger.Info($"Subscribe index success, Code: {code}");
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
            if (waitLoginEvent.WaitOne(10000) == false)
            {
                logger.Error("Not loggedin state");
                return false;
            }

            try
            {
                realObj.SetFieldData("InBlock", "upcode", code);
                realObj.UnadviseRealData();

                subscribeCount--;
                logger.Info($"Unsubscribe index success, Code: {code}");
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
                IndexConclusion conclusion = new IndexConclusion();

                string temp = realObj.GetFieldData("OutBlock", "upcode");
                conclusion.Code = temp;

                temp = realObj.GetFieldData("OutBlock", "time");
                var now = DateTime.Now;
                uint time;

                if (uint.TryParse(temp, out time) == true)
                    conclusion.Time = new DateTime(now.Year, now.Month, now.Day, (int)(time / 10000), (int)((time / 100) % 100), (int)time % 100, now.Millisecond);
                else
                    conclusion.Time = now;

                temp = realObj.GetFieldData("OutBlock", "jisu");
                double index;
                if (double.TryParse(temp, out index) == true)
                    conclusion.Index = index;

                temp = realObj.GetFieldData("OutBlock", "volume");
                double volumn;
                if (double.TryParse(temp, out volumn) == true)
                    conclusion.Volume = volumn * 1000;

                temp = realObj.GetFieldData("OutBlock", "value");
                double value;
                if (double.TryParse(temp, out value) == true)
                    conclusion.Value = value;

                temp = realObj.GetFieldData("OutBlock", "upjo");
                int upperLimitCount;
                if (int.TryParse(temp, out upperLimitCount) == true)
                    conclusion.UpperLimitedItemCount = upperLimitCount;

                temp = realObj.GetFieldData("OutBlock", "highjo");
                int increasingCount;
                if (int.TryParse(temp, out increasingCount) == true)
                    conclusion.IncreasingItemCount = increasingCount;

                temp = realObj.GetFieldData("OutBlock", "unchgjo");
                int steadyCount;
                if (int.TryParse(temp, out steadyCount) == true)
                    conclusion.SteadyItemCount = steadyCount;

                temp = realObj.GetFieldData("OutBlock", "lowjo");
                int decreasingCount;
                if (int.TryParse(temp, out decreasingCount) == true)
                    conclusion.DecreasingItemCount = decreasingCount;

                temp = realObj.GetFieldData("OutBlock", "downjo");
                int lowerLimitedCount;
                if (int.TryParse(temp, out lowerLimitedCount) == true)
                    conclusion.LowerLimitedItemCount = lowerLimitedCount;

                if (PrevIndexConclusions.ContainsKey(conclusion.Code) == false)
                    PrevIndexConclusions.TryAdd(conclusion.Code, new IndexConclusion());

                if (PrevIndexConclusions[conclusion.Code].Value == conclusion.Value && 
                    PrevIndexConclusions[conclusion.Code].Volume == conclusion.Volume)
                    return;

                double newReceived;
                newReceived = conclusion.Value;
                conclusion.Value = conclusion.Value - PrevIndexConclusions[conclusion.Code].Value;
                PrevIndexConclusions[conclusion.Code].Value = newReceived;

                newReceived = conclusion.Volume;
                conclusion.Volume = conclusion.Volume - PrevIndexConclusions[conclusion.Code].Volume;
                PrevIndexConclusions[conclusion.Code].Volume = newReceived;

                IndexConclusionQueue.Enqueue(conclusion);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public bool GetQuote(string code, ref StockMaster stockMaster)
        {
            if (Monitor.TryEnter(lockObject, 1000 * 10) == false)
            {
                logger.Error($"Quoting failed, Code: {code}, Can't obtaion lock object");
                return false;
            }

            if (waitLoginEvent.WaitOne(10000) == false)
            {
                logger.Error($"Quoting failed, Code: {code}, Not loggedin state");
                return false;
            }

            logger.Info($"Start quoting, Code: {code}");
            requestResult = false;
            int ret = -1;

            try
            {
                QuotingStockMaster = stockMaster;

                WaitQuotingLimit();

                stockQuotingObj.SetFieldData("t1102InBlock", "shcode", 0, code);
                ret = stockQuotingObj.Request(false);

                if (ret > 0)
                {
                    if (WaitQuotingEvent.WaitOne(1000 * 10) == true)
                    {
                        logger.Info($"Quoting done. Code: {code}");
                    }
                    else
                    {
                        logger.Error($"Quoting timeout. Code: {code}");
                        ret = -1;
                    }
                }
                else
                {
                    logger.Error($"Quoting request failed. Code: {code}, Quoting result: {ret}");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                QuotingStockMaster = null;
                Monitor.Exit(lockObject);
            }

            return (ret > 0 && requestResult == true);
        }

        public bool GetQuote(string code, ref IndexMaster indexMaster)
        {
            if (Monitor.TryEnter(lockObject, 1000 * 10) == false)
            {
                logger.Error($"Quoting failed, Code: {code}, Can't obtaion lock object");
                return false;
            }

            if (waitLoginEvent.WaitOne(10000) == false)
            {
                logger.Error($"Quoting failed, Code: {code}, Not loggedin state");
                return false;
            }

            logger.Info($"Start quoting, Code: {code}");

            requestResult = false;
            int ret = -1;

            try
            {
                QuotingIndexMaster = indexMaster;

                indexQuotingObj.SetFieldData("t1511InBlock", "upcode", 0, code);
                ret = indexQuotingObj.Request(false);

                if (ret > 0)
                {
                    if (WaitQuotingEvent.WaitOne(1000 * 10) == true)
                    {
                        logger.Info($"Quoting done. Code: {code}");
                    }
                    else
                    {
                        logger.Error($"Quoting timeout. Code: {code}");
                        ret = -1;
                    }
                }
                else
                {
                    logger.Error($"Quoting request failed. Code: {code}, Quoting result: {ret}");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                QuotingIndexMaster = null;
                Monitor.Exit(lockObject);
            }

            return (ret > 0 && requestResult == true);
        }

        private void StockMasterReceived()
        {
            try
            {
                if (QuotingStockMaster == null)
                    return;

                string temp = stockQuotingObj.GetFieldData("t1102OutBlock", "price", 0);
                if (temp == "")
                {
                    logger.Error("price is null");
                    temp = "0";
                }

                temp = stockQuotingObj.GetFieldData("t1102OutBlock", "jnilvolume", 0);
                if (temp == "")
                {
                    logger.Error("previous volume is null");
                    temp = "0";
                }
                //QuotingStockMaster.PreviousVolume = int.Parse(temp); //전일거래량 => Daishin에서 조회

                temp = stockQuotingObj.GetFieldData("t1102OutBlock", "abscnt", 0);
                if (temp == "")
                {
                    logger.Error("Circulating volume is null");
                    temp = "0";
                }
                QuotingStockMaster.CirculatingVolume = int.Parse(temp) * 1000;  //유통주식수

                string valueAltered = stockQuotingObj.GetFieldData("t1102OutBlock", "info1", 0);

                if (valueAltered == "권배락")
                    QuotingStockMaster.ValueAltered = ValueAlteredType.ExRightDividend;
                else if (valueAltered == "권리락")
                    QuotingStockMaster.ValueAltered = ValueAlteredType.ExRight;
                else if (valueAltered == "배당락")
                    QuotingStockMaster.ValueAltered = ValueAlteredType.ExDividend;
                else if (valueAltered == "액면분할")
                    QuotingStockMaster.ValueAltered = ValueAlteredType.SplitFaceValue;
                else if (valueAltered == "액면병합")
                    QuotingStockMaster.ValueAltered = ValueAlteredType.MergeFaceValue;
                else if (valueAltered == "주식병합")
                    QuotingStockMaster.ValueAltered = ValueAlteredType.Consolidation;
                else if (valueAltered == "기업분할")
                    QuotingStockMaster.ValueAltered = ValueAlteredType.Divestiture;
                else if (valueAltered == "감자")
                    QuotingStockMaster.ValueAltered = ValueAlteredType.CapitalReduction;
                else
                    QuotingStockMaster.ValueAltered = ValueAlteredType.None;

                requestResult = true;
            }
            catch (Exception ex)
            {
                requestResult = false;
                QuotingStockMaster.Code = string.Empty;
                logger.Error(ex);
            }
            finally
            {
                WaitQuotingEvent.Set();
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

                requestResult = true;
            }
            catch (Exception ex)
            {
                requestResult = false;
                QuotingIndexMaster.Code = string.Empty;
                logger.Error(ex);
            }
            finally
            {
                WaitQuotingEvent.Set();
            }
        }

        public override StockMaster GetStockMaster(string code)
        {
            var stockMaster = new StockMaster();

            try
            {
                if (GetQuote(code, ref stockMaster))
                {
                    stockMaster.Code = code;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return stockMaster;
        }

        protected override void ServiceClient_Opened(object sender, EventArgs e)
        {
            // Login이 완료된 후에 Publisher contract 등록
            WaitLogin(); 
            base.ServiceClient_Opened(sender, e);
        }

        public override void CloseClient()
        {
            // Logout한 이후 Process 종료시킨다
            Logout();
            base.CloseClient();
        }

        public override bool IsSubscribable()
        {
            return subscribeCount < maxSubscribeCount;
        }

        private void WaitQuotingLimit()
        { 
            if (StockQuoteInterval > 0)
                Thread.Sleep(StockQuoteInterval);
        }
    }
}
