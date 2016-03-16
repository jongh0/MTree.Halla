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

namespace MTree.EbestPublisher
{
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
        private XAQueryClass indexQuotingObj;
        private XAQueryClass stockQuotingObj;
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

                StartIndexConclusionQueueTask();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
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
        }
        #endregion

        #region XASession
        private void sessionObj_Event_Logout()
        {
            LastFirmCommunicateTick = Environment.TickCount;

            LoginInstance.State = StateType.Logout;
            logger.Info(LoginInstance.ToString());
        }

        private void sessionObj_Event_Login(string szCode, string szMsg)
        {
            LastFirmCommunicateTick = Environment.TickCount;

            QuoteInterval = 1000 / stockQuotingObj.GetTRCountPerSec("t1102");

            LoginInstance.State = StateType.Login;
            SetLogin();

            logger.Info($"{LoginInstance.ToString()}nszCode: {szCode}, szMsg: {szMsg}");
        }

        private void sessionObj_Disconnect()
        {
            LoginInstance.State = StateType.Disconnect;
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
            base.SubscribeIndex(code);

            if (WaitLogin() == false)
            {
                logger.Error("Not loggedin state");
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
            base.UnsubscribeIndex(code);

            if (WaitLogin() == false)
            {
                logger.Error("Not loggedin state");
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
                conclusion.Index = index;

                temp = indexSubscribingObj.GetFieldData("OutBlock", "volume");
                double volumn = 0;
                if (double.TryParse(temp, out volumn) == false)
                    logger.Error($"Index conclusion index error, {temp}");
                conclusion.Volume = volumn * 1000;

                temp = indexSubscribingObj.GetFieldData("OutBlock", "value");
                double value;
                if (double.TryParse(temp, out value) == false)
                    logger.Error($"Index conclusion value error, {temp}");
                conclusion.Value = value;

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

        public bool SubscribeCircuitBreak(string code)
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
                circuitBreak.Code = viSubscribingObj.GetFieldData("OutBlock", "shcode");
                circuitBreak.CircuitBreakState = (CircuitBreakType)Convert.ToInt32(viSubscribingObj.GetFieldData("OutBlock", "vi_gubun"));

                if (circuitBreak.CircuitBreakState == CircuitBreakType.StaticInvoke)
                    circuitBreak.BasePrice = Convert.ToSingle(viSubscribingObj.GetFieldData("OutBlock", "svi_recprice"));
                else if (circuitBreak.CircuitBreakState == CircuitBreakType.DynamicInvoke)
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

        public bool GetQuote(string code, ref StockMaster stockMaster)
        {
            if (Monitor.TryEnter(QuoteLock, QuoteLockTimeout) == false)
            {
                logger.Error($"Quoting failed, Code: {code}, Can't obtaion lock object");
                return false;
            }

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

                stockQuotingObj.SetFieldData("t1102InBlock", "shcode", 0, code);
                ret = stockQuotingObj.Request(false);

                if (ret > 0)
                {
                    if (WaitQuoting() == true)
                    {
                        if (QuotingStockMaster.Code != string.Empty)
                        {
                            logger.Info($"Quoting done. Code: {code}");
                            return true;
                        }

                        logger.Error($"Quoting fail. Code: {code}");
                    }

                    logger.Error($"Quoting timeout. Code: {code}");
                }
                else
                {
                    logger.Error($"Quoting request fail. Code: {code}, result: {ret}");
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
                            logger.Info($"Quoting done. Code: {code}");
                            return true;
                        }

                        logger.Error($"Quoting fail. Code: {code}");
                    }

                    logger.Error($"Quoting timeout. Code: {code}");
                }
                else
                {
                    logger.Error($"Quoting request fail. Code: {code}, Quoting result: {ret}");
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

                string valueAltered = stockQuotingObj.GetFieldData("t1102OutBlock", "info1", 0);
                if (valueAltered == "권배락")         QuotingStockMaster.ValueAltered = ValueAlteredType.ExRightDividend;
                else if (valueAltered == "권리락")    QuotingStockMaster.ValueAltered = ValueAlteredType.ExRight;
                else if (valueAltered == "배당락")    QuotingStockMaster.ValueAltered = ValueAlteredType.ExDividend;
                else if (valueAltered == "액면분할")  QuotingStockMaster.ValueAltered = ValueAlteredType.SplitFaceValue;
                else if (valueAltered == "액면병합")  QuotingStockMaster.ValueAltered = ValueAlteredType.MergeFaceValue;
                else if (valueAltered == "주식병합")  QuotingStockMaster.ValueAltered = ValueAlteredType.Consolidation;
                else if (valueAltered == "기업분할")  QuotingStockMaster.ValueAltered = ValueAlteredType.Divestiture;
                else if (valueAltered == "감자")      QuotingStockMaster.ValueAltered = ValueAlteredType.CapitalReduction;
                else                                  QuotingStockMaster.ValueAltered = ValueAlteredType.None;
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
            base.GetStockMaster(code);

            var stockMaster = new StockMaster();
            stockMaster.Code = code;

            if (GetQuote(code, ref stockMaster) == false)
                stockMaster.Code = string.Empty;

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
            base.IsSubscribable();

            return subscribeCount < maxSubscribeCount;
        }

        protected override void OnCommunicateTimer(object sender, ElapsedEventArgs e)
        {
            // TODO : Keep firm communication code
            if ((Environment.TickCount - LastFirmCommunicateTick) > MaxCommunicateInterval)
            {
                logger.Info($"[{GetType().Name}] Keep firm connection");
            }

            base.OnCommunicateTimer(sender, e);
        }
    }
}
