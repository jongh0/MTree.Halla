using MTree.DataStructure;
using MTree.Publisher;
using MTree.Utility;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using MTree.RealTimeProvider;
using System.Diagnostics;
using MTree.Configuration;

namespace MTree.KiwoomPublisher
{
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
#if false 
                // 모의투자 체크박스 잘 안될 때가 있음
                // 공인인증서 비밀번호까지는 들어가지는데 탭버튼 등 다른 입력을 한번 줘야지만 공인인증서 관련 팝업이 안생김
                // 버전처리 실행 시 프로그램이 종료가 되어있어야해서 동작이 꼬임
				Task.Run(() => KiwoomSessionUtility.HandleSession());  
#endif

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
                    Thread.Sleep(1000 * 5);

                    Environment.Exit(0);
                });
            }

            base.NotifyMessage(type, message);
        }
    }
}
