using Configuration;
using RealTimeProvider;
using Trader;
using CommonLib;
using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.ServiceModel;
using System.Threading;
using System.Timers;
using XA_DATASETLib;
using XA_SESSIONLib;
using CommonLib.Firm;
using CommonLib.Extension;
using CommonLib.Firm.Ebest;
using CommonLib.Firm.Ebest.Block;
using CommonLib.Firm.Ebest.Query;
using CommonLib.Firm.Ebest.Real;
using System.Threading.Tasks;

namespace EbestTrader
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, ValidateMustUnderstand = false)]
    public partial class EbestTrader_ : IRealTimeTrader, ITrader, INotifyPropertyChanged
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private ConcurrentDictionary<Guid, TraderContract> TraderContracts { get; set; } = new ConcurrentDictionary<Guid, TraderContract>();

        public LoginInformation LoginInfo { get; } = new LoginInformation();

        private const int WaitLoginTimeout = 1000 * 15;
        private ManualResetEvent WaitLoginEvent { get; } = new ManualResetEvent(false);

        #region Event
        public event Action<TraderStateTypes, string> StateNotified;
        #endregion

        #region Keep session
        private int MaxCommInterval => 1000 * 60 * 20; // 통신 안한지 20분 넘어가면 Quote 시작
        private int CommTimerInterval => 1000 * 60 * 2; // 2분마다 체크
        private int LastCommTick { get; set; } = Environment.TickCount;
        private System.Timers.Timer CommTimer { get; set; }
        #endregion

        #region Ebest Specific
        private XASessionClass _session;

        private EbestReal<SC0OutBlock> _orderSubmitReal;
        private EbestReal<SC1OutBlock> _orderConclusionReal;
        private EbestReal<SC2OutBlock> _orderModifiedReal;
        private EbestReal<SC3OutBlock> _orderCanceledReal;
        private EbestReal<SC4OutBlock> _orderRejectedReal;
        #endregion

        public EbestTrader_()
        {
            try
            {
                CommTimer = new System.Timers.Timer(CommTimerInterval);
                CommTimer.Elapsed += OnCommunTimer;
                CommTimer.AutoReset = true;

                #region XASession
                _session = new XASessionClass();
                _session.Disconnect += SessionObj_Disconnect;
                _session._IXASessionEvents_Event_Login += Session_Event_Login;
                _session._IXASessionEvents_Event_Logout += Session_Event_Logout;
                #endregion

                #region XAReal
                _orderSubmitReal = new EbestReal<SC0OutBlock>();
                _orderSubmitReal.OutBlockReceived += OrderSubmitReal_OutBlockReceived;
                _orderConclusionReal = new EbestReal<SC1OutBlock>();
                _orderConclusionReal.OutBlockReceived += OrderConclusionReal_OutBlockReceived;
                _orderModifiedReal = new EbestReal<SC2OutBlock>();
                _orderModifiedReal.OutBlockReceived += OrderModifiedReal_OutBlockReceived;
                _orderCanceledReal = new EbestReal<SC3OutBlock>();
                _orderCanceledReal.OutBlockReceived += OrderCanceledReal_OutBlockReceived;
                _orderRejectedReal = new EbestReal<SC4OutBlock>();
                _orderRejectedReal.OutBlockReceived += OrderRejectedReal_OutBlockReceived;
                #endregion

                #region Login
                LoginInfo.UserId = Config.Ebest.UserId;
                LoginInfo.UserPw = Config.Ebest.UserPw;
                LoginInfo.CertPw = Config.Ebest.CertPw;
                if (Config.General.TraderType == TraderTypes.EbestSimul)
                {
                    LoginInfo.AccountPw = "0000";
                    LoginInfo.ServerType = ServerTypes.Simul;
                    LoginInfo.ServerAddress = Config.Ebest.SimulServerAddress;
                }
                else
                {
                    LoginInfo.AccountPw = Config.Ebest.AccountPw;
                    LoginInfo.ServerType = ServerTypes.Real;
                    LoginInfo.ServerAddress = Config.Ebest.RealServerAddress;
                }
                LoginInfo.ServerPort = Config.Ebest.ServerPort;

                if (string.IsNullOrEmpty(LoginInfo.UserId) == false &&
                    string.IsNullOrEmpty(LoginInfo.UserPw) == false &&
                    string.IsNullOrEmpty(LoginInfo.CertPw) == false)
                {
                    Login();
                }
                else
                {
                    _logger.Error("Check Ebest configuration");
                    return;
                }
                #endregion
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private void AdviseRealData()
        {
            try
            {
                _orderSubmitReal.AdviseRealData();
                _orderConclusionReal.AdviseRealData();
                _orderModifiedReal.AdviseRealData();
                _orderCanceledReal.AdviseRealData();
                _orderRejectedReal.AdviseRealData();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }


        #region XASession
        private void Session_Event_Logout()
        {
            CommTimer.Stop();
            LoginInfo.State = LoginStates.Logout;
            _logger.Info(LoginInfo.ToString());
        }

        private void Session_Event_Login(string szCode, string szMsg)
        {
            if (szCode == "0000")
            {
                LoginInfo.State = LoginStates.Login;
                _logger.Info($"Login success, {LoginInfo.ToString()}");
                SetLogin();

                NotifyState(TraderStateTypes.LoginSuccess, "Login success");
                AdviseRealData();
            }
            else
            {
                _logger.Error($"Login fail, szCode: {szCode}, szMsg: {szMsg}");
                NotifyState(TraderStateTypes.LoginFail, "Login fail");
            }
        }

        private void SessionObj_Disconnect()
        {
            CommTimer.Stop();
            LoginInfo.State = LoginStates.Disconnect;
            _logger.Error(LoginInfo.ToString());
        }
        #endregion

        #region Login / Logout
        public bool WaitLogin()
        {
            if (WaitLoginEvent.WaitOne(WaitLoginTimeout) == false)
            {
                _logger.Error($"{GetType().Name} login timeout");
                return false;
            }

            return true;
        }

        private void SetLogin()
        {
            Thread.Sleep(1000 * 3); // 로그인후 대기

            _logger.Info($"{GetType().Name} set login");
            WaitLoginEvent.Set();
        }

        public bool Login()
        {
            try
            {
                if (_session.ConnectServer(LoginInfo.ServerAddress, LoginInfo.ServerPort) == false)
                {
                    _logger.Error($"Server connection fail, {GetLastErrorMessage()}");
                    return false;
                }

                _logger.Info($"Try login, Id: {LoginInfo.UserId}");

                if (_session.Login(LoginInfo.UserId, LoginInfo.UserPw, LoginInfo.CertPw, (int)LoginInfo.ServerType, true) == false)
                {
                    _logger.Error($"Login error, {GetLastErrorMessage()}");
                    return false;
                }

                CommTimer.Start();
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return false;
        }

        public bool Logout()
        {
            try
            {
                CommTimer.Stop();
                _session.DisconnectServer();
                LoginInfo.State = LoginStates.Disconnect;

                _logger.Info("Logout success");
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return false;
        }
        #endregion

        private string GetLastErrorMessage(int errCode = 0)
        {
            if (errCode == 0)
                errCode = _session.GetLastError();

            if (errCode >= 0) return string.Empty;

            var errMsg = _session.GetErrorMessage(errCode);

            return $"errCode: {errCode}, errMsg: {errMsg}";
        }

        private void OnCommunTimer(object sender, ElapsedEventArgs e)
        {
            if ((Environment.TickCount - LastCommTick) > MaxCommInterval)
            {
                LastCommTick = Environment.TickCount;
                _logger.Info($"Ebest keep alive");
                KeepAlive();
            }
        }

        public void KeepAlive()
        {
            try
            {
                var query = new EbestQuery<t1102InBlock, t1102OutBlock>();
                if (query.ExecuteQuery(new t1102InBlock { shcode = "000020" }) == false)
                    _logger.Error($"Keep alive error, {GetLastErrorMessage(query.Result)}");
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        public void SendMessage(MessageTypes type, string message)
        {
            if (type == MessageTypes.CloseClient)
            {
                Logout();

                Task.Run(() =>
                {
                    _logger.Info("Process will be closed");
                    Thread.Sleep(1000 * 5);

                    Environment.Exit(0);
                });
            }
        }

        public void RegisterTraderContract(Guid clientId, TraderContract contract)
        {
            try
            {
                if (TraderContracts.ContainsKey(clientId) == true)
                {
                    _logger.Error($"Contract exist {clientId}");
                }
                else
                {
                    contract.Callback = OperationContext.Current.GetCallbackChannel<IRealTimeTraderCallback>();
                    TraderContracts.TryAdd(clientId, contract);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        public void UnregisterTraderContract(Guid clientId)
        {
            try
            {
                if (TraderContracts.TryRemove(clientId, out var temp) == true)
                    _logger.Info($"{clientId} contract unregistered");
                else
                    _logger.Warn($"{clientId} contract not exist");
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private void NotifyState(TraderStateTypes state, string message = "")
        {
            StateNotified?.Invoke(state, message);
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
