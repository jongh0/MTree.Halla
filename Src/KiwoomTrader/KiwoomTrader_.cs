using Configuration;
using RealTimeProvider;
using Trader;
using CommonLib;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommonLib.Firm;
using CommonLib.Firm.Kiwoom;

namespace KiwoomTrader
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Single, ValidateMustUnderstand = false)]
    public partial class KiwoomTrader_ : IRealTimeTrader, INotifyPropertyChanged
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private ConcurrentDictionary<Guid, TraderContract> TraderContracts { get; set; } = new ConcurrentDictionary<Guid, TraderContract>();

        private AxKHOpenAPILib.AxKHOpenAPI _kiwoomObj;

        public LoginInfo LoginInstance { get; } = new LoginInfo();

        public KiwoomTrader_(AxKHOpenAPILib.AxKHOpenAPI axKHOpenAPI)
        {
            try
            {
                _kiwoomObj = axKHOpenAPI;
                _kiwoomObj.OnEventConnect += OnEventConnect;
                _kiwoomObj.OnReceiveTrData += OnReceiveTrData;
                _kiwoomObj.OnReceiveChejanData += OnReceiveChejanData;

                Login();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        #region Session
        public bool Login()
        {
            try
            {
                if (_kiwoomObj.CommConnect() == 0)
                {
                    _logger.Info("Login window open success");

                    if (Config.Kiwoom.UseSessionManager == true)
                    {
                        Task.Run(() =>
                        {
                            Thread.Sleep(3000);
                            ProcessUtility.Start(ProcessTypes.KiwoomSessionManager, Process.GetCurrentProcess().Id.ToString());
                        });
                    }

                    return true;
                }

                _logger.Error("Login window open fail");
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
                _kiwoomObj.CommTerminate();
                LoginInstance.State = LoginStates.LoggedOut;
                _logger.Info("Logout success");
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return false;
        }

        private void OnEventConnect(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnEventConnectEvent e)
        {
            try
            {
                if (e.nErrCode == 0)
                {
                    _logger.Info("Login sucess");
                    LoginInstance.State = LoginStates.LoggedIn;
                }
                else
                {
                    _logger.Error($"Login fail, {KiwoomError.GetErrorMessage(e.nErrCode)}");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            finally
            {
                ClosePopup();
            }
        }

        private bool ClosePopup()
        {
            try
            {
                IntPtr windowH = WindowsAPI.findWindow("khopenapi");

                if (windowH != IntPtr.Zero)
                {
                    _logger.Info($"khopenapi popup found");

                    IntPtr buttonH = WindowsAPI.findWindowEx(windowH, "Button", "확인");
                    if (buttonH != IntPtr.Zero)
                    {
                        _logger.Info($"확인 button clicked");
                        WindowsAPI.sendMessage(buttonH, WindowsAPI.BM_CLICK, 0, 0);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return false;
        }
        #endregion

        private void OnReceiveTrData(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveTrDataEvent e)
        {
            switch (e.sRQName)
            {
                case "주식주문":
                    OrderResultReceived(e);
                    break;
            }
        }

        private void OnReceiveChejanData(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveChejanDataEvent e)
        {
            switch (e.sGubun)
            {
                case "0": // 주문체결통보
                    OrderConclusionReceived(e);
                    break;

                case "1": // 잔고통보
                    AccountDepositReceived(e);
                    break;

                case "3": // 특이신호
                    break;
            }
        }

        public void NotifyMessage(MessageTypes type, string message)
        {
        }

        public void RegisterTraderContract(Guid clientId, TraderContract contract)
        {
        }

        public void UnregisterTraderContract(Guid clientId)
        {
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
