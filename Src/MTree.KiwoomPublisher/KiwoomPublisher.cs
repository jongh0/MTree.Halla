using MTree.DataStructure;
using MTree.Publisher;
using MTree.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

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
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        private AxKHOpenAPILib.AxKHOpenAPI kiwoomObj;

        private int _scrNum = 5000;

        public KiwoomPublisher(AxKHOpenAPILib.AxKHOpenAPI axKHOpenAPI) : base()
        {
            try
            {
                QuoteInterval = 1000 / 5;

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
                LoginInstance.State = LoginStates.Logout;
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
            LastFirmCommunicateTick = Environment.TickCount;

            try
            {
                if (e.nErrCode == 0)
                {
                    logger.Info("Login sucess");
                    LoginInstance.State = LoginStates.Login;
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

                Task.Run(() =>
                {
                    Thread.Sleep(5000); // Login 시간이 오래걸려서 대기
                    SetLogin();
                });
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
            if (Monitor.TryEnter(QuoteLock, QuoteLockTimeout) == false)
            {
                logger.Error($"Quoting failed, Code: {code}, Can't obtaion lock object");
                return false;
            }

            int ret = -1;

            try
            {
                if (WaitLogin() == false)
                {
                    logger.Error($"Quoting failed, Code: {code}, Not loggedin state");
                    return false;
                }

                WaitQuoteInterval();

                logger.Info($"Start quoting, Code: {code}");
                QuotingStockMaster = stockMaster;

                kiwoomObj.SetInputValue("종목코드", code);

                ret = kiwoomObj.CommRqData("주식기본정보", "OPT10001", 0, GetScrNum());
                
                if (ret == 0)
                {
                    if (WaitQuoting() == true)
                    {
                        if (QuotingStockMaster.Code != string.Empty)
                        {
                            logger.Info($"Quoting done. Code: {code}");
                            return true;
                        }

                        logger.Error($"Quoting fail. Code: {code}");
                    }

                    logger.Error($"Quoting timeout. Code: {code}");
                }
                else
                {
                    logger.Error($"Quoting request fail. Code: {code}, Quoting result: {ret}. Message:{GetErrorMessage(ret)}");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                QuotingStockMaster = null;
                Monitor.Exit(QuoteLock);
            }

            return false;
        }

        private void OnReceiveTrData(object sender, AxKHOpenAPILib._DKHOpenAPIEvents_OnReceiveTrDataEvent e)
        {
            LastFirmCommunicateTick = Environment.TickCount;

            // OPT1001 : 주식기본정보
            if (e?.sRQName == "주식기본정보")
            {
                try
                {
                    int nCnt = kiwoomObj.GetRepeatCnt(e.sTrCode, e.sRQName);
                    if (nCnt != 1)
                    {
                        logger.Error("Multiple response received for single request");
                        QuotingStockMaster.Code = string.Empty;
                        return;
                    }

                    string rxCode = kiwoomObj.CommGetData(e.sTrCode, "", e.sRQName, 0, "종목코드").Trim();
                    if (QuotingStockMaster.Code != rxCode)
                    {
                        logger.Error($"Received code({rxCode}) is different from requested({QuotingStockMaster.Code})");
                        QuotingStockMaster.Code = string.Empty;
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
                }
                catch (Exception ex)
                {
                    QuotingStockMaster.Code = string.Empty;
                    logger.Error(ex);
                }
                finally
                {
                    SetQuoting();
                }
            }
        }

        public override StockMaster GetStockMaster(string code)
        {
            try
            {
                var stockMaster = new StockMaster();
                stockMaster.Code = code;

                if (GetQuote(code, ref stockMaster) == false)
                    stockMaster.Code = string.Empty;

                return stockMaster;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return null;
            }
        }

        public override Dictionary<string, CodeEntity> GetStockCodeList()
        {
            if (WaitLogin() == false)
                logger.Error("Session is not established");

            var codeDictionary = new Dictionary<string, CodeEntity>();

            try
            {
                #region ETF(belongs to KOSPI)
                string[] etfList = kiwoomObj.GetCodeListByMarket("8").Split(';');
                foreach (string code in etfList)
                {
                    if (code != string.Empty)
                    {
                        var codeEntity = new CodeEntity();
                        codeEntity.Code = code;
                        codeEntity.Name = kiwoomObj.GetMasterCodeName(code);
                        codeEntity.MarketType = MarketTypes.ETF;

                        if (!codeDictionary.ContainsKey(code))
                            codeDictionary.Add(codeEntity.Code, codeEntity);
                        else
                            logger.Trace("Code is already in the list");
                    }
                }
                #endregion

                #region KOSPI & ETN
                string[] kospiList = kiwoomObj.GetCodeListByMarket("0").Split(';');
                foreach (string code in kospiList)
                {
                    if (code != string.Empty)
                    {
                        var codeEntity = new CodeEntity();
                        codeEntity.Code = code;
                        codeEntity.Name = kiwoomObj.GetMasterCodeName(code);

                        if (code[0] == '5')
                            codeEntity.MarketType = MarketTypes.ETN;
                        else
                            codeEntity.MarketType = MarketTypes.KOSPI;

                        if (!codeDictionary.ContainsKey(code))
                        {
                            codeDictionary.Add(codeEntity.Code, codeEntity);
                        }
                        else
                        {
                            if (codeDictionary[code].MarketType != MarketTypes.ETF)
                                logger.Trace("Code is already in the list");
                        }
                    }
                }
                #endregion

                #region ELW
                string[] elwList = kiwoomObj.GetCodeListByMarket("3").Split(';');
                foreach (string code in elwList)
                {
                    if (code != string.Empty)
                    {
                        var codeEntity = new CodeEntity();
                        codeEntity.Code = code;
                        codeEntity.Name = kiwoomObj.GetMasterCodeName(code);
                        codeEntity.MarketType = MarketTypes.ELW;

                        if (!codeDictionary.ContainsKey(code))
                            codeDictionary.Add(codeEntity.Code, codeEntity);
                        else
                            logger.Trace("Code is already in the list");
                    }

                }
                #endregion

                #region KOSDAQ
                string[] kosdaqList = kiwoomObj.GetCodeListByMarket("10").Split(';');
                foreach (string code in kosdaqList)
                {
                    if (code != string.Empty)
                    {
                        var codeEntity = new CodeEntity();
                        codeEntity.Code = code;
                        codeEntity.Name = kiwoomObj.GetMasterCodeName(code);
                        codeEntity.MarketType = MarketTypes.KOSDAQ;
                        if (!codeDictionary.ContainsKey(code))
                            codeDictionary.Add(codeEntity.Code, codeEntity);
                        else
                            logger.Trace("Code is already in the list");
                    }
                }
                #endregion
                
                #region KONEX
                string[] konexList = kiwoomObj.GetCodeListByMarket("50").Split(';');
                foreach (string code in konexList)
                {
                    if (code != string.Empty)
                    {
                        var codeEntity = new CodeEntity();
                        codeEntity.Code = code;
                        codeEntity.Name = kiwoomObj.GetMasterCodeName(code);
                        codeEntity.MarketType = MarketTypes.KONEX;
                        if (!codeDictionary.ContainsKey(code))
                            codeDictionary.Add(codeEntity.Code, codeEntity);
                        else
                            logger.Trace("Code is already in the list");
                    }
                }
                #endregion
                
                #region Freeboard (K-OTC)
                string[] freeboard = kiwoomObj.GetCodeListByMarket("30").Split(';');
                foreach (string code in freeboard)
                {
                    if (code != string.Empty)
                    {
                        var codeEntity = new CodeEntity();
                        codeEntity.Code = code;
                        codeEntity.Name = kiwoomObj.GetMasterCodeName(code);
                        codeEntity.MarketType = MarketTypes.FREEBOARD;
                        if (!codeDictionary.ContainsKey(code))
                            codeDictionary.Add(codeEntity.Code, codeEntity);
                        else
                            logger.Trace("Code is already in the list");
                    }

                }
                #endregion  


                logger.Info($"Stock code list query done, Count: {codeDictionary.Count}");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            return codeDictionary;
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
    }
}
