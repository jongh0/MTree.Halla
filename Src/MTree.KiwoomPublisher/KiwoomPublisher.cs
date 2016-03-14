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
    enum KOAErrorCode
    {
        OP_ERR_NONE = 0,     //"정상처리"
        OP_ERR_LOGIN = -100,  //"사용자정보교환에 실패하였습니다. 잠시후 다시 시작하여 주십시오."
        OP_ERR_CONNECT = -101,  //"서버 접속 실패"
        OP_ERR_VERSION = -102,  //"버전처리가 실패하였습니다.
        OP_ERR_SISE_OVERFLOW = -200,  //”시세조회 과부하”
        OP_ERR_RQ_STRUCT_FAIL = -201,  //”REQUEST_INPUT_st Failed”
        OP_ERR_RQ_STRING_FAIL = -202,  //”요청 전문 작성 실패”
        OP_ERR_ORD_WRONG_INPUT = -300,  //”주문 입력값 오류”
        OP_ERR_ORD_WRONG_ACCNO = -301,  //”계좌비밀번호를 입력하십시오.”
        OP_ERR_OTHER_ACC_USE = -302,  //”타인계좌는 사용할 수 없습니다.
        OP_ERR_MIS_2BILL_EXC = -303,  //”주문가격이 20억원을 초과합니다.”
        OP_ERR_MIS_5BILL_EXC = -304,  //”주문가격은 50억원을 초과할 수 없습니다.”
        OP_ERR_MIS_1PER_EXC = -305,  //”주문수량이 총발행주수의 1%를 초과합니다.”
        OP_ERR_MID_3PER_EXC = -306,  //”주문수량은 총발행주수의 3%를 초과할 수 없습니다.”
    }

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

        private int StockQuoteInterval { get; set; } = 0;

        public KiwoomPublisher(AxKHOpenAPILib.AxKHOpenAPI axKHOpenAPI) : base()
        {
            StockQuoteInterval = 1000 / 5;

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
                logger.Error($"Login fail, return code: {e.nErrCode}. Message:{GetErrorMessage(e.nErrCode)}");
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

        private string GetErrorMessage(int errorCode)
        {
            string errorMessage = "";
            
            switch ((KOAErrorCode)errorCode)
            {

                case KOAErrorCode.OP_ERR_NONE:
                    errorMessage = "정상처리";
                    break;
                case KOAErrorCode.OP_ERR_LOGIN:
                    errorMessage = "사용자정보교환에 실패하였습니다. 잠시 후 다시 시작하여 주십시오.";
                    break;
                case KOAErrorCode.OP_ERR_CONNECT:
                    errorMessage = "서버 접속 실패";
                    break;
                case KOAErrorCode.OP_ERR_VERSION:
                    errorMessage = "버전처리가 실패하였습니다";
                    break;
                case KOAErrorCode.OP_ERR_SISE_OVERFLOW:
                    errorMessage = "시세조회 과부하";
                    break;
                case KOAErrorCode.OP_ERR_RQ_STRUCT_FAIL:
                    errorMessage = "REQUEST_INPUT_st Failed";
                    break;
                case KOAErrorCode.OP_ERR_RQ_STRING_FAIL:
                    errorMessage = "요청 전문 작성 실패";
                    break;
                case KOAErrorCode.OP_ERR_ORD_WRONG_INPUT:
                    errorMessage = "주문 입력값 오류";
                    break;
                case KOAErrorCode.OP_ERR_ORD_WRONG_ACCNO:
                    errorMessage = "계좌비밀번호를 입력하십시오.";
                    break;
                case KOAErrorCode.OP_ERR_OTHER_ACC_USE:
                    errorMessage = "타인계좌는 사용할 수 없습니다.";
                    break;
                case KOAErrorCode.OP_ERR_MIS_2BILL_EXC:
                    errorMessage = "주문가격이 20억원을 초과합니다.";
                    break;
                case KOAErrorCode.OP_ERR_MIS_5BILL_EXC:
                    errorMessage = "주문가격은 50억원을 초과할 수 없습니다.";
                    break;
                case KOAErrorCode.OP_ERR_MIS_1PER_EXC:
                    errorMessage = "주문수량이 총발행주수의 1%를 초과합니다.";
                    break;
                case KOAErrorCode.OP_ERR_MID_3PER_EXC:
                    errorMessage = "주문수량은 총발행주수의 3%를 초과할 수 없습니다";
                    break;
                default:
                    errorMessage = "알려지지 않은 오류입니다.";
                    break;
            }

            return errorMessage;
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
                WaitQuotingLimit();

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
                    logger.Error($"Quoting request failed. Code: {code}, Quoting result: {ret}. Message:{GetErrorMessage(ret)}");
                    requestResult = false;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                requestResult = false;
            }
            finally
            {
                QuotingStockMaster = null;
                Monitor.Exit(lockObject);
            }

            return (ret == 0 && requestResult == true);
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
                        requestResult = false;
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

                    string roe = kiwoomObj.CommGetData(e.sTrCode, "", e.sRQName, 0, "ROE").Trim();
                    if (roe != string.Empty)
                        QuotingStockMaster.ROE = Convert.ToDouble(roe);

                    string ev = kiwoomObj.CommGetData(e.sTrCode, "", e.sRQName, 0, "EV").Trim();
                    if (ev != string.Empty)
                        QuotingStockMaster.EV = Convert.ToDouble(ev);

                    requestResult = true;
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

        public override StockMaster GetStockMaster(string code)
        {
            var stockMaster = new StockMaster();

            try
            {
                if (GetQuote(code, ref stockMaster) == true)
                    stockMaster.Code = code;
                else
                {
                    stockMaster.Code = "";
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return stockMaster;
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

        private void WaitQuotingLimit()
        {
            if (StockQuoteInterval > 0)
                Thread.Sleep(StockQuoteInterval);
        }
    }
}
