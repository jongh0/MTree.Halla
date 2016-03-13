using MTree.DataStructure;
using MTree.Publisher;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTree.KiwoomPublisher
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class KiwoomPublisher : BrokerageFirmBase
    {
        #region Dll Import
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string strClassName, string strWindowName);  //1. 찾고자하는 클래스이름, 2.캡션값

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr hWnd1, IntPtr hWnd2, string Ipsz1, string Ipsz2);    //1.바로위의 부모값을 주고 2. 0이나 null 3,4.클래스명과 캡션명을 
        
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);
        
        const int BM_CLICK = 0X00F5;
        #endregion

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private AxKHOpenAPILib.AxKHOpenAPI kiwoomObj;

        private object lockObject = new object();
        private bool requestResult = false;

        private int _scrNum = 5000;

        private ManualResetEvent waitLoginEvent = new ManualResetEvent(false);

        public KiwoomPublisher(AxKHOpenAPILib.AxKHOpenAPI axKHOpenAPI) : base()
        {
            kiwoomObj = axKHOpenAPI;
            kiwoomObj.OnEventConnect += OnEventConnect;
            kiwoomObj.OnReceiveTrData += OnReceiveTrData;

            Login();
        }

        #region Session
        public bool WaitLogin()
        {
            return waitLoginEvent.WaitOne(10000);
        }

        public bool Login()
        {
            bool ret = false;

            waitLoginEvent.Reset();
            try
            {
                if (kiwoomObj.CommConnect() == 0)
                {
                    logger.Info("Login window open success");
                    ret = true;
                }
                else
                {
                    logger.Error("Login window open fail");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return false;
            }

            return ret;
        }

        public bool Logout()
        {
            try
            {
                kiwoomObj.CommTerminate();
                LoginInstance.State = StateType.Logout;
                logger.Info("Logout success");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return false;
            }

            return true;
        }

        private void OnEventConnect(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnEventConnectEvent e)
        {
            if (e.nErrCode == 0)
            {
                logger.Info("Login sucess");
                LoginInstance.State = StateType.Login;
            }
            else
            {
                logger.Error($"Login fail, return code: {e.nErrCode}");
            }

            waitLoginEvent.Set();
            ClosePopup();
        }

        private bool ClosePopup()
        {
            try
            {
                IntPtr windowH = FindWindow(null, "khopenapi");

                if (windowH != IntPtr.Zero)
                {
                    logger.Info($"khopenapi popup found");

                    IntPtr buttonH = FindWindowEx(windowH, IntPtr.Zero, "Button", "확인");
                    if (buttonH != IntPtr.Zero)
                    {
                        logger.Info($"확인 button clicked");
                        SendMessage(buttonH, BM_CLICK, 0, 0);

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return false;
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

        public bool GetQuote(string code, ref StockMaster stockMaster)
        {
            if (Monitor.TryEnter(lockObject, 1000 * 10) == false)
            {
                logger.Error($"Quoting failed, Code: {code}, Can't obtaion lock object");
                return false;
            }

            if (waitLoginEvent.WaitOne(10000) == false)
            {
                logger.Error($"Quoting failed, Code: {code}, Not loggedin state");
                return false;
            }

            logger.Info($"Start quoting, Code: {code}");
            requestResult = false;
            int ret = -1;

            try
            {
                QuotingStockMaster = stockMaster;
                QuotingStockMaster.Code = code;

                kiwoomObj.SetInputValue("종목코드", code);

                ret = kiwoomObj.CommRqData("주식기본정보", "OPT10001", 0, GetScrNum());
                

                if (ret == 0)
                {
                    if (WaitQuotingEvent.WaitOne(1000 * 10) == true)
                    {
                        logger.Info($"Quoting done. Code: {code}");
                    }
                    else
                    {
                        logger.Error($"Quoting timeout. Code: {code}");
                        ret = -1;
                    }
                }
                else
                {
                    logger.Error($"Quoting request failed. Code: {code}, Quoting result: {ret}");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                QuotingStockMaster = null;
                Monitor.Exit(lockObject);
            }

            return (ret > 0 && requestResult == true);
        }

        private void OnReceiveTrData(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveTrDataEvent e)
        {
            // OPT1001 : 주식기본정보
            if (e.sRQName == "주식기본정보")
            {
                try
                {
                    int nCnt = kiwoomObj.GetRepeatCnt(e.sTrCode, e.sRQName);
                    if (nCnt != 1)
                    {
                        logger.Error("Multiple response received for single request");
                        WaitQuotingEvent.Set();
                        return;
                    }
                    string rxCode = kiwoomObj.CommGetData(e.sTrCode, "", e.sRQName, 0, "종목코드").Trim();
                    if (QuotingStockMaster.Code != rxCode)
                    {
                        logger.Error($"Received code({rxCode}) is different from requested({QuotingStockMaster.Code})");
                        requestResult = false;
                        WaitQuotingEvent.Set();
                        return;
                    }

                    QuotingStockMaster.PER = Convert.ToDouble(kiwoomObj.CommGetData(e.sTrCode, "", e.sRQName, 0, "PER").Trim());
                    QuotingStockMaster.EPS= Convert.ToDouble(kiwoomObj.CommGetData(e.sTrCode, "", e.sRQName, 0, "EPS").Trim());
                    QuotingStockMaster.PBR = Convert.ToDouble(kiwoomObj.CommGetData(e.sTrCode, "", e.sRQName, 0, "PBR").Trim());
                    QuotingStockMaster.BPS = Convert.ToDouble(kiwoomObj.CommGetData(e.sTrCode, "", e.sRQName, 0, "BPS").Trim());
                    QuotingStockMaster.ROE = Convert.ToDouble(kiwoomObj.CommGetData(e.sTrCode, "", e.sRQName, 0, "ROE").Trim());
                    QuotingStockMaster.EV = Convert.ToDouble(kiwoomObj.CommGetData(e.sTrCode, "", e.sRQName, 0, "EV").Trim());

                }
                catch (Exception ex)
                {
                    requestResult = false;
                    QuotingStockMaster.Code = string.Empty;
                    logger.Error(ex);
                }
                finally
                {
                    WaitQuotingEvent.Set();
                }
            }

        }

        protected override void ServiceClient_Opened(object sender, EventArgs e)
        {
            // Login이 완료된 후에 Publisher contract 등록
            WaitLogin();
            base.ServiceClient_Opened(sender, e);
        }

        public override void CloseClient()
        {
            // Logout한 이후 Process 종료시킨다
            Logout();
            base.CloseClient();
        }

        public override bool IsSubscribable()
        {
            return false;
        }
    }
}
