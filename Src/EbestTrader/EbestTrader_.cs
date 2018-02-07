using Configuration;
using RealTimeProvider;
using Trader;
using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.ServiceModel;
using System.Threading;
using FirmLib;
using FirmLib.Ebest;
using FirmLib.Ebest.Block;
using FirmLib.Ebest.Real;
using System.Threading.Tasks;
using CommonLib;

namespace EbestTrader
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, ValidateMustUnderstand = false)]
    public partial class EbestTrader_ : IRealTimeTrader, ITrader, INotifyPropertyChanged
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private LoginInformation _loginInfo;

        private ConcurrentDictionary<Guid, TraderContract> TraderContracts { get; } = new ConcurrentDictionary<Guid, TraderContract>();

        #region Event
        public event Action<TraderStateTypes, string> StateNotified;
        #endregion

        #region Ebest Specific
        private EbestSession _session;

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
                #region EbestSession
                _session = new EbestSession();
                _session.Logined += Session_Logined;
                _session.Logouted += Session_Logouted;
                #endregion

                #region EbestReal
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

                #region LoginInformation
                _loginInfo = new LoginInformation();
                _loginInfo.FirmType = FirmTypes.Ebest;
                _loginInfo.UserId = Config.Ebest.UserId;
                _loginInfo.UserPassword = Config.Ebest.UserPw;
                _loginInfo.CertPassword = Config.Ebest.CertPw;
                if (Config.General.TraderType == TraderTypes.EbestSimul)
                {
                    _loginInfo.AccountPassword = "0000";
                    _loginInfo.ServerType = ServerTypes.Simul;
                    _loginInfo.ServerAddress = Config.Ebest.SimulServerAddress;
                }
                else
                {
                    _loginInfo.AccountPassword = Config.Ebest.AccountPw;
                    _loginInfo.ServerType = ServerTypes.Real;
                    _loginInfo.ServerAddress = Config.Ebest.RealServerAddress;
                }
                _loginInfo.ServerPort = Config.Ebest.ServerPort;

                if (_loginInfo.IsValid == false)
                {
                    _logger.Error("Check Ebest configuration");
                    return;
                }
                #endregion

                #region Login
                if (_session.Login(_loginInfo) == false)
                {
                    _logger.Error("Login fail");
                    NotifyState(TraderStateTypes.LoginFail);
                } 
                #endregion
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private void Session_Logouted()
        {
            NotifyState(TraderStateTypes.Logout);
        }

        private void Session_Logined()
        {
            NotifyState(TraderStateTypes.LoginSuccess);
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

        private string GetLastErrorMessage(int errCode = 0)
        {
            return _session.GetLastErrorMessage(errCode);
        }

        public void SendMessage(MessageTypes type, string message)
        {
            _session.LastCommTick = Environment.TickCount;

            if (type == MessageTypes.CloseClient)
            {
                _session.Logout();

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
