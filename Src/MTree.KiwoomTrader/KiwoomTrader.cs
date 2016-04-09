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
	[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public partial class KiwoomTrader : ITrader
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private AxKHOpenAPILib.AxKHOpenAPI kiwoomObj;

        private int _scrNum = 5000;

        private int WaitLoginTimeout { get; } = 1000 * 15;

        private ManualResetEvent WaitLoginEvent { get; } = new ManualResetEvent(false);

        public KiwoomTrader(AxKHOpenAPILib.AxKHOpenAPI axKHOpenAPI)
        {
            try
            {
                kiwoomObj = axKHOpenAPI;
                kiwoomObj.OnEventConnect += OnEventConnect;
                kiwoomObj.OnReceiveTrData += OnReceiveTrData;

                Login();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }


        #region Session
        protected bool WaitLogin()
        {
            return WaitLoginEvent.WaitOne(WaitLoginTimeout);
        }

        protected void SetLogin()
        {
            WaitLoginEvent.Set();
        }

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
                }
                else
                {
                    logger.Error($"Login fail, return code: {e.nErrCode}. Message: {ErrorMessageUtility.GetErrorMessage(e.nErrCode)}");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                ClosePopup();
                SetLogin();
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
        
        // 화면번호 생산
        private string GetScrNum()
        {
            if (_scrNum < 9999)
                _scrNum++;
            else
                _scrNum = 5000;

            return _scrNum.ToString();
        }

        private void OnReceiveTrData(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveTrDataEvent e)
        {
        }
    }
}
