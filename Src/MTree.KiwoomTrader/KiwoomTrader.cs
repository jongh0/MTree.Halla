using MTree.Trader;
using MTree.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTree.KiwoomTrader
{
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Single, ValidateMustUnderstand = false)]
    public partial class KiwoomTrader : ITrader
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private AxKHOpenAPILib.AxKHOpenAPI kiwoomObj;

        public LoginInfo LoginInstance { get; } = new LoginInfo();

        public KiwoomTrader(AxKHOpenAPILib.AxKHOpenAPI axKHOpenAPI)
        {
            try
            {
                kiwoomObj = axKHOpenAPI;
                kiwoomObj.OnEventConnect += OnEventConnect;
                kiwoomObj.OnReceiveTrData += OnReceiveTrData;
                kiwoomObj.OnReceiveChejanData += OnReceiveChejanData;

                Login();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        #region Session
        public bool Login()
        {
            try
            {
                if (kiwoomObj.CommConnect() == 0)
                {
                    logger.Info("Login window open success");
                    return true;
                }

                logger.Error("Login window open fail");
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
                kiwoomObj.CommTerminate();
                LoginInstance.State = LoginStates.LoggedOut;
                logger.Info("Logout success");
                return true;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return false;
        }

        private void OnEventConnect(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnEventConnectEvent e)
        {
            try
            {
                if (e.nErrCode == 0)
                {
                    logger.Info("Login sucess");
                    LoginInstance.State = LoginStates.LoggedIn;
                }
                else
                {
                    logger.Error($"Login fail, {KiwoomError.GetErrorMessage(e.nErrCode)}");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
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
                IntPtr windowH = WindowUtility.FindWindow2("khopenapi");

                if (windowH != IntPtr.Zero)
                {
                    logger.Info($"khopenapi popup found");

                    IntPtr buttonH = WindowUtility.FindWindowEx2(windowH, "Button", "확인");
                    if (buttonH != IntPtr.Zero)
                    {
                        logger.Info($"확인 button clicked");
                        WindowUtility.SendMessage2(buttonH, WindowUtility.BM_CLICK, 0, 0);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
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
    }
}
