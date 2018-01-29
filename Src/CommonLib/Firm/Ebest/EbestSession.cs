using CommonLib.Firm.Ebest.Block;
using CommonLib.Firm.Ebest.Query;
using CommonLib.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XA_SESSIONLib;

namespace CommonLib.Firm.Ebest
{
    /// <summary>
    /// Ebest Login 처리
    /// 일정 시간 이상 통신이 없을 경우 Quote 수행해서 연결을 유지한다.
    /// </summary>
    public class EbestSession
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private const int MaxCommInterval = 1000 * 60 * 20; // 통신 안한지 20분 넘어가면 Quote 시작
        private const int CommTimerInterval = 1000 * 60 * 2; // 2분마다 체크
        private const int WaitLoginTimeout = 10000;

        public int LastCommTick { get; set; }

        private System.Timers.Timer _commTimer { get; set; }

        public XASessionClass Session { get; private set; }
        public LoginInformation LoginInfo { get; private set; }

        #region Event
        public event Action Logined;
        public event Action Logouted;
        public event Action Disconnected; 
        #endregion

        public EbestSession()
        {
            try
            {
                Session = new XASessionClass();
                Session.Disconnect += Session_Disconnect;
                Session._IXASessionEvents_Event_Login += Session__IXASessionEvents_Event_Login;
                Session._IXASessionEvents_Event_Logout += Session__IXASessionEvents_Event_Logout;

                _commTimer = new System.Timers.Timer(CommTimerInterval);
                _commTimer.Elapsed += CommTimer_Elapsed;
                _commTimer.AutoReset = true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        /// <summary>
        /// 서버에 접속 후 Login한다.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool Login(LoginInformation info)
        {
            try
            {
                LoginInfo = info ?? throw new ArgumentNullException(nameof(info));

                _logger.Info($"Login, Id: {LoginInfo.UserId}");

                if (Session.IsConnected() == false &&
                    Session.ConnectServer(LoginInfo.ServerAddress, LoginInfo.ServerPort) == false)
                {
                    _logger.Error($"Server connection fail, {GetLastErrorMessage()}");
                    return false;
                }

                if (LoginInfo.State != LoginStates.Login && 
                    Session.Login(LoginInfo.UserId, LoginInfo.UserPassword, LoginInfo.CertPassword, (int)LoginInfo.ServerType, true) == false)
                {
                    _logger.Error($"Login error, {GetLastErrorMessage()}");
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return false;
        }

        /// <summary>
        /// Logout 후 서버와 연결을 끊는다.
        /// </summary>
        /// <returns></returns>
        public bool Logout()
        {
            try
            {
                _logger.Info("Logout");

                _commTimer.Stop();
                Session.Logout();
                Session.DisconnectServer();
                return true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return false;
        }

        /// <summary>
        /// Login이 완료될 때 까지 대기한다.
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public bool WaitLogin(int timeout = WaitLoginTimeout)
        {
            if (LoginInfo == null) return false;

            while (LoginInfo.State != LoginStates.Login)
            {
                if (timeout <= 0) return false;

                DispatcherUtility.DoEvents();

                timeout -= 10;
                Thread.Sleep(10);
            }

            return true;
        }

        private void CommTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if ((Environment.TickCount - LastCommTick) > MaxCommInterval)
            {
                _logger.Info($"Ebest keep alive");
                KeepAlive();
            }
        }

        private void KeepAlive()
        {
            try
            {
                LastCommTick = Environment.TickCount;

                var query = new EbestQuery<t1102InBlock, t1102OutBlock>();
                if (query.ExecuteQuery(new t1102InBlock { shcode = "000020" }) == false)
                    _logger.Error($"Keep alive error, {GetLastErrorMessage(query.Result)}");
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        public string GetLastErrorMessage(int errCode = 0)
        {
            try
            {
                if (errCode == 0)
                    errCode = Session.GetLastError();

                if (errCode >= 0) return string.Empty;

                var errMsg = Session.GetErrorMessage(errCode);

                return $"{nameof(errCode)}: {errCode}, {nameof(errMsg)}: {errMsg}";
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return $"{nameof(errCode)}: {errCode}";
        }

        /// <summary>
        /// 계좌가 몇 개 있는지 조회한다.
        /// </summary>
        /// <returns></returns>
        public int GetAccountListCount()
        {
            if (Session.IsConnected() == false)
                return 0;

            return Session.GetAccountListCount();
        }

        /// <summary>
        /// 계좌번호를 조회한다.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string GetAccountList(int index)
        {
            if (Session.IsConnected() == false)
                return string.Empty;

            return Session.GetAccountList(index);
        }

        private void Session__IXASessionEvents_Event_Logout()
        {
            LoginInfo.State = LoginStates.Logout;
            Logouted?.Invoke();
        }

        private void Session__IXASessionEvents_Event_Login(string szCode, string szMsg)
        {
            _logger.Info($"{nameof(szCode)}: {szCode}, {nameof(szMsg)}: {szMsg}");

            LoginInfo.State = LoginStates.Login;
            _commTimer.Start();
            Logined?.Invoke();
        }

        private void Session_Disconnect()
        {
            LoginInfo.State = LoginStates.Disconnect;
            _commTimer.Stop();
            Disconnected?.Invoke();
        }
    }
}
