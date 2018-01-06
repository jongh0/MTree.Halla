using DataStructure;
using Publisher;
using CommonLib;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using RealTimeProvider;
using System.Diagnostics;
using Configuration;

namespace KiwoomPublisher
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public partial class KiwoomPublisher_ : BrokerageFirmBase
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private AxKHOpenAPILib.AxKHOpenAPI _kiwoomObj;

        public KiwoomPublisher_(AxKHOpenAPILib.AxKHOpenAPI axKHOpenAPI)
        {
            try
            {
                QuoteInterval = 1000 / 5;

                _kiwoomObj = axKHOpenAPI;
                _kiwoomObj.OnEventConnect += OnEventConnect;
                _kiwoomObj.OnReceiveTrData += OnReceiveTrData;

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
                            Thread.Sleep(3000); // Wait for server connection
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
                    GetCodeMap(CodeMapTypes.Theme);
                    SetLogin();
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
                case "주식기본정보":
                    StockMasterReceived(e);
                    break;
            }
        }

        public override Dictionary<string, object> GetCodeMap(CodeMapTypes codemapType)
        {
            if (codemapType == CodeMapTypes.Theme)
                return GetThemeCodeMap();
            else
                return null;
        }

        private Dictionary<string, object> GetThemeCodeMap()
        {
            var codeDictionary = new Dictionary<string, object>();
            try
            {
                _logger.Info($"Theme list query start");

                var themeList = _kiwoomObj.GetThemeGroupList(0).Split(';');
                foreach (var theme in themeList)
                {
                    var themeGroup = new Dictionary<string, object>();
                    var themeCode = theme.Split('|')[0];
                    var themeName = theme.Split('|')[1];
                    var codes = _kiwoomObj.GetThemeGroupCode(themeCode).Split(';');
                    foreach (var code in codes)
                    {
                        var shortCode = code.Substring(1);
                        if (themeGroup.ContainsKey(shortCode) == true)
                        {
                            _logger.Warn($"{shortCode} is already exist in {themeCode}.");
                            continue;
                        }
                        themeGroup.Add(shortCode, _kiwoomObj.GetMasterCodeName(shortCode));
                    }

                    if (codeDictionary.ContainsKey(themeName) == true)
                    {
                        _logger.Error($"{themeName} is already exist.");
                        continue;
                    }
                    codeDictionary.Add(themeName, themeGroup);
                }
                _logger.Info($"Theme list query done");
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return codeDictionary;
        }

        public override Dictionary<string, CodeEntity> GetCodeList()
        {
            var codeDictionary = new Dictionary<string, CodeEntity>();

            try
            {
                #region ETF(belongs to KOSPI)
                string[] etfList = _kiwoomObj.GetCodeListByMarket("8").Split(';');
                foreach (string code in etfList)
                {
                    if (code != string.Empty)
                    {
                        var codeEntity = new CodeEntity();
                        codeEntity.Code = code;
                        codeEntity.Name = _kiwoomObj.GetMasterCodeName(code);
                        codeEntity.MarketType = MarketTypes.ETF;

                        if (!codeDictionary.ContainsKey(code))
                            codeDictionary.Add(codeEntity.Code, codeEntity);
                        else
                            _logger.Trace("Code is already in the list");
                    }
                }
                #endregion

                #region KOSPI & ETN
                string[] kospiList = _kiwoomObj.GetCodeListByMarket("0").Split(';');
                foreach (string code in kospiList)
                {
                    if (code != string.Empty)
                    {
                        var codeEntity = new CodeEntity();
                        codeEntity.Code = code;
                        codeEntity.Name = _kiwoomObj.GetMasterCodeName(code);

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
                                _logger.Trace("Code is already in the list");
                        }
                    }
                }
                #endregion

                #region ELW
                string[] elwList = _kiwoomObj.GetCodeListByMarket("3").Split(';');
                foreach (string code in elwList)
                {
                    if (code != string.Empty)
                    {
                        var codeEntity = new CodeEntity();
                        codeEntity.Code = code;
                        codeEntity.Name = _kiwoomObj.GetMasterCodeName(code);
                        codeEntity.MarketType = MarketTypes.ELW;

                        if (!codeDictionary.ContainsKey(code))
                            codeDictionary.Add(codeEntity.Code, codeEntity);
                        else
                            _logger.Trace("Code is already in the list");
                    }

                }
                #endregion

                #region KOSDAQ
                string[] kosdaqList = _kiwoomObj.GetCodeListByMarket("10").Split(';');
                foreach (string code in kosdaqList)
                {
                    if (code != string.Empty)
                    {
                        var codeEntity = new CodeEntity();
                        codeEntity.Code = code;
                        codeEntity.Name = _kiwoomObj.GetMasterCodeName(code);
                        codeEntity.MarketType = MarketTypes.KOSDAQ;
                        if (!codeDictionary.ContainsKey(code))
                            codeDictionary.Add(codeEntity.Code, codeEntity);
                        else
                            _logger.Trace("Code is already in the list");
                    }
                }
                #endregion
                
                #region KONEX
                string[] konexList = _kiwoomObj.GetCodeListByMarket("50").Split(';');
                foreach (string code in konexList)
                {
                    if (code != string.Empty)
                    {
                        var codeEntity = new CodeEntity();
                        codeEntity.Code = code;
                        codeEntity.Name = _kiwoomObj.GetMasterCodeName(code);
                        codeEntity.MarketType = MarketTypes.KONEX;
                        if (!codeDictionary.ContainsKey(code))
                            codeDictionary.Add(codeEntity.Code, codeEntity);
                        else
                            _logger.Trace("Code is already in the list");
                    }
                }
                #endregion
                
                #region Freeboard (K-OTC)
                string[] freeboard = _kiwoomObj.GetCodeListByMarket("30").Split(';');
                foreach (string code in freeboard)
                {
                    if (code != string.Empty)
                    {
                        var codeEntity = new CodeEntity();
                        codeEntity.Code = code;
                        codeEntity.Name = _kiwoomObj.GetMasterCodeName(code);
                        codeEntity.MarketType = MarketTypes.FREEBOARD;
                        if (!codeDictionary.ContainsKey(code))
                            codeDictionary.Add(codeEntity.Code, codeEntity);
                        else
                            _logger.Trace("Code is already in the list");
                    }

                }
                #endregion  


                _logger.Info($"Stock code list query done, Count: {codeDictionary.Count}");
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return codeDictionary;
        }

        protected override void ServiceClient_Opened(object sender, EventArgs e)
        {
            base.ServiceClient_Opened(sender, e);

            Task.Run(() =>
            {
                // Login이 완료된 후에 Publisher contract 등록
                if (WaitLogin() == true)
                    // Contract 등록
                    RegisterPublishContract();
                else
                {
                    _logger.Error("Login Fail");

                    ProcessUtility.Kill("khministarter");
                    _logger.Error("Restart Kiwoom Publisher");
                    Process.Start(System.Windows.Forms.Application.ExecutablePath);
                    Environment.Exit(-1);
                }
            });
        }

        public override void NotifyMessage(MessageTypes type, string message)
        {
            if (type == MessageTypes.CloseClient)
            {
                Logout();

                Task.Run(() =>
                {
                    _logger.Info("Process will be closed");
                    Thread.Sleep(1000 * 5);

                    Environment.Exit(0);
                });
            }

            base.NotifyMessage(type, message);
        }
    }
}
