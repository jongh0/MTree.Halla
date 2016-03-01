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
using XA_DATASETLib;
using XA_SESSIONLib;
using System.Threading.Tasks;
using MTree.DataStructure;
using MTree.Configuration;
using MTree.Provider;

namespace MTree.EbestProvider
{
    public class EbestProvider : _IXASessionEvents, _IXAQueryEvents, _IXARealEvents
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private const int MaxQueryableCount = 200;

        private int queryableCount;
        public int QueryableCount
        {
            get { return queryableCount; }
        }

        public bool IsQueryable
        {
            get { return (queryableCount < MaxQueryableCount); }
        }

        private LoginInfo loginInfo;

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

        private readonly string loginInfoFile = "c:\\perfLogs\\etradeloginInfo.xml";
        private readonly string resFilePath = "\\Res";

        private AutoResetEvent waitQuoting;
        private StockMaster quotingStockMaster;
        private IndexMaster quotingIndexMaster;

        private Dictionary<string, IndexConclusion> prevIndexConclusions;

        private bool isAnyDataReceived = false;

        #region Etrade Specific
        public string Server { get; set; }

        public int Port { get; set; }

        public string LastError { get; set; }

        public string LastErrorMsg { get; set; }

        private IXASession session;
        private IXAQuery currentPriceQueryObj;
        private IXAQuery currentIndexQueryObj;

        private IXAReal indexPriceSubscribingObj;

        private UCOMIConnectionPoint m_icp;
        private UCOMIConnectionPointContainer m_icpc;
        private int m_dwCookie;
        #endregion

        private EbestProvider()
        {
            try
            {
                waitQuoting = new AutoResetEvent(false);
                prevIndexConclusions = new Dictionary<string, IndexConclusion>();

                loginInfo = new LoginInfo();
                loginInfo.GUID = Guid.NewGuid();
                loginInfo.LoginState = LoginState.LoggedOut;
                loginInfo.UserId = Config.Ebest.UserId;
                loginInfo.UserPw = Config.Ebest.UserPw;
                loginInfo.CertPw = Config.Ebest.CertPw;
                loginInfo.AccountPw = Config.Ebest.AccountPw;

                if (loginInfo.ProviderType == BrokerageServerType.Real)
                    Server = Config.Ebest.ServerAddress;
                else
                    Server = Config.Ebest.DemoServerAddress;

                Port = Config.Ebest.ServerPort;

                int m_dwCookie = 0;
                session = new XASession();
                m_icpc = (UCOMIConnectionPointContainer)session;
                Guid IID_SessionEvents = typeof(_IXASessionEvents).GUID;
                m_icpc.FindConnectionPoint(ref IID_SessionEvents, out m_icp);
                m_icp.Advise(this, out m_dwCookie);

                currentPriceQueryObj = new XAQuery();
                currentPriceQueryObj.ResFileName = resFilePath + "\\t1102.res";
                m_icpc = (UCOMIConnectionPointContainer)currentPriceQueryObj;
                Guid IID_QueryEvents = typeof(_IXAQueryEvents).GUID;
                m_icpc.FindConnectionPoint(ref IID_QueryEvents, out m_icp);
                m_icp.Advise(this, out m_dwCookie);

                currentIndexQueryObj = new XAQuery();
                currentIndexQueryObj.ResFileName = resFilePath + "\\t1511.res";
                m_icpc = (UCOMIConnectionPointContainer)currentIndexQueryObj;
                IID_QueryEvents = typeof(_IXAQueryEvents).GUID;
                m_icpc.FindConnectionPoint(ref IID_QueryEvents, out m_icp);
                m_icp.Advise(this, out m_dwCookie);

                indexPriceSubscribingObj = new XAReal();
                indexPriceSubscribingObj.ResFileName = resFilePath + "\\IJ_.res";
                m_icpc = (UCOMIConnectionPointContainer)indexPriceSubscribingObj;
                Guid IID_RealEvents = typeof(_IXARealEvents).GUID;
                m_icpc.FindConnectionPoint(ref IID_RealEvents, out m_icp);
                m_icp.Advise(this, out m_dwCookie);

                //LogIn();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public bool LogIn()
        {
            bool ret = false;

            try
            {
                ret = session.ConnectServer(Server, Port);

                if (ret == false)
                {
                    logger.Error("Server not connected");
                    return ret;
                }

                if (loginInfo.ProviderType == BrokerageServerType.Real)
                    ret = session.Login(loginInfo.UserId, loginInfo.UserPw, loginInfo.CertPw, (int)XA_SESSIONLib.XA_SERVER_TYPE.XA_REAL_SERVER, true);
                else
                    ret = session.Login(loginInfo.UserId, loginInfo.UserPw, loginInfo.CertPw, (int)XA_SESSIONLib.XA_SERVER_TYPE.XA_SIMUL_SERVER, true);

                if (ret == true)
                {
                    loginInfo.LoginState = LoginState.LoggedIn;
                    ThreadStart loginStateCheckThreadStarter = new ThreadStart(LoginStateCheckerThread);
                    Thread loginStateCheckThread = new Thread(loginStateCheckThreadStarter);
                    loginStateCheckThread.Start();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return ret;
        }

        public bool LogOut()
        {
            bool ret = false;

            try
            {
                if (loginInfo.LoginState != LoginState.LoggedIn)
                    return false;

                ret = session.Logout();
                session.DisconnectServer();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                loginInfo.LoginState = LoginState.LoggingOut;
            }

            return ret;
        }

        private void LoginStateCheckerThread() // TODO : 확인 필요함
        {
            Thread.CurrentThread.Name = "LoginStateCheckerThread";
            //Thread.Sleep(10000);
            while (loginInfo.LoginState != LoginState.LoggedOut)
            {
                if (isAnyDataReceived == false)
                {
                    IndexMaster indexMaster = new IndexMaster();
                    //GetQuote("001", ref indexMaster);
                }
                isAnyDataReceived = false;
                Thread.Sleep(1000 * 60 * 10);
            }

            loginInfo.LoginState = LoginState.LoggedOut;

            if (loginInfo.LoginState == LoginState.LoggedOut)
                logger.Info("Session disconnected");
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
                currentPriceQueryObj.SetFieldData("t1102InBlock", "shcode", 0, code);

                int retryCount = 5;
                while (retryCount-- > 0)
                {
                    ret = currentPriceQueryObj.Request(false);

                    if (ret == -21)
                        Thread.Sleep(1000);
                    else
                        break;
                }

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

            return (ret >= 0);
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
                currentIndexQueryObj.SetFieldData("t1511InBlock", "upcode", 0, code);

                int retryCount = 5;
                while (retryCount-- > 0)
                {
                    ret = currentIndexQueryObj.Request(false);

                    if (ret == -21)
                        Thread.Sleep(1000);
                    else
                        break;
                }

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

            return (ret >= 0);
        }

        public bool IndexSubscribe(string code)
        {
            if (IsQueryable == false)
            {
                logger.Error($"Not subscribable, Code: {code}");
                return false;
            }

            try
            {
                indexPriceSubscribingObj.SetFieldData("InBlock", "upcode", code);
                indexPriceSubscribingObj.AdviseRealData();

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


        #region _IXARealEvents 멤버

        void _IXARealEvents.RecieveLinkData(string szLinkName, string szData, string szFiller)
        {
        }

        void _IXARealEvents.ReceiveRealData(string szTrCode)
        {
            if (szTrCode == "IJ_")
                IndexPriceReceived(szTrCode);

            isAnyDataReceived = true;
        }

        private void IndexPriceReceived(string szTrCode)
        {
            try
            {
                IndexConclusion indexUpdated = new IndexConclusion();

                string temp = indexPriceSubscribingObj.GetFieldData("OutBlock", "upcode");
                indexUpdated.Code = temp;

                temp = indexPriceSubscribingObj.GetFieldData("OutBlock", "time");
                var now = DateTime.Now;
                uint time;

                if (uint.TryParse(temp, out time) == true)
                    indexUpdated.ConcludedTime = new DateTime(now.Year, now.Month, now.Day, (int)(time / 10000), (int)((time / 100) % 100), (int)time % 100, now.Millisecond);
                else
                    indexUpdated.ConcludedTime = now;

                temp = indexPriceSubscribingObj.GetFieldData("OutBlock", "jisu");
                double index;
                if (double.TryParse(temp, out index) == true)
                    indexUpdated.Index = index;

                temp = indexPriceSubscribingObj.GetFieldData("OutBlock", "volume");
                double volumn;
                if (double.TryParse(temp, out volumn) == true)
                    indexUpdated.Volume = volumn * 1000;

                temp = indexPriceSubscribingObj.GetFieldData("OutBlock", "value");
                double value;
                if (double.TryParse(temp, out value) == true)
                    indexUpdated.Value = value;

                temp = indexPriceSubscribingObj.GetFieldData("OutBlock", "upjo");
                int upperLimitCount;
                if (int.TryParse(temp, out upperLimitCount) == true)
                    indexUpdated.UpperLimitedItemCount = upperLimitCount;

                temp = indexPriceSubscribingObj.GetFieldData("OutBlock", "highjo");
                int increasingCount;
                if (int.TryParse(temp, out increasingCount) == true)
                    indexUpdated.IncreasingItemCount = increasingCount;

                temp = indexPriceSubscribingObj.GetFieldData("OutBlock", "unchgjo");
                int steadyCount;
                if (int.TryParse(temp, out steadyCount) == true)
                    indexUpdated.SteadyItemCount = steadyCount;

                temp = indexPriceSubscribingObj.GetFieldData("OutBlock", "lowjo");
                int decreasingCount;
                if (int.TryParse(temp, out decreasingCount) == true)
                    indexUpdated.DecreasingItemCount = decreasingCount;

                temp = indexPriceSubscribingObj.GetFieldData("OutBlock", "downjo");
                int lowerLimitedCount;
                if (int.TryParse(temp, out lowerLimitedCount) == true)
                    indexUpdated.LowerLimitedItemCount = lowerLimitedCount;

                if (prevIndexConclusions.ContainsKey(indexUpdated.Code) == false)
                    prevIndexConclusions.Add(indexUpdated.Code, new IndexConclusion());

                if (prevIndexConclusions[indexUpdated.Code].Value == indexUpdated.Value && 
                    prevIndexConclusions[indexUpdated.Code].Volume == indexUpdated.Volume)
                    return;

                double newReceived;
                newReceived = indexUpdated.Value;
                indexUpdated.Value = indexUpdated.Value - prevIndexConclusions[indexUpdated.Code].Value;
                prevIndexConclusions[indexUpdated.Code].Value = newReceived;

                newReceived = indexUpdated.Volume;
                indexUpdated.Volume = indexUpdated.Volume - prevIndexConclusions[indexUpdated.Code].Volume;
                prevIndexConclusions[indexUpdated.Code].Volume = newReceived;

                // TODO : Conclusion 처리 추가해야함
                //if (IndexValueUpdated != null)
                //{
                //    IndexValueUpdated(indexUpdated);
                //}
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
        #endregion

        #region _IXASessionEvents 멤버

        void _IXASessionEvents.Login(string szCode, string szMsg)
        {
            if (szCode == "0000")
                loginInfo.LoginState = LoginState.LoggedIn;

            LastError = szCode;
            LastErrorMsg = szMsg;
            logger.Info($"Login sucess, szCode: {szCode}, szMsg: {szMsg}");
        }

        void _IXASessionEvents.Logout()
        {
            loginInfo.LoginState = LoginState.LoggedOut;

            LastErrorMsg = "LogOut";
            logger.Info("Logged out");
        }

        void _IXASessionEvents.Disconnect()
        {
            loginInfo.LoginState = LoginState.LoggedOut;

            LastErrorMsg = "Disconnect";
            logger.Info("Disconnected");
        }
        #endregion

        #region _IXAQueryEvents 멤버
        void _IXAQueryEvents.ReceiveData(string szTrCode)
        {
            if (szTrCode == "t1102")
                CurrentPriceQueryObj_Received();
            else if (szTrCode == "t1511")
                CurrentIndexQueryObj_Received();

            isAnyDataReceived = true;
        }

        void _IXAQueryEvents.ReceiveMessage(bool bIsSystemError, string nMessageCode, string szMessage)
        {
        }

        void _IXAQueryEvents.ReceiveChartRealData(string szTrCode)
        {
        }

        private void CurrentPriceQueryObj_Received()
        {
            try
            {
                if (quotingStockMaster == null)
                    return;

                string temp = currentPriceQueryObj.GetFieldData("t1102OutBlock", "price", 0);
                if (temp == "") temp = "0";
                quotingStockMaster.BasisPrice = int.Parse(temp); // 현재가 TODO : 맞나?

                temp = currentPriceQueryObj.GetFieldData("t1102OutBlock", "jnilvolume", 0);
                if (temp == "") temp = "0";
                quotingStockMaster.PreviousVolume = int.Parse(temp); //전일거래량

                temp = currentPriceQueryObj.GetFieldData("t1102OutBlock", "abscnt", 0);
                if (temp == "") temp = "0";
                quotingStockMaster.CirculatingVolume = int.Parse(temp);  //유통주식수

                string valueAltered = currentPriceQueryObj.GetFieldData("t1102OutBlock", "info1", 0);

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

        private void CurrentIndexQueryObj_Received()
        {
            try
            {
                if (quotingIndexMaster == null)
                    return;

                string temp = currentIndexQueryObj.GetFieldData("t1511OutBlock", "jniljisu", 0);
                if (temp == "") temp = "0";
                quotingIndexMaster.PreviousClosedPrice = Convert.ToDouble(temp); // 현재가

                temp = currentIndexQueryObj.GetFieldData("t1511OutBlock", "jnilvolume", 0);
                if (temp == "") temp = "0";
                quotingIndexMaster.PreviousVolume = Convert.ToInt64(temp); //전일거래량

                temp = currentIndexQueryObj.GetFieldData("t1511OutBlock", "jnilvalue", 0);
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
        #endregion
    }
}
