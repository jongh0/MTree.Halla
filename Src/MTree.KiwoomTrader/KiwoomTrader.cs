using MTree.Trader;
using MTree.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTree.KiwoomTrader
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

    public class KiwoomTrader: ITrader
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

        public List<string> GetAccounts()
        {
            List<string> accounts = new List<string>();
            if (WaitLogin() == true)
            {
                foreach (string acc in kiwoomObj.GetLoginInfo("ACCNO").Split(';'))
                {
                    if (acc != string.Empty)
                    {
                        accounts.Add(acc);
                    }
                }
            }
            return accounts;
        }

        public int GetDeposit(string account)
        {
            throw new NotImplementedException();
        }

        public OrderResult Order(Order order)
        {
            throw new NotImplementedException();
        }

        public List<HoldingStock> GetHoldingStocks(string account)
        {
            throw new NotImplementedException();
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
                    logger.Error($"Login fail, return code: {e.nErrCode}. Message:{GetErrorMessage(e.nErrCode)}");
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

        private string GetErrorMessage(int errorCode)
        {
            switch ((KOAErrorCode)errorCode)
            {
                case KOAErrorCode.OP_ERR_NONE:
                    return "정상처리";
                case KOAErrorCode.OP_ERR_LOGIN:
                    return "사용자정보교환에 실패하였습니다. 잠시 후 다시 시작하여 주십시오.";
                case KOAErrorCode.OP_ERR_CONNECT:
                    return "서버 접속 실패";
                case KOAErrorCode.OP_ERR_VERSION:
                    return "버전처리가 실패하였습니다";
                case KOAErrorCode.OP_ERR_SISE_OVERFLOW:
                    return "시세조회 과부하";
                case KOAErrorCode.OP_ERR_RQ_STRUCT_FAIL:
                    return "REQUEST_INPUT_st Failed";
                case KOAErrorCode.OP_ERR_RQ_STRING_FAIL:
                    return "요청 전문 작성 실패";
                case KOAErrorCode.OP_ERR_ORD_WRONG_INPUT:
                    return "주문 입력값 오류";
                case KOAErrorCode.OP_ERR_ORD_WRONG_ACCNO:
                    return "계좌비밀번호를 입력하십시오.";
                case KOAErrorCode.OP_ERR_OTHER_ACC_USE:
                    return "타인계좌는 사용할 수 없습니다.";
                case KOAErrorCode.OP_ERR_MIS_2BILL_EXC:
                    return "주문가격이 20억원을 초과합니다.";
                case KOAErrorCode.OP_ERR_MIS_5BILL_EXC:
                    return "주문가격은 50억원을 초과할 수 없습니다.";
                case KOAErrorCode.OP_ERR_MIS_1PER_EXC:
                    return "주문수량이 총발행주수의 1%를 초과합니다.";
                case KOAErrorCode.OP_ERR_MID_3PER_EXC:
                    return "주문수량은 총발행주수의 3%를 초과할 수 없습니다";
                default:
                    return "알려지지 않은 오류입니다.";
            }
        }

        private void OnReceiveTrData(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveTrDataEvent e)
        {
        }
    }
}
