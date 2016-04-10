using MTree.Configuration;
using MTree.Publisher;
using MTree.Trader;
using System;
using System.ComponentModel;
using System.ServiceModel;
using System.Threading;
using System.Timers;
using XA_DATASETLib;
using XA_SESSIONLib;

namespace MTree.EbestTrader
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public partial class EbestTrader : ITrader, INotifyPropertyChanged
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly string resFilePath = "\\Res";

        public LoginInfo LoginInstance { get; } = new LoginInfo();

        private int WaitTimeout { get; } = 1000 * 10;
        private AutoResetEvent WaitDepositEvent { get; } = new AutoResetEvent(false);
        private AutoResetEvent WaitOrderEvent { get; } = new AutoResetEvent(false);

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
                sessionObj.Disconnect += sessionObj_Disconnect;
                sessionObj._IXASessionEvents_Event_Login += sessionObj_Event_Login;
                sessionObj._IXASessionEvents_Event_Logout += sessionObj_Event_Logout;
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

                #region Login
                LoginInstance.UserId = Config.Ebest.UserId;
                LoginInstance.UserPw = Config.Ebest.UserPw;
                LoginInstance.CertPw = Config.Ebest.CertPw;
                LoginInstance.AccountPw = Config.Ebest.AccountPw;
                if (Config.General.SimulTrade == true)
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
        private void sessionObj_Event_Logout()
        {
            LastCommTick = Environment.TickCount;

            CommTimer?.Stop();

            LoginInstance.State = LoginStates.LoggedOut;
            logger.Info(LoginInstance.ToString());
        }

        private void sessionObj_Event_Login(string szCode, string szMsg)
        {
            LastCommTick = Environment.TickCount;

            if (szCode == "0000")
            {
                logger.Info("Login success");
                LoginInstance.State = LoginStates.LoggedIn;
            }
            else
            {
                logger.Error($"Login fail, szCode: {szCode}, szMsg: {szMsg}");
            }
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
            try
            {
                if (sessionObj.ConnectServer(LoginInstance.ServerAddress, LoginInstance.ServerPort) == false)
                {
                    logger.Error($"Server connection fail, {GetLastErrorMessage()}");
                    return false;
                }

                logger.Info("Server connected");

                int serverType = LoginInstance.ServerType == ServerTypes.Real ? (int)XA_SERVER_TYPE.XA_REAL_SERVER : (int)XA_SERVER_TYPE.XA_SIMUL_SERVER;

                if (sessionObj.Login(LoginInstance.UserId, LoginInstance.UserPw, LoginInstance.CertPw, serverType, true) == true)
                {
                    logger.Info($"Try login with id: {LoginInstance.UserId}");
                    CommTimer.Start();
                    return true;
                }

                logger.Error($"Login error, {GetLastErrorMessage()}");
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

                if (LoginInstance.State != LoginStates.LoggedIn)
                    return false;

                sessionObj.Logout();
                sessionObj.DisconnectServer();

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

        private string GetLastErrorMessage()
        {
            var errCode = sessionObj.GetLastError();
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
                if (stockQuotingObj.Request(false) < 0)
                    logger.Error($"Keep alive error, {GetLastErrorMessage()}");
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
            finally
            {
                WaitOrderEvent.Set();
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
            finally
            {
                WaitOrderEvent.Set();
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
            finally
            {
                WaitOrderEvent.Set();
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
