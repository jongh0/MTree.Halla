using MTree.Configuration;
using MTree.Publisher;
using MTree.Trader;
using System;
using System.ComponentModel;
using System.ServiceModel;
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

        #region Keep session
        private int MaxCommInterval { get; } = 1000 * 60 * 20; // 통신 안한지 20분 넘어가면 Quote 시작
        private int CommTimerInterval { get; } = 1000 * 60 * 2; // 2분마다 체크
        private int LastCommTick { get; set; } = Environment.TickCount;
        private System.Timers.Timer CommTimer { get; set; }
        #endregion

        #region Ebest Specific
        private XASessionClass sessionObj;
        private XAQueryClass stockQuotingObj;
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
                stockQuotingObj.ReceiveData += queryObj_ReceiveData;
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

            LoginInstance.State = LoginStates.Logout;
            logger.Info(LoginInstance.ToString());
        }

        private void sessionObj_Event_Login(string szCode, string szMsg)
        {
            LastCommTick = Environment.TickCount;
            LoginInstance.State = LoginStates.Login;
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
            try
            {
                if (sessionObj.ConnectServer(LoginInstance.ServerAddress, LoginInstance.ServerPort) == false)
                {
                    logger.Error("Server connection fail");
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

                logger.Error("Login error");
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

                if (LoginInstance.State != LoginStates.Login)
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
                stockQuotingObj.Request(false);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private void queryObj_ReceiveData(string szTrCode)
        {
            LastCommTick = Environment.TickCount;
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
