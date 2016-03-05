using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Xml.Serialization;
using System.Threading.Tasks;
using MTree.DataStructure;
using MTree.Configuration;
using MTree.Provider;
using XA_SESSIONLib;
using XA_DATASETLib;

namespace MTree.EbestProvider
{
    public class EbestProvider : BrokerageFirmProvider
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private const int maxQueryableCount = 200;
        private int queryableCount = 0;

        public bool IsQueryable
        {
            get { return (queryableCount < maxQueryableCount); }
        }

        static private object lockObject = new object();

        static private volatile EbestProvider _instance;
        static public EbestProvider Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (lockObject)
                    {
                        if (_instance == null)
                            _instance = new EbestProvider();
                    }
                }

                return _instance;
            }
        }

        private readonly string resFilePath = "\\Res";

        public string Server { get; set; }

        public int Port { get; set; }

        private LoginInfo loginInfo;

        private CancellationTokenSource loginCheckerCancelSource = new CancellationTokenSource();
        private CancellationToken loginCheckerCancelToken;

        private bool isAnyDataReceived;

        #region Ebest Specific
        private XASessionClass sessionObj;
        private XARealClass realObj;
        private XAQueryClass queryObj; 
        #endregion

        private EbestProvider() : base()
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
                queryObj = new XAQueryClass();
                queryObj.ReceiveChartRealData += queryObj_ReceiveChartRealData;
                queryObj.ReceiveData += queryObj_ReceiveData;
                queryObj.ReceiveMessage += queryObj_ReceiveMessage;
                #endregion

                #region Login
                loginInfo = new LoginInfo();
                loginInfo.LoginState = LoginStateType.Disconnected;
                loginInfo.UserId = Config.Ebest.UserId;
                loginInfo.UserPw = Config.Ebest.UserPw;
                loginInfo.CertPw = Config.Ebest.CertPw;
                loginInfo.AccountPw = Config.Ebest.AccountPw;
                loginInfo.ProviderType = BrokerageServerType.Real;

                if (loginInfo.ProviderType == BrokerageServerType.Real)
                    Server = Config.Ebest.ServerAddress;
                else
                    Server = Config.Ebest.DemoServerAddress;

                Port = Config.Ebest.ServerPort;

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
            logger.Info($"bIsSystemError: {bIsSystemError}, nMessageCode: {nMessageCode}, szMessage: {szMessage}");
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
            loginInfo.LoginState = LoginStateType.LoggedOut;
            loginCheckerCancelSource.Cancel();
        }

        private void sessionObj_Event_Login(string szCode, string szMsg)
        {
            logger.Info($"szCode: {szCode}, szMsg: {szMsg}");
            loginInfo.LoginState = LoginStateType.LoggedIn;

            Task.Run(() => { LoginStateChecker(); }, loginCheckerCancelToken);
        }

        private void sessionObj_Disconnect()
        {
            loginInfo.LoginState = LoginStateType.Disconnected;
            loginCheckerCancelSource.Cancel();
        } 
        #endregion

        private void LoginStateChecker()
        {
            logger.Info("LoginStateChecker started");

            while (loginInfo.LoginState == LoginStateType.LoggedIn)
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
                    Console.WriteLine("Operation canceled");
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
                if (sessionObj.ConnectServer(Server, Port) == true)
                {
                    logger.Info("Server connected");

                    if (loginInfo.ProviderType == BrokerageServerType.Real)
                        ret = sessionObj.Login(loginInfo.UserId, loginInfo.UserPw, loginInfo.CertPw, (int)XA_SERVER_TYPE.XA_REAL_SERVER, true);
                    else
                        ret = sessionObj.Login(loginInfo.UserId, loginInfo.UserPw, loginInfo.CertPw, (int)XA_SERVER_TYPE.XA_SIMUL_SERVER, true);

                    if (ret == true)
                    {
                        logger.Info("Login success");
                    }
                    else
                    {
                        logger.Error("Login fail");
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

            return false;
        }

        public bool Logout()
        {
            try
            {
                if (sessionObj.IsConnected() == false)
                    return false;

                if (loginInfo.LoginState != LoginStateType.LoggedIn)
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

        public bool SubscribeIndex(string code)
        {
            if (IsQueryable == false)
            {
                logger.Error($"Not subscribable, Code: {code}");
                return false;
            }

            try
            {
                realObj.SetFieldData("InBlock", "upcode", code);
                realObj.AdviseRealData();

                queryableCount++;
                logger.Info($"Subscribe success, Code: {code}");
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return false;
        }

        public bool UnsubscribeIndex(string code)
        {
            try
            {
                realObj.SetFieldData("InBlock", "upcode", code);
                realObj.UnadviseRealData();

                queryableCount--;
                logger.Info($"Unsubscribe success, Code: {code}");
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

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

                if (prevIndexConclusions.ContainsKey(conclusion.Code) == false)
                    prevIndexConclusions.TryAdd(conclusion.Code, new IndexConclusion());

                if (prevIndexConclusions[conclusion.Code].Value == conclusion.Value && 
                    prevIndexConclusions[conclusion.Code].Volume == conclusion.Volume)
                    return;

                double newReceived;
                newReceived = conclusion.Value;
                conclusion.Value = conclusion.Value - prevIndexConclusions[conclusion.Code].Value;
                prevIndexConclusions[conclusion.Code].Value = newReceived;

                newReceived = conclusion.Volume;
                conclusion.Volume = conclusion.Volume - prevIndexConclusions[conclusion.Code].Volume;
                prevIndexConclusions[conclusion.Code].Volume = newReceived;

                indexConclusionQueue.Enqueue(conclusion);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public bool GetQuote(string code, ref StockMaster stockMaster)
        {
            logger.Info($"Start quoting, Code: {code}");

            if (Monitor.TryEnter(lockObject, 1000 * 10) == false)
            {
                logger.Error($"Quoting failed, Code: {code}");
                return false;
            }

            int ret = -1;

            try
            {
                quotingStockMaster = stockMaster;

                queryObj.SetFieldData("t1102InBlock", "shcode", 0, code);
                ret = queryObj.Request(false);

                if (ret > 0)
                {
                    if (waitQuoting.WaitOne(1000 * 10) == false)
                        ret = -1;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                Monitor.Exit(lockObject);
            }

            return (ret == 0);
        }

        public bool GetQuote(string code, ref IndexMaster indexMaster)
        {
            logger.Info($"Start quoting, Code: {code}");

            if (Monitor.TryEnter(lockObject, 1000 * 10) == false)
            {
                logger.Error($"Quoting failed, Code: {code}");
                return false;
            }

            int ret = -1;

            try
            {
                quotingIndexMaster = indexMaster;

                queryObj.SetFieldData("t1511InBlock", "upcode", 0, code);
                ret = queryObj.Request(false);

                if (ret > 0)
                {
                    if (waitQuoting.WaitOne(1000 * 10) == false)
                        ret = -1;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                Monitor.Exit(lockObject);
            }

            return (ret == 0);
        }

        private void StockMasterReceived()
        {
            try
            {
                if (quotingStockMaster == null)
                    return;

                string temp = queryObj.GetFieldData("t1102OutBlock", "price", 0);
                if (temp == "") temp = "0";
                //quotingStockMaster.LastSale = int.Parse(temp); // 현재가 // TODO : 필요한건가? Daishin에도 주석처리되어 있음

                temp = queryObj.GetFieldData("t1102OutBlock", "jnilvolume", 0);
                if (temp == "") temp = "0";
                quotingStockMaster.PreviousVolume = int.Parse(temp); //전일거래량

                temp = queryObj.GetFieldData("t1102OutBlock", "abscnt", 0);
                if (temp == "") temp = "0";
                quotingStockMaster.CirculatingVolume = int.Parse(temp);  //유통주식수

                string valueAltered = queryObj.GetFieldData("t1102OutBlock", "info1", 0);

                if (valueAltered == "권배락")
                    quotingStockMaster.ValueAltered = ValueAlteredType.ExRightDividend;
                else if (valueAltered == "권리락")
                    quotingStockMaster.ValueAltered = ValueAlteredType.ExRight;
                else if (valueAltered == "배당락")
                    quotingStockMaster.ValueAltered = ValueAlteredType.ExDividend;
                else if (valueAltered == "액면분할")
                    quotingStockMaster.ValueAltered = ValueAlteredType.SplitFaceValue;
                else if (valueAltered == "액면병합")
                    quotingStockMaster.ValueAltered = ValueAlteredType.MergeFaceValue;
                else if (valueAltered == "주식병합")
                    quotingStockMaster.ValueAltered = ValueAlteredType.Consolidation;
                else if (valueAltered == "기업분할")
                    quotingStockMaster.ValueAltered = ValueAlteredType.Divestiture;
                else if (valueAltered == "감자")
                    quotingStockMaster.ValueAltered = ValueAlteredType.CapitalReduction;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                waitQuoting.Set();
            }
        }

        private void IndexMasterReceived()
        {
            try
            {
                if (quotingIndexMaster == null)
                    return;

                string temp = queryObj.GetFieldData("t1511OutBlock", "jniljisu", 0);
                if (temp == "") temp = "0";
                quotingIndexMaster.PreviousClosedPrice = Convert.ToDouble(temp); // 현재가

                temp = queryObj.GetFieldData("t1511OutBlock", "jnilvolume", 0);
                if (temp == "") temp = "0";
                quotingIndexMaster.PreviousVolume = Convert.ToInt64(temp); //전일거래량

                temp = queryObj.GetFieldData("t1511OutBlock", "jnilvalue", 0);
                if (temp == "") temp = "0";
                quotingIndexMaster.PreviousTradeCost = Convert.ToInt64(temp);  //전일거래대금
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                waitQuoting.Set();
            }
        }
    }
}
