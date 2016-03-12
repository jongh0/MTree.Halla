using MTree.DataStructure;
using MTree.Publisher;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTree.KiwoomPublisher
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class KiwoomPublisher : BrokerageFirmBase
    {
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
                    QuotingStockMaster.Asset = Convert.ToInt64(kiwoomObj.CommGetData(e.sTrCode, "", e.sRQName, 0, "자본금").Trim());
                    QuotingStockMaster.ShareVolume = Convert.ToInt64(kiwoomObj.CommGetData(e.sTrCode, "", e.sRQName, 0, "상장주식").Trim());

                    //double BPS { get { return Asset / ShareVolume; } }
                    //double PBR { get { return BasisPrice / BPS; } }
                    //double EPS { get { return NetIncome / ShareVolume; } }
                    //double PER { get { return BasisPrice / EPS; } }

                    double bps = Convert.ToDouble(kiwoomObj.CommGetData(e.sTrCode, "", e.sRQName, 0, "BPS").Trim());
                    double pbr = Convert.ToDouble(kiwoomObj.CommGetData(e.sTrCode, "", e.sRQName, 0, "PBR").Trim());

                    Debugger.Break();
                    //logger.Trace(kiwoomObj.CommGetData(e.sTrCode, "", e.sRQName, i, "등락율"));
                    //logger.Trace(kiwoomObj.CommGetData(e.sTrCode, "", e.sRQName, i, "거래량"));
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
