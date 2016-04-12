using MTree.Configuration;
using MTree.Trader;
using MTree.Utility;
using System;
using System.ComponentModel;
using System.ServiceModel;
using System.Threading;
using System.Timers;
using XA_DATASETLib;
using XA_SESSIONLib;

namespace MTree.EbestTrader
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Single)]
    public partial class EbestTrader : ITrader, INotifyPropertyChanged
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly string resFilePath = "\\Res";

        public LoginInfo LoginInstance { get; } = new LoginInfo();

        private int WaitTimeout { get; } = 1000 * 10;
        private AutoResetEvent WaitDepositEvent { get; } = new AutoResetEvent(false);

        private int WaitLoginTimeout { get; } = 1000 * 15;
        private ManualResetEvent WaitLoginEvent { get; } = new ManualResetEvent(false);

        #region Keep session
        private int MaxCommInterval { get; } = 1000 * 60 * 20; // 통신 안한지 20분 넘어가면 Quote 시작
        private int CommTimerInterval { get; } = 1000 * 60 * 2; // 2분마다 체크
        private int LastCommTick { get; set; } = Environment.TickCount;
        private System.Timers.Timer CommTimer { get; set; }
        #endregion

        #region Ebest Specific
        private XASessionClass sessionObj;
        private XAQueryClass stockQuotingObj;
        private XAQueryClass newOrderObj;
        private XAQueryClass modifyOrderObj;
        private XAQueryClass cancelOrderObj;
        private XAQueryClass accDepositObj;

        private XARealClass orderSubmittedObj;
        private XARealClass orderConcludedObj;
        private XARealClass orderModifiedObj;
        private XARealClass orderCanceledObj;
        private XARealClass orderRejectedObj;
        #endregion

        public EbestTrader()
        {
            try
            {
                CommTimer = new System.Timers.Timer(CommTimerInterval);
                CommTimer.Elapsed += OnCommunTimer;
                CommTimer.AutoReset = true;

                #region XASession
                sessionObj = new XASessionClass();
                sessionObj.Disconnect += SessionObj_Disconnect;
                sessionObj._IXASessionEvents_Event_Login += SessionObj_Event_Login;
                sessionObj._IXASessionEvents_Event_Logout += SessionObj_Event_Logout;
                #endregion

                #region XAQuery
                stockQuotingObj = new XAQueryClass();
                stockQuotingObj.ResFileName = resFilePath + "\\t1102.res";
                stockQuotingObj.ReceiveData += StockQuotingObj_ReceiveData;

                newOrderObj = new XAQueryClass();
                newOrderObj.ResFileName = resFilePath + "\\CSPAT00600.res";
                newOrderObj.ReceiveData += NewOrderObj_ReceiveData;

                modifyOrderObj = new XAQueryClass();
                modifyOrderObj.ResFileName = resFilePath + "\\CSPAT00700.res";
                modifyOrderObj.ReceiveData += ModifyOrderObj_ReceiveData;

                cancelOrderObj = new XAQueryClass();
                cancelOrderObj.ResFileName = resFilePath + "\\CSPAT00800.res";
                cancelOrderObj.ReceiveData += CancelOrderObj_ReceiveData;

                accDepositObj = new XAQueryClass();
                accDepositObj.ResFileName = resFilePath + "\\t0424.res";
                accDepositObj.ReceiveData += AccDepositObj_ReceiveData;
                #endregion

                #region XAReal
                orderSubmittedObj = new XARealClass();
                orderSubmittedObj.ResFileName = resFilePath + "\\SC0.res";
                orderSubmittedObj.ReceiveRealData += OrderSubmittedObj_ReceiveRealData;
                orderSubmittedObj.AdviseRealData();

                orderConcludedObj = new XARealClass();
                orderConcludedObj.ResFileName = resFilePath + "\\SC1.res";
                orderConcludedObj.ReceiveRealData += OrderConcludedObj_ReceiveRealData;
                orderConcludedObj.AdviseRealData();

                orderModifiedObj = new XARealClass();
                orderModifiedObj.ResFileName = resFilePath + "\\SC2.res";
                orderModifiedObj.ReceiveRealData += OrderModifiedObj_ReceiveRealData;
                orderModifiedObj.AdviseRealData();

                orderCanceledObj = new XARealClass();
                orderCanceledObj.ResFileName = resFilePath + "\\SC3.res";
                orderCanceledObj.ReceiveRealData += OrderCanceledObj_ReceiveRealData;
                orderCanceledObj.AdviseRealData();

                orderRejectedObj = new XARealClass();
                orderRejectedObj.ResFileName = resFilePath + "\\SC3.res";
                orderRejectedObj.ReceiveRealData += OrderRejectedObj_ReceiveRealData;
                orderRejectedObj.AdviseRealData();
                #endregion

                #region Login
                LoginInstance.UserId = Config.Ebest.UserId;
                LoginInstance.UserPw = Config.Ebest.UserPw;
                LoginInstance.CertPw = Config.Ebest.CertPw;
                LoginInstance.AccountPw = Config.Ebest.AccountPw;
                if (Config.General.TraderType == TraderTypes.EbestSimul)
                {
                    LoginInstance.ServerType = ServerTypes.Simul;
                    LoginInstance.ServerAddress = Config.Ebest.SimulServerAddress;
                }
                else
                {
                    LoginInstance.ServerType = ServerTypes.Real;
                    LoginInstance.ServerAddress = Config.Ebest.RealServerAddress;
                }
                LoginInstance.ServerPort = Config.Ebest.ServerPort;

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
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        #region XASession
        private void SessionObj_Event_Logout()
        {
            CommTimer.Stop();
            LoginInstance.State = LoginStates.LoggedOut;
            logger.Info(LoginInstance.ToString());
        }

        private void SessionObj_Event_Login(string szCode, string szMsg)
        {
            if (szCode == "0000")
            {
                LoginInstance.State = LoginStates.LoggedIn;
                logger.Info($"Login success, {LoginInstance.ToString()}");
                SetLogin();
            }
            else
            {
                logger.Error($"Login fail, szCode: {szCode}, szMsg: {szMsg}");
            }
        }

        private void SessionObj_Disconnect()
        {
            CommTimer.Stop();
            LoginInstance.State = LoginStates.Disconnect;
            logger.Error(LoginInstance.ToString());
        }
        #endregion

        #region Login / Logout

        public bool WaitLogin()
        {
            if (WaitLoginEvent.WaitOne(WaitLoginTimeout) == false)
            {
                logger.Error($"{GetType().Name} wait login timeout");
                return false;
            }

            return true;
        }

        private void SetLogin()
        {
            Thread.Sleep(1000 * 3); // 로그인후 대기

            logger.Info($"{GetType().Name} set login");
            WaitLoginEvent.Set();
        }

        public bool Login()
        {
            try
            {
                if (sessionObj.ConnectServer(LoginInstance.ServerAddress, LoginInstance.ServerPort) == false)
                {
                    logger.Error($"Server connection fail, {GetLastErrorMessage()}");
                    return false;
                }

                logger.Info($"Try login, Id: {LoginInstance.UserId}");

                if (sessionObj.Login(LoginInstance.UserId, LoginInstance.UserPw, LoginInstance.CertPw, 0, true) == false)
                {
                    logger.Error($"Login error, {GetLastErrorMessage()}");
                    return false;
                }

                CommTimer.Start();
                return true;
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
                CommTimer.Stop();
                sessionObj.DisconnectServer();
                LoginInstance.State = LoginStates.Disconnect;

                logger.Info("Logout success");
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return false;
        }
        #endregion

        private string GetLastErrorMessage(int errCode = 0)
        {
            if (errCode == 0)
                errCode = sessionObj.GetLastError();
            var errMsg = sessionObj.GetErrorMessage(errCode);

            return $"errCode: {errCode}, errMsg: {errMsg}";
        }

        private void OnCommunTimer(object sender, ElapsedEventArgs e)
        {
            if ((Environment.TickCount - LastCommTick) > MaxCommInterval)
            {
                LastCommTick = Environment.TickCount;
                logger.Info($"Ebest keep alive");
                KeepAlive();
            }
        }

        public void KeepAlive()
        {
            try
            {
                stockQuotingObj.SetFieldData("t1102InBlock", "shcode", 0, "000020");
                var ret = stockQuotingObj.Request(false);
                if (ret < 0)
                    logger.Error($"Keep alive error, {GetLastErrorMessage(ret)}");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private void StockQuotingObj_ReceiveData(string szTrCode)
        {
            LastCommTick = Environment.TickCount;
            logger.Trace($"szTrCode: {szTrCode}");
        }

        private void NewOrderObj_ReceiveData(string szTrCode)
        {
            try
            {
                LastCommTick = Environment.TickCount;
                logger.Trace($"szTrCode: {szTrCode}");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private void CancelOrderObj_ReceiveData(string szTrCode)
        {
            try
            {
                LastCommTick = Environment.TickCount;
                logger.Trace($"szTrCode: {szTrCode}");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private void ModifyOrderObj_ReceiveData(string szTrCode)
        {
            try
            {
                LastCommTick = Environment.TickCount;
                logger.Trace($"szTrCode: {szTrCode}");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private void AccDepositObj_ReceiveData(string szTrCode)
        {
            try
            {
                LastCommTick = Environment.TickCount;
                logger.Trace($"szTrCode: {szTrCode}");

                CurrDeposit = long.Parse(accDepositObj.GetFieldData("t0424OutBlock", "sunamt", 0));
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                WaitDepositEvent.Set();
            }
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
