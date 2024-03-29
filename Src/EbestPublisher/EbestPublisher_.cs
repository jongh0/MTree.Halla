﻿using System;
using System.Threading;
using DataStructure;
using Configuration;
using Publisher;
using XA_SESSIONLib;
using XA_DATASETLib;
using System.ServiceModel;
using RealTimeProvider;
using System.Timers;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
using System.ComponentModel;
using CommonLib;
using FirmLib.Ebest;
using FirmLib.Ebest.Block;
using FirmLib.Ebest.Query;
using FirmLib;

namespace EbestPublisher
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public partial class EbestPublisher_ : BrokerageFirmBase, INotifyPropertyChanged
    {
        private NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly string resFilePath = "\\Res";

        #region Keep session
        private int MaxCommInterval => 1000 * 60 * 20; // 통신 안한지 20분 넘어가면 Quote 시작
        private int CommTimerInterval => 1000 * 60 * 2; // 2분마다 체크
        private int LastCommTick { get; set; } = Environment.TickCount;
        private System.Timers.Timer CommTimer { get; set; } 
        #endregion

        #region Ebest Specific
        private XASessionClass sessionObj;
        
        //private XARealClass indexSubscribingObj;
        private XARealClass viSubscribingObj;
        private XARealClass dviSubscribingObj;

        private XAQueryClass indexListObj;
        private XAQueryClass stockListObj;
        private XAQueryClass indexQuotingObj;
        private XAQueryClass stockQuotingObj;

        private XAQueryClass warningObj1;
        private XAQueryClass warningObj2;

        private XAQueryClass themeCodeObj;
        private XAQueryClass themeListObj;
        #endregion

        #region Code list
        private Dictionary<string, string> IndexCodeList { get; set; } = new Dictionary<string, string>();
        private Dictionary<string, string> StockCodeList { get; set; } = new Dictionary<string, string>();
        private Dictionary<string, string> ThemeCodeList { get; set; } = new Dictionary<string, string>();
        private Dictionary<string, object> ThemeList { get; set; } = new Dictionary<string, object>();
        #endregion

        #region Warning
        private Dictionary<string, List<string>> WarningList { get; set; } = new Dictionary<string, List<string>>();
        private string CurrUpdatingWarningType { get; set; }
        #endregion

        public EbestPublisher_()
        {
            try
            {
                Thread.CurrentThread.Priority = ThreadPriority.Highest;
                
                CommTimer = new System.Timers.Timer(CommTimerInterval);
                CommTimer.Elapsed += OnCommunTimer;
                CommTimer.AutoReset = true;

                #region XASession
                sessionObj = new XASessionClass();
                sessionObj.Disconnect += SessionObj_Disconnect;
                sessionObj._IXASessionEvents_Event_Login += SessionObj_Event_Login;
                sessionObj._IXASessionEvents_Event_Logout += SessionObj_Event_Logout;
                #endregion

                #region XAReal
                viSubscribingObj = new XARealClass();
                viSubscribingObj.ReceiveRealData += ViSubscribingObj_ReceiveRealData;
                viSubscribingObj.ResFileName = resFilePath + "\\VI_.res";

                dviSubscribingObj = new XARealClass();
                dviSubscribingObj.ReceiveRealData += DviSubscribingObj_ReceiveRealData;
                dviSubscribingObj.ResFileName = resFilePath + "\\DVI.res";
                #endregion

                #region XAQuery
                indexListObj = new XAQueryClass();
                indexListObj.ResFileName = resFilePath + "\\t8424.res";
                indexListObj.ReceiveChartRealData += QueryObj_ReceiveChartRealData;
                indexListObj.ReceiveData += IndexListObj_ReceiveData;
                indexListObj.ReceiveMessage += QueryObj_ReceiveMessage;

                stockListObj = new XAQueryClass();
                stockListObj.ResFileName = resFilePath + "\\t8430.res";
                stockListObj.ReceiveChartRealData += QueryObj_ReceiveChartRealData;
                stockListObj.ReceiveData += StockListObj_ReceiveData;
                stockListObj.ReceiveMessage += QueryObj_ReceiveMessage;

                indexQuotingObj = new XAQueryClass();
                indexQuotingObj.ResFileName = resFilePath + "\\t1511.res";
                indexQuotingObj.ReceiveChartRealData += QueryObj_ReceiveChartRealData;
                indexQuotingObj.ReceiveData += IndexQuotingObj_ReceiveData;
                indexQuotingObj.ReceiveMessage += QueryObj_ReceiveMessage;

                stockQuotingObj = new XAQueryClass();
                stockQuotingObj.ResFileName = resFilePath + "\\t1102.res";
                stockQuotingObj.ReceiveChartRealData += QueryObj_ReceiveChartRealData;
                stockQuotingObj.ReceiveData += StockQuotingObj_ReceiveData;
                stockQuotingObj.ReceiveMessage += QueryObj_ReceiveMessage;

                warningObj1 = new XAQueryClass();
                warningObj1.ResFileName = resFilePath + "\\t1404.res";
                warningObj1.ReceiveChartRealData += QueryObj_ReceiveChartRealData;
                warningObj1.ReceiveData += WarningObj1_ReceiveData;
                warningObj1.ReceiveMessage += QueryObj_ReceiveMessage;

                warningObj2 = new XAQueryClass();
                warningObj2.ResFileName = resFilePath + "\\t1405.res";
                warningObj2.ReceiveChartRealData += QueryObj_ReceiveChartRealData;
                warningObj2.ReceiveData += WarningObj2_ReceiveData;
                warningObj2.ReceiveMessage += QueryObj_ReceiveMessage;

                themeCodeObj = new XAQueryClass();
                themeCodeObj.ResFileName = resFilePath + "\\t8425.res";
                themeCodeObj.ReceiveChartRealData += QueryObj_ReceiveChartRealData;
                themeCodeObj.ReceiveData += ThemeCode_ReceiveData;
                themeCodeObj.ReceiveMessage += QueryObj_ReceiveMessage;

                themeListObj = new XAQueryClass();
                themeListObj.ResFileName = resFilePath + "\\t1537.res";
                themeListObj.ReceiveChartRealData += QueryObj_ReceiveChartRealData;
                themeListObj.ReceiveData += ThemeList_ReceiveData;
                themeListObj.ReceiveMessage += QueryObj_ReceiveMessage;

                #endregion

                #region Login
                LoginInfo.FirmType = FirmTypes.Ebest;
                LoginInfo.UserId = Config.Ebest.UserId;
                LoginInfo.UserPassword = Config.Ebest.UserPw;
                LoginInfo.CertPassword = Config.Ebest.CertPw;
                LoginInfo.AccountPassword = Config.Ebest.AccountPw;
                LoginInfo.ServerType = ServerTypes.Real;
                LoginInfo.ServerAddress = Config.Ebest.RealServerAddress;
                LoginInfo.ServerPort = Config.Ebest.ServerPort;

                if (string.IsNullOrEmpty(LoginInfo.UserId) == false &&
                    string.IsNullOrEmpty(LoginInfo.UserPassword) == false &&
                    string.IsNullOrEmpty(LoginInfo.CertPassword) == false)
                {
                    Login();
                }
                else
                {
                    _logger.Error("Check Ebest configuration");
                    return;
                }
                #endregion

                StartCircuitBreakQueueTask(); 
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        #region XAQuery
        private void QueryObj_ReceiveMessage(bool bIsSystemError, string nMessageCode, string szMessage)
        {
            LastCommTick = Environment.TickCount;

            if (bIsSystemError == true)
                _logger.Error($"bIsSystemError: {bIsSystemError}, nMessageCode: {nMessageCode}, szMessage: {szMessage}");
        }

        private void QueryObj_ReceiveChartRealData(string szTrCode)
        {
            LastCommTick = Environment.TickCount;
            _logger.Trace($"szTrCode: {szTrCode}");
        }
        #endregion

        #region XASession
        private void SessionObj_Event_Logout()
        {
            CommTimer.Stop();
            LoginInfo.Status = LoginStatus.Logout;
            _logger.Info(LoginInfo.ToString());
        }

        private void SessionObj_Event_Login(string szCode, string szMsg)
        {
            if (szCode == "0000")
            {
                LoginInfo.Status = LoginStatus.Login;
                _logger.Info($"Login success, {LoginInfo.ToString()}");
                SetLogin();
            }
            else
            {
                _logger.Error($"Login fail, szCode: {szCode}, szMsg: {szMsg}");
            }
        }

        private void SessionObj_Disconnect()
        {
            CommTimer.Stop();
            LoginInfo.Status = LoginStatus.Disconnect;
            _logger.Error(LoginInfo.ToString());
        }
        #endregion

        #region Login / Logout
        public bool Login()
        {
            try
            {
                if (sessionObj.ConnectServer(LoginInfo.ServerAddress, LoginInfo.ServerPort) == false)
                {
                    _logger.Error($"Server connection fail, {GetLastErrorMessage()}");
                    return false;
                }

                _logger.Info($"Try login, Id: {LoginInfo.UserId}");

                if (sessionObj.Login(LoginInfo.UserId, LoginInfo.UserPassword, LoginInfo.CertPassword, (int)LoginInfo.ServerType, true) == false)
                {
                    _logger.Error($"Login error, {GetLastErrorMessage()}");
                    return false;
                }

                CommTimer.Start();
                return true;
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
                CommTimer.Stop();
                sessionObj.DisconnectServer();
                LoginInfo.Status = LoginStatus.Disconnect;

                _logger.Info("Logout success");
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return false;
        }
        #endregion

        private void UpdateWarningList()
        {
            try
            {
                foreach (WarningTypes1 warningType in Enum.GetValues(typeof(WarningTypes1)))
                {
                    GetWarningList(warningType);
                }

                foreach (WarningTypes2 warningType in Enum.GetValues(typeof(WarningTypes2)))
                {
                    GetWarningList(warningType);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        public void UpdateThemeList()
        {
            var themeList = new Dictionary<string, object>();
            try
            {
                _logger.Info($"Theme code list query start");

                QuoteInterval = 1000 / stockQuotingObj.GetTRCountPerSec("t8425");
                WaitQuoteInterval();
                themeCodeObj.SetFieldData("t8425InBlock", "dummy", 0, "");
                var ret = themeCodeObj.Request(false);
                if (ret < 0)
                {
                    _logger.Error($"Theme code request error, {GetLastErrorMessage(ret)}");
                    return;
                }

                if (WaitQuoting() == false)
                    return;
                _logger.Info($"Theme code list query done");

                QuoteInterval = 1000 / stockQuotingObj.GetTRCountPerSec("t1537") + 100;
                foreach (KeyValuePair<string, string> theme in ThemeCodeList)
                {
                    WaitQuoteInterval();
                    //_logger.Info($"Theme codes for {theme.Value} query start");
                    themeListObj.SetFieldData("t1537InBlock", "tmcode", 0, theme.Key);
                    ret = themeListObj.Request(false);
                    if (ret < 0)
                    {
                        _logger.Error($"Theme list request error, {GetLastErrorMessage(ret)}");
                        return;
                    }
                    if (WaitQuoting() == false)
                    {
                        _logger.Error($"Theme code request error, Quoting timeout");
                        return;
                    }
                    //_logger.Info($"Theme codes for {theme.Value} query done");
                    themeList.Add(theme.Value, ThemeList);
                }
                ThemeList = themeList;
                _logger.Info("Quoting theme list success");
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return;
        }

        public Dictionary<string, string> GetIndexCodeList()
        {
            try
            {
                WaitQuoteInterval();

                indexListObj.SetFieldData("t8424InBlock", "gubun1", 0, "");
                var ret = indexListObj.Request(false);
                if (ret < 0)
                {
                    _logger.Error($"Index code request error, {GetLastErrorMessage(ret)}");
                    return null;
                }

                if (WaitQuoting() == false)
                    return null;

                _logger.Info("Quoting industry list success");
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return IndexCodeList;
        }

        public Dictionary<string, string> GetStockCodeList()
        {
            try
            {
                WaitQuoteInterval();

                stockListObj.SetFieldData("t8430InBlock", "gubun", 0, "0");
                var ret = stockListObj.Request(false);
                if (ret < 0)
                {
                    _logger.Error($"Stock code request error, {GetLastErrorMessage(ret)}");
                    return null;
                }

                if (WaitQuoting() == false)
                    return null;

                _logger.Info("Quoting stock list success");
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return StockCodeList;
        }

        public override Dictionary<string, CodeEntity> GetCodeList()
        {
            var codeList = new Dictionary<string, CodeEntity>();

            try
            {
                foreach (KeyValuePair<string, string> code in GetIndexCodeList())
                {
                    var codeEntity = new CodeEntity();
                    codeEntity.Code = code.Key;
                    codeEntity.Name = code.Value;
                    codeEntity.MarketType = MarketTypes.INDEX;
                    if (codeList.ContainsKey(codeEntity.Code) == false)
                        codeList.Add(codeEntity.Code, codeEntity);
                    else
                        _logger.Error($"{codeEntity.Code} code already exists");

                }

                foreach (KeyValuePair<string, string> code in GetStockCodeList())
                {
                    var codeEntity = new CodeEntity();
                    codeEntity.Code = code.Key;
                    codeEntity.Name = code.Value;
                    codeEntity.MarketType = MarketTypes.Unknown;
                    if (codeList.ContainsKey(codeEntity.Code) == false)
                        codeList.Add(codeEntity.Code, codeEntity);
                    else
                        _logger.Error($"{codeEntity.Code} code already exists");
                }

                _logger.Info($"Code list query done, Count: {codeList.Count}");
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return codeList;
        }

        public override Dictionary<string, object> GetCodeMap(CodeMapTypes codemapType)
        {
            if (codemapType == CodeMapTypes.Theme)
            {
                UpdateThemeList();
                return ThemeList;
            }
            else
            {
                return null;
            }
        }
        
        private void IndexListObj_ReceiveData(string szTrCode)
        {
            try
            {
                LastCommTick = Environment.TickCount;
                _logger.Trace($"szTrCode: {szTrCode}");

                int cnt = indexListObj.GetBlockCount("t8424OutBlock");
                for (int i = 0; i < cnt; i++)
                {
                    IndexCodeList.Add(indexListObj.GetFieldData("t8424OutBlock", "upcode", i), indexListObj.GetFieldData("t8424OutBlock", "hname", i));
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            finally
            {
                SetQuoting();
            }
        }

        private void StockListObj_ReceiveData(string szTrCode)
        {
            try
            {
                LastCommTick = Environment.TickCount;
                _logger.Trace($"szTrCode: {szTrCode}");

                int cnt = stockListObj.GetBlockCount(nameof(t8430OutBlock));

                for (int i = 0; i < cnt; i++)
                {
                    if (stockListObj.GetFieldData(out t8430OutBlock block, i) == true)
                        StockCodeList.Add(block.shcode, block.hname);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            finally
            {
                SetQuoting();
            }
        }

        private bool GetWarningList(WarningTypes1 warningType)
        {
            if (Monitor.TryEnter(QuoteLock, QuoteLockTimeout) == false)
            {
                _logger.Error($"Updating {warningType.ToString()} list fail. Can't obtaion lock object");
                return false;
            }

            try
            {
                QuoteInterval = 1000 / stockQuotingObj.GetTRCountPerSec("t1404");
                WaitQuoteInterval();

                if (WarningList.ContainsKey(warningType.ToString()) == false)
                    WarningList.Add(warningType.ToString(), new List<string>());

                CurrUpdatingWarningType = warningType.ToString();
                warningObj1.SetFieldData("t1404InBlock", "gubun", 0, "0");
                warningObj1.SetFieldData("t1404InBlock", "jongchk", 0, ((int)warningType).ToString());

                var ret = warningObj1.Request(false);
                if (ret < 0)
                {
                    _logger.Error($"Warning1 list request error");
                    return false;
                }

                if (WaitQuoting() == false)
                    return false;

                _logger.Info($"Updating {warningType.ToString()} list done. count: {WarningList[warningType.ToString()].Count}");
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            finally
            {
                CurrUpdatingWarningType = "";
                Monitor.Exit(QuoteLock);
            }

            return true;
        }

        private bool GetWarningList(WarningTypes2 warningType)
        {
            if (Monitor.TryEnter(QuoteLock, QuoteLockTimeout) == false)
            {
                _logger.Error($"Updating {warningType.ToString()} list fail. Can't obtaion lock object");
                return false;
            }

            try
            {
                QuoteInterval = 1000 / stockQuotingObj.GetTRCountPerSec("t1405");
                WaitQuoteInterval();

                if (WarningList.ContainsKey(warningType.ToString()) == false)
                    WarningList.Add(warningType.ToString(), new List<string>());

                CurrUpdatingWarningType = warningType.ToString();
                warningObj2.SetFieldData("t1405InBlock", "gubun", 0, "0");
                warningObj2.SetFieldData("t1405InBlock", "jongchk", 0, ((int)warningType).ToString());

                var ret = warningObj2.Request(false);
                if (ret < 0)
                {
                    _logger.Error($"Warning2 list request error");
                    return false;
                }

                if (WaitQuoting() == false)
                    return false;

                _logger.Info($"Updating {warningType.ToString()} list done. count: {WarningList[warningType.ToString()].Count}");
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            finally
            {
                CurrUpdatingWarningType = "";
                Monitor.Exit(QuoteLock);
            }

            return true;
        }

        private void WarningObj1_ReceiveData(string szTrCode)
        {
            try
            {
                LastCommTick = Environment.TickCount;
                _logger.Trace($"szTrCode: {szTrCode}");

                int cnt = warningObj1.GetBlockCount("t1404OutBlock1");
                for (int i = 0; i < cnt; i++)
                {
                    WarningList[CurrUpdatingWarningType].Add(warningObj1.GetFieldData("t1404OutBlock1", "shcode", i));
                }

                string continueCode = warningObj1.GetFieldData("t1404OutBlock", "cts_shcode", 0);
                if (continueCode != "")
                {
                    warningObj1.SetFieldData("t1404InBlock", "cts_shcode", 0, continueCode);
                    warningObj1.Request(true);
                }
                else
                {
                    SetQuoting();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private void WarningObj2_ReceiveData(string szTrCode)
        {
            try
            {
                LastCommTick = Environment.TickCount;
                _logger.Trace($"szTrCode: {szTrCode}");

                int cnt = warningObj2.GetBlockCount("t1405OutBlock1");
                for (int i = 0; i < cnt; i++)
                {
                    WarningList[CurrUpdatingWarningType].Add(warningObj2.GetFieldData("t1405OutBlock1", "shcode", i));
                }

                string continueCode = warningObj2.GetFieldData("t1405OutBlock", "cts_shcode", 0);
                if (continueCode != "")
                {
                    warningObj2.SetFieldData("t1405InBlock", "cts_shcode", 0, continueCode);
                    warningObj2.Request(true);
                }
                else
                {
                    SetQuoting();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private void ThemeCode_ReceiveData(string szTrCode)
        {
            try
            {
                LastCommTick = Environment.TickCount;
                _logger.Trace($"szTrCode: {szTrCode}");

                int cnt = themeCodeObj.GetBlockCount("t8425OutBlock");
                for (int i = 0; i < cnt; i++)
                {
                    ThemeCodeList.Add(themeCodeObj.GetFieldData("t8425OutBlock", "tmcode", i), themeCodeObj.GetFieldData("t8425OutBlock", "tmname", i));
                }

                SetQuoting();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }
        private void ThemeList_ReceiveData(string szTrCode)
        {
            try
            {
                LastCommTick = Environment.TickCount;
                _logger.Trace($"szTrCode: {szTrCode}");

                int cnt = Convert.ToInt32(themeListObj.GetFieldData("t1537OutBlock", "tmcnt", 0));
                ThemeList = new Dictionary<string, object>();
                for (int i = 0; i < cnt; i++)
                {
                    ThemeList.Add(themeListObj.GetFieldData("t1537OutBlock1", "shcode", i), themeListObj.GetFieldData("t1537OutBlock1", "hname", i));
                }

                SetQuoting();
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private string GetLastErrorMessage(int errCode = 0)
        {
            if (errCode == 0)
                errCode = sessionObj.GetLastError();
            var errMsg = sessionObj.GetErrorMessage(errCode);

            return $"errCode: {errCode}, errMsg: {errMsg}";
        }

        protected override void ServiceClient_Opened(object sender, EventArgs e)
        {
            base.ServiceClient_Opened(sender, e);

            Task.Run(() =>
            {
                // Login이 완료된 후에 Publisher contract 등록
                WaitLogin();

                // Warning List Update
                UpdateWarningList();
                
                // Contract 등록
                RegisterContract();
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

        private void OnCommunTimer(object sender, ElapsedEventArgs e)
        {
            if ((Environment.TickCount - LastCommTick) > MaxCommInterval)
            {
                LastCommTick = Environment.TickCount;
                _logger.Info($"Ebest keep alive");
                GetStockMaster("000020");
            }
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}
