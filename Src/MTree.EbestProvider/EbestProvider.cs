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
    public class EbestProvider
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private const int maxQueryableCount = 200;
        private int queryableCount = 0;

        public bool IsQueryable
        {
            get { return (queryableCount < maxQueryableCount); }
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

        private readonly string resFilePath = "\\Res";

        private Dictionary<string, IndexConclusion> prevIndexConclusions;

        public string Server { get; set; }

        public int Port { get; set; }

        private XASessionClass xaSession;
        private XARealClass xaReal;

        private EbestProvider()
        {
            try
            {
                xaSession = new XASessionClass();
                xaSession.Disconnect += XaSession_Disconnect;
                xaSession._IXASessionEvents_Event_Login += XaSession__IXASessionEvents_Event_Login;
                xaSession._IXASessionEvents_Event_Logout += XaSession__IXASessionEvents_Event_Logout;

                xaReal = new XARealClass();
                xaReal.ReceiveRealData += XaReal_ReceiveRealData;
                xaReal.RecieveLinkData += XaReal_RecieveLinkData;
                xaReal.ResFileName = resFilePath + "\\IJ_.res";

                loginInfo = new LoginInfo();
                loginInfo.LoginState = LoginState.Disconnected;
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
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private void XaReal_RecieveLinkData(string szLinkName, string szData, string szFiller)
        {
        }

        private void XaReal_ReceiveRealData(string szTrCode)
        {
            if (szTrCode == "IJ_")
                IndexPriceReceived(szTrCode);
        }

        private void XaSession__IXASessionEvents_Event_Logout()
        {
            loginInfo.LoginState = LoginState.LoggedOut;
        }

        private void XaSession__IXASessionEvents_Event_Login(string szCode, string szMsg)
        {
            loginInfo.LoginState = LoginState.LoggedIn;
        }

        private void XaSession_Disconnect()
        {
            loginInfo.LoginState = LoginState.Disconnected;
        }

        public bool Login()
        {
            bool ret = false;

            try
            {
                if (xaSession.ConnectServer(Server, Port) == true)
                {
                    logger.Info("Server connected");

                    if (loginInfo.ProviderType == BrokerageServerType.Real)
                        ret = xaSession.Login(loginInfo.UserId, loginInfo.UserPw, loginInfo.CertPw, (int)XA_SERVER_TYPE.XA_REAL_SERVER, true);
                    else
                        ret = xaSession.Login(loginInfo.UserId, loginInfo.UserPw, loginInfo.CertPw, (int)XA_SERVER_TYPE.XA_SIMUL_SERVER, true);

                    if (ret == true)
                        logger.Info("Login success");
                    else
                        logger.Error("Login fail");
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
                if (xaSession.IsConnected() == false)
                    return false;

                if (loginInfo.LoginState != LoginState.LoggedIn)
                    return false;

                xaSession.Logout();
                xaSession.DisconnectServer();

                logger.Info("Logout success");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return false;
            }

            return true;
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
                xaReal.SetFieldData("InBlock", "upcode", code);
                xaReal.AdviseRealData();

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

        public bool IndexUnsubscribe(string code)
        {
            try
            {
                xaReal.SetFieldData("InBlock", "upcode", code);
                xaReal.UnadviseRealData();

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

        private void IndexPriceReceived(string szTrCode)
        {
            try
            {
                IndexConclusion indexUpdated = new IndexConclusion();

                string temp = xaReal.GetFieldData("OutBlock", "upcode");
                indexUpdated.Code = temp;

                temp = xaReal.GetFieldData("OutBlock", "time");
                var now = DateTime.Now;
                uint time;

                if (uint.TryParse(temp, out time) == true)
                    indexUpdated.Time = new DateTime(now.Year, now.Month, now.Day, (int)(time / 10000), (int)((time / 100) % 100), (int)time % 100, now.Millisecond);
                else
                    indexUpdated.Time = now;

                temp = xaReal.GetFieldData("OutBlock", "jisu");
                double index;
                if (double.TryParse(temp, out index) == true)
                    indexUpdated.Index = index;

                temp = xaReal.GetFieldData("OutBlock", "volume");
                double volumn;
                if (double.TryParse(temp, out volumn) == true)
                    indexUpdated.Volume = volumn * 1000;

                temp = xaReal.GetFieldData("OutBlock", "value");
                double value;
                if (double.TryParse(temp, out value) == true)
                    indexUpdated.Value = value;

                temp = xaReal.GetFieldData("OutBlock", "upjo");
                int upperLimitCount;
                if (int.TryParse(temp, out upperLimitCount) == true)
                    indexUpdated.UpperLimitedItemCount = upperLimitCount;

                temp = xaReal.GetFieldData("OutBlock", "highjo");
                int increasingCount;
                if (int.TryParse(temp, out increasingCount) == true)
                    indexUpdated.IncreasingItemCount = increasingCount;

                temp = xaReal.GetFieldData("OutBlock", "unchgjo");
                int steadyCount;
                if (int.TryParse(temp, out steadyCount) == true)
                    indexUpdated.SteadyItemCount = steadyCount;

                temp = xaReal.GetFieldData("OutBlock", "lowjo");
                int decreasingCount;
                if (int.TryParse(temp, out decreasingCount) == true)
                    indexUpdated.DecreasingItemCount = decreasingCount;

                temp = xaReal.GetFieldData("OutBlock", "downjo");
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
    }
}
