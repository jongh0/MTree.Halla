using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;
using Configuration;
using CommonLib;
using CommonLib.Utility;
using CommonLib.Windows;

namespace DaishinSessionManager
{
    class Program
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            try
            {
                _logger.Info("Application Started");

                if (string.IsNullOrEmpty(Config.Daishin.UserId) == true ||
                    string.IsNullOrEmpty(Config.Daishin.UserPw) == true ||
                    string.IsNullOrEmpty(Config.Daishin.CertPw) == true)
                {
                    _logger.Error("Check Daishin configuration");
                    return;
                }

                ProcessUtility.Kill("DaishinSessionManager", Process.GetCurrentProcess().Id);
                ProcessUtility.Kill("CpStart"); // 키보드 보안 및 메모리 보안 프로그램 사용 체크 해제해야 함 (CpStart -> 설정)
                ProcessUtility.Kill("DibServer");

                LaunchStarter();

                while(true)
                {
                    // 중복실행 방지 팝업 정리 
                    ClosePopup("ncStarter", "예(&Y)"); 

                    // 보안 경고창 Close 
                    if (ClosePopup("대신증권 CYBOS FAMILY", "예(&Y)")) //=> 키보드 보안 및 메모리 보안 프로그램 사용 미체크 시 Popup뜸
                        break;

                    Thread.Sleep(1000);
                }

                // Find CYBOS Starter
                var cybosStarterH = WindowsAPI.findWindow("CYBOS Starter", retryCount: 100);
                if (cybosStarterH == IntPtr.Zero)
                {
                    _logger.Error("CYBOS Starter not found");
                    return;
                }

                // Click CYBOS plus Button
                ClickCybosPlusButton(cybosStarterH);

                // UserPw, CertPw 핸들 찾기
                IntPtr idH = WindowsAPI.getWindow(cybosStarterH, WindowsAPI.GW_CHILD);
                IntPtr pwH = WindowsAPI.getWindow(idH, WindowsAPI.GW_HWNDNEXT);
                IntPtr certPwH = WindowsAPI.getWindow(pwH, WindowsAPI.GW_HWNDNEXT);

                EnterUserPw(pwH);
                EnterCertPw(certPwH);

                ClickLoginButton(cybosStarterH);

                ClosePopup("인증서 만료 안내", "나중에 갱신");

                while (true)
                {
                    Process dibServer = Process.GetProcessesByName("DibServer")[0];
                    if (dibServer != null)
                    {
                        dibServer.PriorityClass = ProcessPriorityClass.RealTime;
                        _logger.Info("Set DibServer Priority to Realtime");
                        break;
                    }
                    Thread.Sleep(1000);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            finally
            {
                Thread.Sleep(1000 * 20); // Login 완료까지 여유시간
                _logger.Info("Application finished");
            }
        }

        private static void ClickCybosPlusButton(IntPtr cybosStarterH)
        {
            try
            {
                var plusButtonH = WindowsAPI.findWindowEx(cybosStarterH, "Button", "PLUS", retryCount: 10);
                if (plusButtonH != IntPtr.Zero)
                {
                    WindowsAPI.setForegroundWindow(cybosStarterH);
                    WindowsAPI.sendMessage(plusButtonH, WindowsAPI.BM_CLICK, 0, 0);

                    _logger.Info("CYBOS plus button clicked");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
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
                    _logger.Error("User password empty");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
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
                    _logger.Error("Certification password empty");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        private static void ClickLoginButton(IntPtr cybosStarterH)
        {
            try
            {
                var buttonH = WindowsAPI.findWindowEx(cybosStarterH, "Button", "연결");
                if (buttonH != IntPtr.Zero)
                {
                    WindowsAPI.setForegroundWindow(cybosStarterH);
                    WindowsAPI.sendMessage(buttonH, WindowsAPI.BM_CLICK, 0, 0);

                    _logger.Info("Login button clicked");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }
        
        private static bool ClosePopup(string title, string buttonName)
        {
            try
            {
                var popupH = WindowsAPI.findWindow(title, retryCount: 100);
                if (popupH != IntPtr.Zero)
                {
                    _logger.Info($"{title} popup handle found");

                    var buttonH = WindowsAPI.findWindowEx(popupH, "Button", buttonName, retryCount: 100);
                    if (buttonH != IntPtr.Zero)
                    {
                        WindowsAPI.setForegroundWindow(popupH);
                        WindowsAPI.sendMessage(buttonH, WindowsAPI.BM_CLICK, 0, 0);

                        _logger.Info($"{buttonName} info popup closed");
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
