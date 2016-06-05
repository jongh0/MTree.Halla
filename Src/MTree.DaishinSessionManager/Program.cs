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
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            try
            {
                logger.Info("Application Started");

                if (string.IsNullOrEmpty(Config.Daishin.UserId) == true ||
                    string.IsNullOrEmpty(Config.Daishin.UserPw) == true ||
                    string.IsNullOrEmpty(Config.Daishin.CertPw) == true)
                {
                    logger.Error("Check Daishin configuration");
                    return;
                }

                ProcessUtility.Kill("DaishinSessionManager", Process.GetCurrentProcess().Id);
                ProcessUtility.Kill("CpStart"); // 키보드 보안 및 메모리 보안 프로그램 사용 체크 해제해야 함 (CpStart -> 설정)
                ProcessUtility.Kill("DibServer");

                LaunchStarter();

                // 중복실행 방지 팝업 정리 
                //CloseDuplicationPopup(); //=> CpStart Process Kill하면서 필요없는듯

                // 보안 경고창 Close 
                ClosePopup("대신증권 CYBOS FAMILY", "예(&Y)"); //=> 키보드 보안 및 메모리 보안 프로그램 사용 미체크 시 Popup뜸

                // Find CYBOS Starter
                var cybosStarterH = WindowsAPI.findWindow("CYBOS Starter", retryCount: 100);
                if (cybosStarterH == IntPtr.Zero)
                {
                    logger.Error("CYBOS Starter not found");
                    return;
                }

                // Click CYBOS plus Button
                ClickCybosPlusButton(cybosStarterH);

                // UserPw, CertPw 핸들 찾기
                IntPtr idH = WindowsAPI.getWindow(cybosStarterH, WindowsAPI.GW_CHILD);
                IntPtr pwH = WindowsAPI.getWindow(idH, WindowsAPI.GW_HWNDNEXT);
                IntPtr certPwH = WindowsAPI.getWindow(pwH, WindowsAPI.GW_HWNDNEXT);

#if false
                // UserPw, CertPw 삭제
                for (int i = 0; i < 20; i++)
                {
                    WindowUtility.SendMessage2(pwH, WindowUtility.WM_CHAR, WindowUtility.VK_BACKSPACE, 0);
                    WindowUtility.SendMessage2(certPwH, WindowUtility.WM_CHAR, WindowUtility.VK_BACKSPACE, 0);
                }
#endif

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
                Thread.Sleep(1000 * 20); // Login 완료까지 여유시간
                logger.Info("Application finished");
            }
        }

        private static void ClickCybosPlusButton(IntPtr cybosStarterH)
        {
            try
            {
                var plusButtonH = WindowsAPI.findWindowEx(cybosStarterH, "Button", "PLUS", retryCount: 10);
                if (plusButtonH != IntPtr.Zero)
                {
                    WindowsAPI.sendMessage(plusButtonH, WindowsAPI.BM_CLICK, 0, 0);
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
                        WindowsAPI.sendMessage(pwH, WindowsAPI.WM_CHAR, c, 0);
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
                        WindowsAPI.sendMessage(certPwH, WindowsAPI.WM_CHAR, c, 0);
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
                var loginButtonH = WindowsAPI.findWindowEx(cybosStarterH, "Button", "연결");
                if (loginButtonH != IntPtr.Zero)
                {
                    WindowsAPI.sendMessage(loginButtonH, WindowsAPI.BM_CLICK, 0, 0);
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
                var ncStarterH = WindowsAPI.findWindow("ncStarter", 10);
                if (ncStarterH != IntPtr.Zero)
                {
                    var cancelButtonH = WindowsAPI.findWindowEx(ncStarterH, "Button", "아니요(&N)", retryCount: 10);
                    if (cancelButtonH != IntPtr.Zero)
                    {
                        WindowsAPI.sendMessage(cancelButtonH, WindowsAPI.BM_CLICK, 0, 0);
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
                var certWindowH = WindowsAPI.findWindow(title, retryCount: 100);
                if (certWindowH != IntPtr.Zero)
                {
                    logger.Info($"{title} popup handle found");
                    var applyButtonH = WindowsAPI.findWindowEx(certWindowH, "Button", buttonName, retryCount: 100);
                    if (applyButtonH != IntPtr.Zero)
                    {
                        WindowsAPI.sendMessage(applyButtonH, WindowsAPI.BM_CLICK, 0, 0);
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
            try // 원격에서 Exception 발생하는거 방지
            {
                ProcessUtility.Start("C:\\Daishin\\STARTER\\ncStarter.exe", "/prj:cp")?.WaitForInputIdle();
            }
            catch
            { }
        }
    }
}
