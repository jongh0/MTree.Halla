using MTree.DataStructure;
using MTree.Publisher;
using MTree.Utility;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using MTree.RealTimeProvider;

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
    public partial class KiwoomPublisher : BrokerageFirmBase
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

        public override Dictionary<string, CodeEntity> GetCodeList()
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

        public override void NotifyMessage(MessageTypes type, string message)
        {
            if (type == MessageTypes.CloseClient)
            {
                Logout();

                Task.Run(() =>
                {
                    logger.Info("Process will be closed");
                    Thread.Sleep(1000 * 10);

                    Environment.Exit(0);
                });
            }

            base.NotifyMessage(type, message);
        }
    }
}
