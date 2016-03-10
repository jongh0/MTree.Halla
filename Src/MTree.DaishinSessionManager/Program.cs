using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;
using MTree.Configuration;
using MTree.Utility;

namespace MTree.DaishinSessionManager
{
    class Program
    {
        #region Dll Import
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string strClassName, string strWindowName);  //1. 찾고자하는 클래스이름, 2.캡션값

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr hWnd1, IntPtr hWnd2, string Ipsz1, string Ipsz2);    //1.바로위의 부모값을 주고 2. 0이나 null 3,4.클래스명과 캡션명을 

        [DllImport("user32.dll")]
        public static extern IntPtr GetWindow(IntPtr hWnd, int uCmd);

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        const int GW_HWNDFIRST = 0;
        const int GW_HWNDLAST = 1;
        const int GW_HWNDNEXT = 2;
        const int GW_HWNDPREV = 3;
        const int GW_OWNER = 4;
        const int GW_CHILD = 5;

        const uint WM_SETTEXT = 0x000C;
        const uint WM_KEYDOWN = 0x0100;
        const uint WM_KEYUP = 0x0101;
        const uint WM_CHAR = 0x0102;

        const int VK_TAB = 0x09;
        const int VK_CANCEL = 0x03;
        const int VK_ENTER = 0x0D;
        const int VK_UP = 0x26;
        const int VK_DOWN = 0x28;
        const int VK_RIGHT = 0x27;
        const int VK_BACKSPACE = 0x08;

        const int BM_CLICK = 0X00F5;
        #endregion

        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            try
            {
                logger.Info("Application Started");

                ProcessUtility.Kill("DaishinSessionManager", Process.GetCurrentProcess().Id);
                ProcessUtility.Kill("CpStart"); // 키보드 보안 및 메모리 보안 프로그램 사용 체크 해제해야 함 (CpStart -> 설정)
                ProcessUtility.Kill("DibServer");

                LaunchStarter();

                // 중복실행 방지 팝업 정리 
                //CloseDuplicationPopup(); //=> CpStart Process Kill하면서 필요없는듯

                // 보안 경고창 Close 
                ClosePopup("대신증권 CYBOS FAMILY", "예(&Y)"); //=> 키보드 보안 및 메모리 보안 프로그램 사용 미체크 시 Popup뜸

                // Find CYBOS Starter
                var cybosStarterH = FindWindowAndRetry("CYBOS Starter");
                if (cybosStarterH == IntPtr.Zero)
                {
                    logger.Error("CYBOS Starter not found");
                    return;
                }

                // Click CYBOS plus Button
                ClickCybosPlusButton(cybosStarterH);

                // UserPw, CertPw 핸들 찾기
                IntPtr idH = GetWindow(cybosStarterH, GW_CHILD);
                IntPtr pwH = GetWindow(idH, GW_HWNDNEXT);
                IntPtr certPwH = GetWindow(pwH, GW_HWNDNEXT);

                // UserPw, CertPw 삭제
                for (int i = 0; i < 20; i++)
                {
                    SendMessage(pwH, WM_CHAR, VK_BACKSPACE, 0);
                    SendMessage(certPwH, WM_CHAR, VK_BACKSPACE, 0);
                }

                EnterUserPw(pwH);
                EnterCertPw(certPwH);

                ClickLoginButton(cybosStarterH);

                ClosePopup("인증서 만료 안내", "나중에 갱신");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                logger.Info("Application finished");
            }
        }

        private static void ClickCybosPlusButton(IntPtr cybosStarterH)
        {
            try
            {
                var plusButtonH = FindWindowExAndRetry(cybosStarterH, "Button", "PLUS");
                if (plusButtonH != IntPtr.Zero)
                {
                    SendMessage(plusButtonH, BM_CLICK, 0, 0);
                    logger.Info("CYBOS plus button clicked");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private static void EnterUserPw(IntPtr pwH)
        {
            try
            {
                string userPw = Config.Daishin.UserPw;
                if (string.IsNullOrWhiteSpace(userPw) == false)
                {
                    foreach (var c in userPw.ToCharArray())
                    {
                        SendMessage(pwH, WM_CHAR, c, 0);
                    }
                }
                else
                {
                    logger.Error("User password empty");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private static void EnterCertPw(IntPtr certPwH)
        {
            try
            {
                string certPw = Config.Daishin.CertPw;
                if (string.IsNullOrWhiteSpace(certPw) == false)
                {
                    foreach (var c in certPw.ToCharArray())
                    {
                        SendMessage(certPwH, WM_CHAR, c, 0);
                    }
                }
                else
                {
                    logger.Error("Certification password empty");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private static void ClickLoginButton(IntPtr cybosStarterH)
        {
            try
            {
                var loginButtonH = FindWindowExAndRetry(cybosStarterH, "Button", "연결");
                if (loginButtonH != IntPtr.Zero)
                {
                    SendMessage(loginButtonH, BM_CLICK, 0, 0);
                    logger.Info("Login button clicked");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private static void CloseDuplicationPopup()
        {
            try
            {
                var ncStarterH = FindWindowAndRetry("ncStarter", 100);
                if (ncStarterH != IntPtr.Zero)
                {
                    var cancelButtonH = FindWindowExAndRetry(ncStarterH, "Button", "아니요(&N)");
                    if (cancelButtonH != IntPtr.Zero)
                    {
                        SendMessage(cancelButtonH, BM_CLICK, 0, 0);
                        logger.Info("Duplicated launcher popup closed");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private static void ClosePopup(string title, string buttonName)
        {
            try
            {
                var certWindowH = FindWindowAndRetry(title);
                if (certWindowH != IntPtr.Zero)
                {
                    logger.Info($"{title} popup handle found");
                    var applyButtonH = FindWindowExAndRetry(certWindowH, "Button", buttonName);
                    if (applyButtonH != IntPtr.Zero)
                    {
                        SendMessage(applyButtonH, BM_CLICK, 0, 0);
                        logger.Info($"{buttonName} info popup closed");
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private static void LaunchStarter()
        {
            ProcessUtility.Start("C:\\Daishin\\STARTER\\ncStarter.exe", "/prj:cp", waitIdle: true);
        }

        private static IntPtr FindWindowAndRetry(string windowName, int retryCount = 30, int interval = 100, bool setForeground = true)
        {
            try
            {
                while (retryCount-- > 0)
                {
                    IntPtr handle = FindWindow(null, windowName);
                    if (handle != IntPtr.Zero)
                    {
                        logger.Info($"[{windowName}/{handle}] window found");

                        if (setForeground)
                        {
                            SetForegroundWindow(handle);

                            logger.Info($"[{windowName}/{handle}] window set foreground");
                            Thread.Sleep(1000);
                        }

                        return handle;
                    }

                    Thread.Sleep(interval);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            logger.Info($"[{windowName}] window not found");
            return IntPtr.Zero;
        }

        private static IntPtr FindWindowExAndRetry(IntPtr window, string className, string caption, int retryCount = 30, int interval = 100)
        {
            try
            {
                while (retryCount-- > 0)
                {
                    IntPtr handle = FindWindowEx(window, IntPtr.Zero, className, caption);
                    if (handle != IntPtr.Zero)
                    {
                        logger.Info($"[{caption}/{handle}] control found");
                        return handle;
                    }

                    Thread.Sleep(interval);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }

            logger.Info($"[{caption}] control not found");
            return IntPtr.Zero;
        }
    }
}
