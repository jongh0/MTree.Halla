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

namespace EbestTrader
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Single, ValidateMustUnderstand = false)]
    public partial class EbestTrader_ : ITrader, INotifyPropertyChanged
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private ConcurrentDictionary<Guid, TraderContract> TraderContracts { get; set; } = new ConcurrentDictionary<Guid, TraderContract>();

        private const string ResFilePath = "\\Res";

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

        public EbestTrader_()
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
                stockQuotingObj.ResFileName = ResFilePath + "\\t1102.res";
                stockQuotingObj.ReceiveData += StockQuotingObj_ReceiveData;
                stockQuotingObj.ReceiveMessage += QueryObj_ReceiveMessage;

                newOrderObj = new XAQueryClass();
                newOrderObj.ResFileName = ResFilePath + "\\CSPAT00600.res";
                newOrderObj.ReceiveData += NewOrderObj_ReceiveData;
                newOrderObj.ReceiveMessage += QueryObj_ReceiveMessage;

                modifyOrderObj = new XAQueryClass();
                modifyOrderObj.ResFileName = ResFilePath + "\\CSPAT00700.res";
                modifyOrderObj.ReceiveData += ModifyOrderObj_ReceiveData;
                modifyOrderObj.ReceiveMessage += QueryObj_ReceiveMessage;

                cancelOrderObj = new XAQueryClass();
                cancelOrderObj.ResFileName = ResFilePath + "\\CSPAT00800.res";
                cancelOrderObj.ReceiveData += CancelOrderObj_ReceiveData;
                cancelOrderObj.ReceiveMessage += QueryObj_ReceiveMessage;

                accDepositObj = new XAQueryClass();
                accDepositObj.ResFileName = ResFilePath + "\\t0424.res";
                accDepositObj.ReceiveData += AccDepositObj_ReceiveData;
                accDepositObj.ReceiveMessage += QueryObj_ReceiveMessage;
                #endregion

                #region XAReal
                orderSubmittedObj = new XARealClass();
                orderSubmittedObj.ResFileName = ResFilePath + "\\SC0.res";
                orderSubmittedObj.ReceiveRealData += OrderSubmittedObj_ReceiveRealData;

                orderConcludedObj = new XARealClass();
                orderConcludedObj.ResFileName = ResFilePath + "\\SC1.res";
                orderConcludedObj.ReceiveRealData += OrderConcludedObj_ReceiveRealData;

                orderModifiedObj = new XARealClass();
                orderModifiedObj.ResFileName = ResFilePath + "\\SC2.res";
                orderModifiedObj.ReceiveRealData += OrderModifiedObj_ReceiveRealData;

                orderCanceledObj = new XARealClass();
                orderCanceledObj.ResFileName = ResFilePath + "\\SC3.res";
                orderCanceledObj.ReceiveRealData += OrderCanceledObj_ReceiveRealData;

                orderRejectedObj = new XARealClass();
                orderRejectedObj.ResFileName = ResFilePath + "\\SC4.res";
                orderRejectedObj.ReceiveRealData += OrderRejectedObj_ReceiveRealData;
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
                orderSubmittedObj.AdviseRealData();
                orderConcludedObj.AdviseRealData();
                orderModifiedObj.AdviseRealData();
                orderCanceledObj.AdviseRealData();
                orderRejectedObj.AdviseRealData();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private void QueryObj_ReceiveMessage(bool bIsSystemError, string nMessageCode, string szMessage)
        {
            if (bIsSystemError == true)
                _logger.Error($"{nameof(nMessageCode)}: {nMessageCode}, {nameof(szMessage)}: {szMessage}");
            else
                _logger.Info($"{nameof(nMessageCode)}: {nMessageCode}, {nameof(szMessage)}: {szMessage}");
        }

        #region XASession
        private void SessionObj_Event_Logout()
        {
            CommTimer.Stop();
            LoginInstance.State = LoginStates.LoggedOut;
            _logger.Info(LoginInstance.ToString());
        }

        private void SessionObj_Event_Login(string szCode, string szMsg)
        {
            if (szCode == "0000")
            {
                LoginInstance.State = LoginStates.LoggedIn;
                _logger.Info($"Login success, {LoginInstance.ToString()}");
                SetLogin();

                AdviseRealData();
            }
            else
            {
                _logger.Error($"Login fail, szCode: {szCode}, szMsg: {szMsg}");
            }
        }

        private void SessionObj_Disconnect()
        {
            CommTimer.Stop();
            LoginInstance.State = LoginStates.Disconnect;
            _logger.Error(LoginInstance.ToString());
        }
        #endregion

        #region Login / Logout

        public bool WaitLogin()
        {
            if (WaitLoginEvent.WaitOne(WaitLoginTimeout) == false)
            {
                _logger.Error($"{GetType().Name} wait login timeout");
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
                if (sessionObj.ConnectServer(LoginInstance.ServerAddress, LoginInstance.ServerPort) == false)
                {
                    _logger.Error($"Server connection fail, {GetLastErrorMessage()}");
                    return false;
                }

                _logger.Info($"Try login, Id: {LoginInstance.UserId}");

                if (sessionObj.Login(LoginInstance.UserId, LoginInstance.UserPw, LoginInstance.CertPw, 0, true) == false)
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
                sessionObj.DisconnectServer();
                LoginInstance.State = LoginStates.Disconnect;

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
                errCode = sessionObj.GetLastError();
            var errMsg = sessionObj.GetErrorMessage(errCode);

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
                stockQuotingObj.SetFieldData("t1102InBlock", "shcode", 0, "000020");
                var ret = stockQuotingObj.Request(false);
                if (ret < 0)
                    _logger.Error($"Keep alive error, {GetLastErrorMessage(ret)}");
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private void StockQuotingObj_ReceiveData(string szTrCode)
        {
            LastCommTick = Environment.TickCount;
            _logger.Trace($"szTrCode: {szTrCode}");
        }

        public void NotifyMessage(MessageTypes type, string message)
        {
            try
            {

            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        public void RegisterContract(Guid clientId, TraderContract contract)
        {
            try
            {
                if (TraderContracts.ContainsKey(clientId) == true)
                {
                    _logger.Error($"Contract exist {clientId}");
                }
                else
                {
                    contract.Callback = OperationContext.Current.GetCallbackChannel<ITraderCallback>();
                    TraderContracts.TryAdd(clientId, contract);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        public void UnregisterContract(Guid clientId)
        {
            try
            {
                if (TraderContracts.ContainsKey(clientId) == true)
                {
                    TraderContracts.TryRemove(clientId, out var temp);
                    _logger.Info($"{clientId} contract unregistered");
                }
                else
                {
                    _logger.Warn($"{clientId} contract not exist");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
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
